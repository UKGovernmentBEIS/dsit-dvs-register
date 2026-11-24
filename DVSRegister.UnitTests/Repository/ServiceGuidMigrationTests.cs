using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class ServiceGuidMigrationTests(PostgresTestFixture fixture) : IAsyncLifetime
    {
        private const string PreviousMigration = "20260923125030_AddGuidToServices";
        private const string ServiceGuidMigration = "20261109164508_AddAndBackfillServiceGuid";

        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Backfill_VersionsOfOneServiceKey_ShareOneGuid_AndOtherKeysDiffer()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (version1, version2, other) = await SeedAsync(dbContext);
            await MigrateAsync(dbContext, PreviousMigration);

            await MigrateAsync(dbContext, ServiceGuidMigration);

            var guids = await dbContext.Service.AsNoTracking()
                .Where(s => new[] { version1, version2, other }.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.ServiceGuid);
            Assert.NotEqual(Guid.Empty, guids[version1]);
            Assert.Equal(guids[version1], guids[version2]);
            Assert.NotEqual(guids[version1], guids[other]);
        }

        [Fact]
        public async Task Backfill_ServiceKeyOwnedByTwoProviders_FailsVisibly()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (version1, version2, _) = await SeedAsync(dbContext);
            var otherProvider = RepositoryTestHelper.CreateProviderProfile(1, "Other provider");
            dbContext.ProviderProfile.Add(otherProvider);
            await dbContext.SaveChangesAsync();
            await MigrateAsync(dbContext, PreviousMigration);
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Service\" SET \"ProviderProfileId\" = {otherProvider.Id} WHERE \"Id\" = {version2}");

            var ex = await Assert.ThrowsAnyAsync<Exception>(() => MigrateAsync(dbContext, ServiceGuidMigration));

            Assert.Contains("more than one provider", ex.ToString());
        }

        [Fact]
        public async Task Backfill_DuplicateVersionNumbers_FailsVisibly()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (_, version2, _) = await SeedAsync(dbContext);
            await MigrateAsync(dbContext, PreviousMigration);
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Service\" SET \"ServiceVersion\" = 1 WHERE \"Id\" = {version2}");

            var ex = await Assert.ThrowsAnyAsync<Exception>(() => MigrateAsync(dbContext, ServiceGuidMigration));

            Assert.Contains("duplicate ServiceVersion", ex.ToString());
        }

        [Fact]
        public async Task Backfill_NonPositiveServiceKey_FailsVisibly()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (_, _, other) = await SeedAsync(dbContext);
            await MigrateAsync(dbContext, PreviousMigration);
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Service\" SET \"ServiceKey\" = 0 WHERE \"Id\" = {other}");

            var ex = await Assert.ThrowsAnyAsync<Exception>(() => MigrateAsync(dbContext, ServiceGuidMigration));

            Assert.Contains("non-positive ServiceKey", ex.ToString());
        }

        private static Task MigrateAsync(DVSRegisterDbContext dbContext, string targetMigration) =>
            dbContext.GetService<IMigrator>().MigrateAsync(targetMigration);

        // Key 6001 has two versions; key 6002 is a separate service
        private static async Task<(int Version1, int Version2, int Other)> SeedAsync(DVSRegisterDbContext dbContext)
        {
            ProviderProfile provider = RepositoryTestHelper.CreateProviderProfile(1, "Migration provider");
            dbContext.ProviderProfile.Add(provider);
            await dbContext.SaveChangesAsync();

            Service version1 = RepositoryTestHelper.CreateService(1, "Service v1", provider.Id, ServiceStatusEnum.Published, false, false, false, 6001);
            Service version2 = RepositoryTestHelper.CreateService(1, "Service v2", provider.Id, ServiceStatusEnum.Submitted, false, false, false, 6001);
            Service other = RepositoryTestHelper.CreateService(1, "Other service", provider.Id, ServiceStatusEnum.Published, false, false, false, 6002);
            version1.ServiceVersion = 1;
            version2.ServiceVersion = 2;
            other.ServiceVersion = 1;
            dbContext.Service.AddRange(version1, version2, other);
            await dbContext.SaveChangesAsync();
            return (version1.Id, version2.Id, other.Id);
        }
    }
}
