using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class CabRemovalRequestRepositoryTests
    {
        private readonly ILogger<CabRemovalRequestRepository> logger;
        private readonly PostgresTestFixture fixture;

        public CabRemovalRequestRepositoryTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
            logger = Substitute.For<ILogger<CabRemovalRequestRepository>>();
        }

        [Fact]
        public async Task UpdateRemovalStatus_ReturnSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var cabRemovalRequestRepository = new CabRemovalRequestRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            var removalResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabUserId, providerProfileId, serviceId, "test.user123@test.com", "test");

            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();

            Assert.True(removalResponse.Success);
            Assert.NotNull(service);
            Assert.Equal(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.Equal("test", service.RemovalReasonByCab);
        }

        [Fact]
        public async Task UpdateRemovalStatus_NullService_ReturnFailure()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var cabRemovalRequestRepository = new CabRemovalRequestRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            var removalResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabUserId, providerProfileId, serviceId, "test.user123@test.com", "test");

            var service = await dbContext.Service.Where(s => s.Id == 3 && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();
            Assert.Null(service);
        }

        [Fact]
        public async Task UpdateRemovalStatus_WrongCabId_ReturnFailure()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var cabRemovalRequestRepository = new CabRemovalRequestRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            int wrongCabId = cabUserId + 1;

            var removalResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(wrongCabId, providerProfileId, serviceId, "test.user123@test.com", "test");

            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();
            Assert.False(removalResponse.Success);
            Assert.NotNull(service);
            Assert.NotEqual(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.NotEqual("test", service.RemovalReasonByCab);
        }

        [Fact]
        public async Task UpdateRemovalStatus_WrongProviderId_ReturnFailure()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var cabRemovalRequestRepository = new CabRemovalRequestRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            int wrongProviderProfileId = providerProfileId + 1;

            var removalResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabUserId, wrongProviderProfileId, serviceId, "test.user123@test.com", "test");

            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();
            Assert.False(removalResponse.Success);
            Assert.NotNull(service);
            Assert.NotEqual(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.NotEqual("test", service.RemovalReasonByCab);
        }

        [Fact]
        public async Task UpdateRemovalStatus_WrongServiceId_ReturnFailure()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var cabRemovalRequestRepository = new CabRemovalRequestRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            int wrongServiceId = serviceId + 1;

            var removalResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabUserId, providerProfileId, wrongServiceId, "test.user123@test.com", "test");

            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();
            Assert.False(removalResponse.Success);
            Assert.NotNull(service);
            Assert.NotEqual(ServiceStatusEnum.CabAwaitingRemovalConfirmation, service.ServiceStatus);
            Assert.NotEqual("test", service.RemovalReasonByCab);
        }

        #region Private methods

        private void InitializeDbContext(out DVSRegisterDbContext dbContext)
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(fixture.GetConnectionString())
                .Options;
            dbContext = new DVSRegisterDbContext(options);
        }

        private async Task<int> SaveProviderProfileAsync(string registeredName, string loggedInUserId, int cabUserId, DVSRegisterDbContext dbContext)
        {
            var providerProfile = RepositoryTestHelper.CreateProviderProfile(cabUserId, registeredName);
            var providerEntity = await dbContext.ProviderProfile.AddAsync(providerProfile);
            await dbContext.SaveChangesAsync();
            return providerEntity.Entity.Id;
        }

        private async Task<int> SaveServiceAsync(int providerProfileId, DVSRegisterDbContext dbContext)
        {
            var service = RepositoryTestHelper.CreateService(1, "sample service 1", providerProfileId, ServiceStatusEnum.ReadyToPublish, false, false, false, 1);
            var serviceEntity = await dbContext.Service.AddAsync(service);
            await dbContext.SaveChangesAsync();
            int serviceId = serviceEntity.Entity.Id;
            return serviceEntity.Entity.Id;
        }

        #endregion
    }
}
