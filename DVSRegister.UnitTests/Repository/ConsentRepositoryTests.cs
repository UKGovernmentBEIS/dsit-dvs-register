using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;
using DVSRegister.Data.Repositories;
using DVSRegister.Data.Entities;
using NuGet.Common;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class ConsentRepositoryTests
    {
        private readonly ILogger<ConsentRepository> logger;
        private readonly PostgresTestFixture fixture;

        public ConsentRepositoryTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
            logger = Substitute.For<ILogger<ConsentRepository>>();
        }

        #region Opening the loop

        [Fact]
        public async Task UpdateServiceStatus_ReturnSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var consentRepository = new ConsentRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            var genericResponse = await consentRepository.UpdateServiceStatus(serviceId, ServiceStatusEnum.Received,"test.user123@test.com");

            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();

            Assert.True(genericResponse.Success);
            Assert.NotNull(service);
            Assert.Equal(ServiceStatusEnum.Received, service.ServiceStatus);
        }

        [Fact]
        public async Task RemoveApplicationToken_ReturnSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            var consentRepository = new ConsentRepository(dbContext, logger);

            int cabUserId = 1;
            int providerProfileId = await SaveProviderProfileAsync("company name", "test@test.com", cabUserId, dbContext);
            int serviceId = await SaveServiceAsync(providerProfileId, dbContext);

            var (token, tokenId) = await SaveTokenAsync(serviceId, dbContext);
            var wasRemoved = await consentRepository.RemoveProceedApplicationConsentToken(token, tokenId, "test.user123@test.com");
            var deletedToken = await dbContext.ProceedApplicationConsentToken.Where(s => s.Token == token && s.TokenId == tokenId).FirstOrDefaultAsync();
            var service = await dbContext.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabUserId).FirstOrDefaultAsync();

            Assert.True(wasRemoved);
            Assert.Equal(TokenStatusEnum.RequestCompleted, service.OpeningLoopTokenStatus);
            Assert.Null(deletedToken);
        }
        #endregion




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
            var service = RepositoryTestHelper.CreateService(1, "sample service 1", providerProfileId, ServiceStatusEnum.Published, false, false, false, 1);
            var serviceEntity = await dbContext.Service.AddAsync(service);
            await dbContext.SaveChangesAsync();
            return serviceEntity.Entity.Id;
        }

        private async Task<(string Token, string TokenId)> SaveTokenAsync(int serviceId, DVSRegisterDbContext dbContext)
        {
            var service = await dbContext.Service.Where(s => s.Id == serviceId ).FirstOrDefaultAsync();
            var token = RepositoryTestHelper.CreateProceedApplicationConsentToken(serviceId, service);
            var tokenEntity = await dbContext.ProceedApplicationConsentToken.AddAsync(token);
            await dbContext.SaveChangesAsync();
            return (tokenEntity.Entity.Token, tokenEntity.Entity.TokenId);
        }

        #endregion
    }
}
