using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class CabRemovalRequestRepositoryTests : IAsyncLifetime
    {
        private const string CabUserEmail = "test.user123@ie.ey.com";
        private readonly ILogger<CabRemovalRequestRepository> logger;
        private readonly PostgresTestFixture fixture;

        public CabRemovalRequestRepositoryTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
            logger = Substitute.For<ILogger<CabRemovalRequestRepository>>();
        }

        public Task InitializeAsync() => fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task AddServiceRemovalRequest_ValidRequest_PersistsPendingRequestAndUpdatesService()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);
            DateTime requestedAfter = DateTime.UtcNow;

            GenericResponse response = await repository.AddServiceRemovalRequest(1, serviceId, CabUserEmail, "No longer certified");

            Service service = await dbContext.Service.SingleAsync(s => s.Id == serviceId);
            ServiceRemovalRequest request = await dbContext.ServiceRemovalRequest.SingleAsync(r => r.Id == response.InstanceId);
            Assert.True(response.Success);
            Assert.Equal(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.NotNull(service.ModifiedTime);
            Assert.True(service.ModifiedTime >= requestedAfter);
            Assert.Equal(serviceId, request.ServiceId);
            Assert.Equal("No longer certified", request.RemovalReasonByCab);
            Assert.Equal(ServiceStatusEnum.Published, request.PreviousServiceStatus);
            Assert.Equal(1, request.RemovalRequestedCabUserId);
            Assert.True(request.IsRequestPending);
            Assert.NotNull(request.RemovalRequestTime);
            Assert.True(request.RemovalRequestTime >= requestedAfter);
        }

        [Fact]
        public async Task AddServiceRemovalRequest_UnknownCabUserEmail_ReturnsFailureWithoutChanges()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            GenericResponse response = await repository.AddServiceRemovalRequest(1, serviceId, "unknown@example.com", "Reason");

            await dbContext.Entry(await dbContext.Service.SingleAsync(s => s.Id == serviceId)).ReloadAsync();
            Service service = await dbContext.Service.SingleAsync(s => s.Id == serviceId);
            Assert.False(response.Success);
            Assert.Equal(ServiceStatusEnum.Published, service.ServiceStatus);
            Assert.Empty(await dbContext.ServiceRemovalRequest.ToListAsync());
            Assert.Single(logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log)));
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(1, int.MaxValue)]
        public async Task AddServiceRemovalRequest_ServiceDoesNotBelongToCab_ReturnsFailureWithoutChanges(int cabId, int serviceId)
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int savedServiceId = await SaveServiceAsync(providerProfileId, dbContext);

            GenericResponse response = await repository.AddServiceRemovalRequest(cabId, serviceId, CabUserEmail, "Reason");

            Service service = await dbContext.Service.SingleAsync(s => s.Id == savedServiceId);
            Assert.False(response.Success);
            Assert.Equal(ServiceStatusEnum.Published, service.ServiceStatus);
            Assert.Empty(await dbContext.ServiceRemovalRequest.ToListAsync());
        }

        [Fact]
        public async Task AddServiceRemovalRequest_SaveThrows_ReturnsFailureRollsBackAndLogsError()
        {
            int serviceId;
            await using (var setupContext = CreateDbContext())
            {
                int providerProfileId = await SaveProviderProfileAsync(setupContext);
                serviceId = await SaveServiceAsync(providerProfileId, setupContext);
            }

            await using (var failingContext = CreateThrowingDbContext())
            {
                var repository = new CabRemovalRequestRepository(failingContext, logger);

                GenericResponse response = await repository.AddServiceRemovalRequest(1, serviceId, CabUserEmail, "Reason");

                Assert.False(response.Success);
            }

            await using var verificationContext = CreateDbContext();
            Service service = await verificationContext.Service.SingleAsync(s => s.Id == serviceId);
            Assert.Equal(ServiceStatusEnum.Published, service.ServiceStatus);
            Assert.Empty(await verificationContext.ServiceRemovalRequest.ToListAsync());
            Assert.Single(logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log)));
        }

        [Fact]
        public async Task CancelServiceRemovalRequest_PendingRequest_RestoresPreviousStatusAndCompletesRequest()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, ServiceStatusEnum.CabAwaitingRemovalConfirmation);
            int requestId = await SaveRemovalRequestAsync(serviceId, dbContext, true);

            GenericResponse response = await repository.CancelServiceRemovalRequest(serviceId, CabUserEmail);

            Service service = await dbContext.Service.SingleAsync(s => s.Id == serviceId);
            ServiceRemovalRequest request = await dbContext.ServiceRemovalRequest.SingleAsync(r => r.Id == requestId);
            Assert.True(response.Success);
            Assert.Equal(requestId, response.InstanceId);
            Assert.Equal(ServiceStatusEnum.Published, service.ServiceStatus);
            Assert.False(request.IsRequestPending);
        }

        [Theory]
        [InlineData(ServiceStatusEnum.Published, 1)]
        [InlineData(ServiceStatusEnum.CabAwaitingRemovalConfirmation, int.MaxValue)]
        public async Task CancelServiceRemovalRequest_RequestIsNotAwaitingConfirmation_ReturnsAlreadyProcessed(ServiceStatusEnum status, int requestedServiceId)
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, status);

            GenericResponse response = await repository.CancelServiceRemovalRequest(requestedServiceId, CabUserEmail);

            Service service = await dbContext.Service.SingleAsync(s => s.Id == serviceId);
            Assert.False(response.Success);
            Assert.Equal(ErrorTypeEnum.RequestAlreadyProcessed, response.ErrorType);
            Assert.Equal(status, service.ServiceStatus);
        }

        [Fact]
        public async Task CancelServiceRemovalRequest_NoPendingRemovalRequest_ReturnsFailureAndLogsError()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, ServiceStatusEnum.CabAwaitingRemovalConfirmation);
            int requestId = await SaveRemovalRequestAsync(serviceId, dbContext, false);

            GenericResponse response = await repository.CancelServiceRemovalRequest(serviceId, CabUserEmail);

            ServiceRemovalRequest request = await dbContext.ServiceRemovalRequest.SingleAsync(r => r.Id == requestId);
            Assert.False(response.Success);
            Assert.False(request.IsRequestPending);
            Assert.Single(logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log)));
        }

        [Fact]
        public async Task CancelServiceRemovalRequest_SaveThrows_ReturnsFailureRollsBackAndLogsError()
        {
            int serviceId;
            int requestId;
            await using (var setupContext = CreateDbContext())
            {
                int providerProfileId = await SaveProviderProfileAsync(setupContext);
                serviceId = await SaveServiceAsync(providerProfileId, setupContext, ServiceStatusEnum.CabAwaitingRemovalConfirmation);
                requestId = await SaveRemovalRequestAsync(serviceId, setupContext, true);
            }

            await using (var failingContext = CreateThrowingDbContext())
            {
                var repository = new CabRemovalRequestRepository(failingContext, logger);

                GenericResponse response = await repository.CancelServiceRemovalRequest(serviceId, CabUserEmail);

                Assert.False(response.Success);
            }

            await using var verificationContext = CreateDbContext();
            Service service = await verificationContext.Service.SingleAsync(s => s.Id == serviceId);
            ServiceRemovalRequest request = await verificationContext.ServiceRemovalRequest.SingleAsync(r => r.Id == requestId);
            Assert.Equal(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.True(request.IsRequestPending);
            Assert.Single(logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log)));
        }

        [Fact]
        public async Task IsLastService_ProviderDoesNotExist_ReturnsFalse()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);

            bool result = await repository.IsLastService(1, int.MaxValue);

            Assert.False(result);
        }

        [Fact]
        public async Task IsLastService_ProviderHasNoOtherServices_ReturnsTrue()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, isInRegister: true);

            bool result = await repository.IsLastService(serviceId, providerProfileId);

            Assert.True(result);
        }

        [Fact]
        public async Task IsLastService_OtherServicesAreNotInRegister_ReturnsTrue()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, isInRegister: true);
            await SaveServiceAsync(providerProfileId, dbContext, isInRegister: false, serviceKey: 2);

            bool result = await repository.IsLastService(serviceId, providerProfileId);

            Assert.True(result);
        }

        [Fact]
        public async Task IsLastService_OtherServiceIsInRegister_ReturnsFalse()
        {
            await using var dbContext = CreateDbContext();
            var repository = new CabRemovalRequestRepository(dbContext, logger);
            int providerProfileId = await SaveProviderProfileAsync(dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext, isInRegister: true);
            await SaveServiceAsync(providerProfileId, dbContext, isInRegister: true, serviceKey: 2);

            bool result = await repository.IsLastService(serviceId, providerProfileId);

            Assert.False(result);
        }

        private DVSRegisterDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            return new DVSRegisterDbContext(options);
        }

        private ThrowingDVSRegisterDbContext CreateThrowingDbContext()
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            return new ThrowingDVSRegisterDbContext(options);
        }

        private static async Task<int> SaveProviderProfileAsync(DVSRegisterDbContext dbContext)
        {
            ProviderProfile providerProfile = RepositoryTestHelper.CreateProviderProfile(1, "Company name");
            var entity = await dbContext.ProviderProfile.AddAsync(providerProfile);
            await dbContext.SaveChangesAsync();
            return entity.Entity.Id;
        }

        private static async Task<int> SaveServiceAsync(int providerProfileId, DVSRegisterDbContext dbContext, ServiceStatusEnum status = ServiceStatusEnum.Published,  bool isInRegister = false, int serviceKey = 1)
        {
            Service service = RepositoryTestHelper.CreateService(1, $"Sample service {serviceKey}", providerProfileId, status, false, false, false, serviceKey);
            service.IsInRegister = isInRegister;
            var entity = await dbContext.Service.AddAsync(service);
            await dbContext.SaveChangesAsync();
            return entity.Entity.Id;
        }

        private static async Task<int> SaveRemovalRequestAsync(int serviceId, DVSRegisterDbContext dbContext, bool isPending)
        {
            var request = new ServiceRemovalRequest
            {
                ServiceId = serviceId,
                RemovalReasonByCab = "Reason",
                RemovalRequestTime = DateTime.UtcNow,
                PreviousServiceStatus = ServiceStatusEnum.Published,
                RemovalRequestedCabUserId = 1,
                IsRequestPending = isPending
            };
            var entity = await dbContext.ServiceRemovalRequest.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return entity.Entity.Id;
        }

        private sealed class ThrowingDVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options) : DVSRegisterDbContext(options)
        {
            public override Task<int> SaveChangesAsync(TeamEnum team = TeamEnum.NA, EventTypeEnum eventType = EventTypeEnum.NA, string actorId = null!) => throw new InvalidOperationException("Simulated database failure");
        }
    }
}
