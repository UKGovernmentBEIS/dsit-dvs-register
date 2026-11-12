using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.Register;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.PublishedService;
using DVSRegister.UnitTests.Helpers;
using DVSRegister.UnitTests.Repository;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace DVSRegister.UnitTests.PublishedService
{
    [Collection("Postgres Collection")]
    public class PublishedServiceHandlerTests(PostgresTestFixture fixture) : IAsyncLifetime
    {
        private readonly IRegisterService registerService = Substitute.For<IRegisterService>();

        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetPublishedService_NewerDraftExists_ReturnsPublishedRow()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            registerService.GetServiceDetails(published.Id).Returns(TestDataFactory.CreatePublishedServiceDto(provider.Guid, provider.RegisteredName, provider.Id, published.Id, published.ServiceGuid, published.ServiceName));

            var result = await new GetPublishedService(dbContext, registerService)
                .HandleAsync(provider.Guid, published.ServiceGuid, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(published.ServiceGuid, result.Value.ServiceGuid);
            Assert.Equal(provider.Guid, result.Value.ProviderGuid);
            await registerService.Received(1).GetServiceDetails(published.Id);
        }

        [Fact]
        public async Task GetPublishedService_ProviderDoesNotOwnService_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (_, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);

            var result = await new GetPublishedService(dbContext, registerService)
                .HandleAsync(Guid.NewGuid(), published.ServiceGuid, CancellationToken.None);

            AssertNotFound(result);
            await registerService.DidNotReceive().GetServiceDetails(Arg.Any<int>());
        }

        [Fact]
        public async Task GetPublishedService_NoPublishedRow_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            published.IsInRegister = false;
            await dbContext.SaveChangesAsync();

            var result = await new GetPublishedService(dbContext, registerService)
                .HandleAsync(provider.Guid, published.ServiceGuid, CancellationToken.None);

            AssertNotFound(result);
        }

        [Fact]
        public async Task GetPublishedService_MoreThanOnePublishedRow_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, draft) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            draft.IsInRegister = true;
            await dbContext.SaveChangesAsync();

            var result = await new GetPublishedService(dbContext, registerService)
                .HandleAsync(provider.Guid, published.ServiceGuid, CancellationToken.None);

            AssertNotFound(result);
        }

        [Fact]
        public async Task ResolveLegacyServiceLink_SupersededPublishedRow_RedirectsToLatest()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, draft) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            // old public row that was superseded by the currently published version
            published.IsInRegister = false;
            draft.ServiceStatus = ServiceStatusEnum.Published;
            draft.IsInRegister = true;
            await dbContext.SaveChangesAsync();

            var result = await new ResolveLegacyServiceLink(dbContext).HandleAsync(published.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(ServiceLinks.Latest(provider.Guid, published.ServiceGuid), result.Value);
        }

        [Fact]
        public async Task ResolveLegacyServiceLink_PublishedRow_RedirectsToLatest()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);

            var result = await new ResolveLegacyServiceLink(dbContext).HandleAsync(published.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(ServiceLinks.Latest(provider.Guid, published.ServiceGuid), result.Value);
        }

        [Fact]
        public async Task ResolveLegacyServiceLink_DraftRowId_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (_, _, draft) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);

            var result = await new ResolveLegacyServiceLink(dbContext).HandleAsync(draft.Id, CancellationToken.None);

            AssertNotFound(result);
        }

        [Fact]
        public async Task ResolveLegacyServiceLink_NoCurrentPublication_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (_, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            published.IsInRegister = false;
            await dbContext.SaveChangesAsync();

            var result = await new ResolveLegacyServiceLink(dbContext).HandleAsync(published.Id, CancellationToken.None);

            AssertNotFound(result);
        }

        [Fact]
        public async Task ResolveLegacyServiceLink_UnknownRowId_ReturnsNotFound()
        {
            await using var dbContext = fixture.CreateDbContext();

            var result = await new ResolveLegacyServiceLink(dbContext).HandleAsync(int.MaxValue, CancellationToken.None);

            AssertNotFound(result);
        }

        [Fact]
        public async Task ServiceGuidIndex_DuplicateServiceGuidAndVersion_IsRejected()
        {
            await using var dbContext = fixture.CreateDbContext();
            var (provider, published, _) = await RepositoryTestHelper.SeedPublishedWithNewerDraftAsync(dbContext);
            Service duplicate = RepositoryTestHelper.CreateService(1, "Duplicate version", provider.Id,
                ServiceStatusEnum.Submitted, false, false, false, published.ServiceKey);
            duplicate.ServiceGuid = published.ServiceGuid;
            duplicate.ServiceVersion = published.ServiceVersion;
            dbContext.Service.Add(duplicate);

            await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
        }

        private static void AssertNotFound<T>(Result<T> result)
        {
            Assert.True(result.IsFailure);
            Assert.Equal(PublishedServiceErrorCodes.NotFound, result.Error.Code);
        }
    }
}
