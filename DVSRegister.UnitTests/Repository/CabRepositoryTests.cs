using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests
{
    public class CabRepositoryTests : IClassFixture<PostgresTestFixture>
    {
        
        private DVSRegisterDbContext _dbContext;
        private readonly CabRepository cabRepository;
        private ILogger<CabRepository> logger;

        public CabRepositoryTests(PostgresTestFixture fixture)
        {            
            _dbContext = fixture.DbContext;
            logger = Substitute.For<ILogger<CabRepository>>();
            cabRepository = new CabRepository(_dbContext, logger);
        }     
      

        [Fact]
        public async Task SaveProviderProfile_Test()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfile(1, "ABC Company");            
            GenericResponse genericResponse= await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await _dbContext.ProviderProfile.FirstOrDefaultAsync(p=>p.Id == genericResponse.InstanceId);
            Assert.NotNull(providerProfile);
            Assert.Equal(providerProfileTest.RegisteredName, providerProfile.RegisteredName);
            Assert.Equal(providerProfileTest.TradingName, providerProfile.TradingName);
            Assert.Equal(providerProfileTest.HasRegistrationNumber, providerProfile.HasRegistrationNumber);
            Assert.Equal(providerProfileTest.CompanyRegistrationNumber, providerProfile.CompanyRegistrationNumber);
            Assert.Null(providerProfile.DUNSNumber);
            Assert.False(providerProfile.HasParentCompany);
            Assert.Null(providerProfile.ParentCompanyLocation);
            Assert.Null(providerProfile.ParentCompanyRegisteredName);
            Assert.Equal(providerProfileTest.PrimaryContactFullName, providerProfile.PrimaryContactFullName);
            Assert.Equal(providerProfileTest.PrimaryContactJobTitle, providerProfile.PrimaryContactJobTitle);
            Assert.Equal(providerProfileTest.PrimaryContactEmail, providerProfile.PrimaryContactEmail);
            Assert.Equal(providerProfileTest.PrimaryContactTelephoneNumber, providerProfile.PrimaryContactTelephoneNumber);
            Assert.Equal(providerProfileTest.SecondaryContactEmail, providerProfile.SecondaryContactEmail);
            Assert.Equal(providerProfileTest.SecondaryContactJobTitle, providerProfile.SecondaryContactJobTitle);
            Assert.Equal(providerProfileTest.SecondaryContactEmail, providerProfile.SecondaryContactEmail);
            Assert.Equal(providerProfileTest.SecondaryContactTelephoneNumber, providerProfile.SecondaryContactTelephoneNumber);
            Assert.Equal(providerProfileTest.PublicContactEmail, providerProfile.PublicContactEmail);
            Assert.Equal(providerProfileTest.ProviderTelephoneNumber, providerProfile.ProviderTelephoneNumber);
            Assert.Equal(providerProfileTest.ProviderWebsiteAddress, providerProfile.ProviderWebsiteAddress);

        }

        [Fact]
        public async Task SaveProviderProfileWithDunsNumber_Test()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfileWithDunsNumber(1, "ABC Company");
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await _dbContext.ProviderProfile.FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            Assert.NotNull(providerProfile);      
            Assert.False( providerProfile.HasRegistrationNumber);
            Assert.Null(providerProfile.CompanyRegistrationNumber);
            Assert.NotNull(providerProfile.DUNSNumber);
            Assert.Equal(providerProfileTest.DUNSNumber, providerProfile.DUNSNumber);         

        }


        [Fact]
        public async Task SaveProviderProfileWithParentCompany_Test()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfileWithParentCompany(1, "ABC Company");
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await _dbContext.ProviderProfile.FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            Assert.NotNull(providerProfile);
            Assert.True(providerProfile.HasParentCompany);
            Assert.NotNull(providerProfile.ParentCompanyRegisteredName);
            Assert.NotNull(providerProfile.ParentCompanyLocation);
           
            Assert.Equal(providerProfileTest.ParentCompanyRegisteredName, providerProfile.ParentCompanyRegisteredName);
            Assert.Equal(providerProfileTest.ParentCompanyLocation, providerProfile.ParentCompanyLocation);

        }

        [Fact]
        public async Task SaveProviderProfile_InvalidProfileDetails_ReturnsFailure()
        {
            ProviderProfile providerProfile = new();
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }
        [Fact]
        public async Task SaveProviderProfile_NullProviderProfile_ReturnsFailure()
        {
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(null!, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }

        [Fact]
        public async Task RegisteredNameExists_Test()
        {
            await SaveProviderProfileHelper("Test Company", "test.user123@test.com", 1);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company");
            Assert.True(actual);
        }

        [Fact]
        public async Task RegisteredNameNotExists_Test()
        {
            await SaveProviderProfileHelper("Test Company 123", "test.user123@test.com", 1);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company 1234");
            Assert.False(actual);
        }

        [Fact]
        public async Task RegisteredNameExists_SameProvider_Test()
        {
            int providerProfileId = await SaveProviderProfileHelper("Test Company", "test.user123@test.com", 1);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company", providerProfileId);
            Assert.False(actual);
        }

        [Fact]
        public async Task RegisteredNameExists_DifferentProvider_Test()
        {
            await SaveProviderProfileHelper("Test Company 1", "test.user123@test.com", 1);
            int providerProfileId = await SaveProviderProfileHelper("Test Company 2", "test.user123@test.com",1);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company 1", providerProfileId);
            Assert.True(actual);
        }

        private async Task<int> SaveProviderProfileHelper(string registeredName, string loggedInUserId, int cabUserId)
        {
            ProviderProfile providerProfile2 = UnitTestHelper.CreateProviderProfile(cabUserId, registeredName);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile2, loggedInUserId);
            Assert.NotEqual(0, genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
            return genericResponse.InstanceId;
        }
    }
}
