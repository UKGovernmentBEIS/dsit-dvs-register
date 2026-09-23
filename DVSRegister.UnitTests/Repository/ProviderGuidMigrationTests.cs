using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Data;

namespace DVSRegister.UnitTests.Repository;

[Collection("Postgres Collection")]
public class ProviderGuidMigrationTests
{
    private const string MigrationBeforeProviderGuid = "20260818132102_AddServiceTypeToReport";
    private readonly PostgresTestFixture fixture;

    public ProviderGuidMigrationTests(PostgresTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Migration_BackfillsExistingProvidersWithUniqueGuids()
    {
        await fixture.ResetAsync();
        await MigrateToAsync(MigrationBeforeProviderGuid);
        await fixture.DbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO "ProviderProfile" ("RegisteredName", "ProviderStatus", "EditProviderTokenStatus", "IsInRegister")
            VALUES ('Existing Provider One', 0, 0, false), ('Existing Provider Two', 0, 0, false);
            """);

        await fixture.DbContext.Database.MigrateAsync();
        var guids = await fixture.DbContext.ProviderProfile
            .Select(provider => provider.Guid)
            .ToListAsync();

        Assert.Equal(2, guids.Count);
        Assert.All(guids, guid => Assert.NotEqual(Guid.Empty, guid));
        Assert.Equal(guids.Count, guids.Distinct().Count());
    }

    [Fact]
    public async Task Migration_Rollback_RemovesProviderGuidColumnAndIndex()
    {
        await fixture.ResetAsync();
        await MigrateToAsync(MigrationBeforeProviderGuid);

        var columnCount = await ExecuteScalarAsync<int>(
            fixture.DbContext,
            """SELECT COUNT(*) FROM information_schema.columns WHERE table_name = 'ProviderProfile' AND column_name = 'Guid';""");
        var indexCount = await ExecuteScalarAsync<int>(
            fixture.DbContext,
            """SELECT COUNT(*) FROM pg_indexes WHERE tablename = 'ProviderProfile' AND indexname = 'IX_ProviderProfile_Guid';""");

        Assert.Equal(0, columnCount);
        Assert.Equal(0, indexCount);
    }

    [Fact]
    public async Task DatabaseDefault_GeneratesGuidWhenProviderInsertOmitsGuid()
    {
        await fixture.ResetAsync();
        await using var command = fixture.DbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = """
            INSERT INTO "ProviderProfile" ("RegisteredName", "ProviderStatus", "EditProviderTokenStatus", "IsInRegister")
            VALUES ('Direct Insert Provider', 0, 0, false)
            RETURNING "Guid";
            """;

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync();

        var value = await command.ExecuteScalarAsync();

        Assert.NotNull(value);
        Assert.NotEqual(Guid.Empty, (Guid)value);
    }

    private static async Task<T> ExecuteScalarAsync<T>(DVSRegisterDbContext dbContext, string sql)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync();

        return (T)Convert.ChangeType((await command.ExecuteScalarAsync())!, typeof(T));
    }

    private async Task MigrateToAsync(string migration)
    {
        var migrator = fixture.DbContext.Database.GetService<IMigrator>();
        await migrator.MigrateAsync(migration);
    }
}
