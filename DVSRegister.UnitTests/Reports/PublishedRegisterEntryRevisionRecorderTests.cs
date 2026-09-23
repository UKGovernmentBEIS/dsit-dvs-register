using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Reports.RegisterHistory;
using DVSRegister.UnitTests.Repository;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.UnitTests.Reports
{
    [Collection("Postgres Collection")]
    public class PublishedRegisterEntryRevisionRecorderTests : IAsyncLifetime
    {
        private readonly PostgresTestFixture fixture;

        public PublishedRegisterEntryRevisionRecorderTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
        }

        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task RecordAsync_RemovalRequestHasDifferentRemovedTime_UsesServiceRemovedTime()
        {
            await using var dbContext = CreateDbContext();
            var serviceRemovedTime = new DateTime(2026, 9, 18, 10, 30, 0, DateTimeKind.Utc);
            var requestRemovedTime = new DateTime(2026, 9, 17, 9, 15, 0, DateTimeKind.Utc);
            int serviceId = await SaveRemovedServiceAsync(dbContext, serviceRemovedTime);
            var removalRequest = new ServiceRemovalRequest
            {
                ServiceId = serviceId,
                IsRequestPending = false,
                RemovedTime = requestRemovedTime,
                PreviousServiceStatus = ServiceStatusEnum.Published
            };
            dbContext.ServiceRemovalRequest.Add(removalRequest);
            await dbContext.SaveChangesAsync();

            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId],
                RegisterHistoryActivityKind.Removed, "test-removal", $"request:{removalRequest.Id}",
                "Approved service removal.");

            PublishedRegisterEntryRevision revision = await dbContext.PublishedRegisterEntryRevisions
                .SingleAsync(entry => entry.ServiceId == serviceId);
            Assert.Equal(serviceRemovedTime, revision.RemovedOn);
            Assert.NotEqual(requestRemovedTime, revision.RemovedOn);
        }

        [Fact]
        public void CreateSourceId_SameInputs_ReturnsStableDistinctHash()
        {
            string first = PublishedRegisterEntryRevisionRecorder.CreateSourceId("update", 42, "one", null, "three");
            string repeated = PublishedRegisterEntryRevisionRecorder.CreateSourceId("update", 42, "one", null, "three");
            string changed = PublishedRegisterEntryRevisionRecorder.CreateSourceId("update", 42, "one", "two", "three");

            Assert.Equal(first, repeated);
            Assert.StartsWith("update:42:", first);
            Assert.NotEqual(first, changed);
        }

        [Fact]
        public async Task RecordAsync_EmptyAndRepeatedServiceIds_DoesNotCreateDuplicateRevision()
        {
            await using var dbContext = CreateDbContext();
            int serviceId = await SaveRemovedServiceAsync(dbContext, DateTime.UtcNow);

            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [],
                RegisterHistoryActivityKind.Publication, "publication", "empty", "Ignored");
            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId, serviceId],
                RegisterHistoryActivityKind.Publication, "publication", "same-source", new string('x', 4100));
            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId],
                RegisterHistoryActivityKind.Publication, "publication", "same-source", "Duplicate");

            PublishedRegisterEntryRevision revision = await dbContext.PublishedRegisterEntryRevisions.SingleAsync();
            Assert.Equal(4000, revision.AdditionalInformation.Length);
        }

        [Fact]
        public async Task RecordAsync_ServiceUpdate_CreatesVisibleFieldChangeSummary()
        {
            await using var dbContext = CreateDbContext();
            int serviceId = await SaveRemovedServiceAsync(dbContext, DateTime.UtcNow);
            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId],
                RegisterHistoryActivityKind.Publication, "publication", "initial", "Published");

            Service service = await dbContext.Service.Include(entry => entry.Provider)
                .SingleAsync(entry => entry.Id == serviceId);
            service.ServiceName = "Updated service";
            service.WebSiteAddress = "https://updated.example.com";
            service.HasGPG44 = true;
            service.Provider.PublicContactEmail = "updated@example.com";
            await dbContext.SaveChangesAsync();

            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId],
                RegisterHistoryActivityKind.ServiceUpdated, "service-update", "updated", "Ignored");
            await PublishedRegisterEntryRevisionRecorder.RecordAsync(dbContext, [serviceId],
                RegisterHistoryActivityKind.ProviderUpdated, "provider-update", "unchanged", "Ignored");

            List<PublishedRegisterEntryRevision> revisions = await dbContext.PublishedRegisterEntryRevisions
                .OrderBy(entry => entry.Id).ToListAsync();
            Assert.Contains("Service name: Removal test service → Updated service", revisions[1].AdditionalInformation);
            Assert.Contains("Public email: contact@abccorp.com → updated@example.com", revisions[1].AdditionalInformation);
            Assert.StartsWith("Changes:", revisions[2].AdditionalInformation);
        }

        [Fact]
        public async Task GetPublishedServiceIdsForProviderAsync_ReturnsOwnAndUnderpinnedPublishedServices()
        {
            await using var dbContext = CreateDbContext();
            int underpinningServiceId = await SavePublishedServiceAsync(dbContext, 2001, "Underpinning service");
            ProviderProfile otherProvider = RepositoryTestHelper.CreateProviderProfile(1, "Other provider");
            dbContext.ProviderProfile.Add(otherProvider);
            await dbContext.SaveChangesAsync();
            Service dependent = RepositoryTestHelper.CreateService(1, "Dependent service", otherProvider.Id,
                ServiceStatusEnum.Published, false, false, false, 2002);
            dependent.IsInRegister = true;
            dependent.UnderPinningServiceId = underpinningServiceId;
            dbContext.Service.Add(dependent);
            await dbContext.SaveChangesAsync();
            int providerId = await dbContext.Service.Where(entry => entry.Id == underpinningServiceId)
                .Select(entry => entry.ProviderProfileId).SingleAsync();

            List<int> serviceIds = await PublishedRegisterEntryRevisionRecorder
                .GetPublishedServiceIdsForProviderAsync(dbContext, providerId);

            Assert.Contains(underpinningServiceId, serviceIds);
            Assert.Contains(dependent.Id, serviceIds);
        }

        private DVSRegisterDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            return new DVSRegisterDbContext(options);
        }

        private static async Task<int> SaveRemovedServiceAsync(DVSRegisterDbContext dbContext,
            DateTime removedTime)
        {
            ProviderProfile provider = RepositoryTestHelper.CreateProviderProfile(1, "Removal test provider");
            provider.IsInRegister = false;
            provider.ProviderStatus = ProviderStatusEnum.AwaitingRemovalConfirmation;
            dbContext.ProviderProfile.Add(provider);
            await dbContext.SaveChangesAsync();

            Service service = RepositoryTestHelper.CreateService(1, "Removal test service", provider.Id,
                ServiceStatusEnum.Removed, false, false, false, 1001);
            service.IsInRegister = false;
            service.RemovedTime = removedTime;
            dbContext.Service.Add(service);
            await dbContext.SaveChangesAsync();
            return service.Id;
        }

        private static async Task<int> SavePublishedServiceAsync(DVSRegisterDbContext dbContext, int serviceKey,
            string serviceName)
        {
            ProviderProfile provider = RepositoryTestHelper.CreateProviderProfile(1, $"Provider {serviceKey}");
            provider.IsInRegister = true;
            dbContext.ProviderProfile.Add(provider);
            await dbContext.SaveChangesAsync();

            Service service = RepositoryTestHelper.CreateService(1, serviceName, provider.Id,
                ServiceStatusEnum.Published, false, false, false, serviceKey);
            service.IsInRegister = true;
            dbContext.Service.Add(service);
            await dbContext.SaveChangesAsync();
            return service.Id;
        }
    }
}
