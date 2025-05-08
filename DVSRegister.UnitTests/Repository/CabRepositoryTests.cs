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


        #region Provider

        [Fact]
        public async Task SaveProviderProfile_Test()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfile(1, "ABC Company");            
            GenericResponse genericResponse= await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await _dbContext.ProviderProfile.FirstOrDefaultAsync(p=>p.Id == genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
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
            Assert.True(genericResponse.Success);
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
            Assert.True(genericResponse.Success);
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


     
        #endregion


        #region Service



        [Fact]
        public async Task SaveServiceWithRoles_Test()
        {
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service", providerProfileId, ServiceStatusEnum.Submitted, null,null,null,0) ;
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            Service? savedService = await _dbContext.Service.Include(p=>p.ServiceRoleMapping).ThenInclude(p=>p.Role).FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            List<Role> roles = await _dbContext.Role.ToListAsync();

            Assert.True(genericResponse.Success);
            Assert.NotNull(savedService);
            Assert.Equal(serviceTest.ServiceName, savedService.ServiceName);
            Assert.Equal(1, savedService.ServiceVersion);
            Assert.Equal(genericResponse.InstanceId, savedService.ServiceKey);
            Assert.Null(savedService.HasGPG44);
            Assert.Null(savedService.HasGPG45);
            Assert.Null(savedService.HasSupplementarySchemes);
            Assert.NotNull(savedService.ServiceRoleMapping);
            foreach(var roleMapping in savedService.ServiceRoleMapping)
            {
                Assert.NotNull(roleMapping);
                Assert.NotNull(roleMapping.Role);
                Assert.Equal(roles.Where(r=>r.Id == roleMapping.RoleId).Select(r=>r.RoleName).FirstOrDefault(), roleMapping.Role.RoleName);
            }         

        }

        [Fact]
        public async Task SaveServiceWithGpg44Gpg55Schemes_Test()
        {
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service", providerProfileId, ServiceStatusEnum.Submitted, true, true, true,0);
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            Service? savedService = await _dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);

            List<QualityLevel> qualityLevels = await _dbContext.QualityLevel.ToListAsync();
            List<IdentityProfile> identityProfiles = await _dbContext.IdentityProfile.ToListAsync();
            List<SupplementaryScheme> supplementarySchemes = await _dbContext.SupplementaryScheme.ToListAsync();

            Assert.True(genericResponse.Success);
            Assert.NotNull(savedService);
            Assert.Equal(serviceTest.ServiceName, savedService.ServiceName);
            Assert.Equal(1, savedService.ServiceVersion);
            Assert.Equal(genericResponse.InstanceId, savedService.ServiceKey);
            Assert.True(savedService.HasGPG44);
            Assert.True(savedService.HasGPG45);
            Assert.True(savedService.HasSupplementarySchemes);
            Assert.NotNull(savedService.ServiceRoleMapping);
            Assert.NotNull(savedService.ServiceQualityLevelMapping);
            Assert.NotNull(savedService.ServiceIdentityProfileMapping);
            Assert.NotNull(savedService.ServiceSupSchemeMapping);
            foreach (var qualityLevelMapping in savedService.ServiceQualityLevelMapping)
            {
                Assert.NotNull(qualityLevelMapping);
                Assert.NotNull(qualityLevelMapping.QualityLevel);
                Assert.Equal(qualityLevels.Where(r => r.Id == qualityLevelMapping.QualityLevelId).Select(r => r.Level).FirstOrDefault(), qualityLevelMapping.QualityLevel.Level);
            }

            foreach (var identityProfileMapping in savedService.ServiceIdentityProfileMapping)
            {
                Assert.NotNull(identityProfileMapping);
                Assert.NotNull(identityProfileMapping.IdentityProfile);
                Assert.Equal(identityProfiles.Where(r => r.Id == identityProfileMapping.IdentityProfileId).Select(r => r.IdentityProfileName).FirstOrDefault(), identityProfileMapping.IdentityProfile.IdentityProfileName);
            }

            foreach (var serviceSupSchemeMapping in savedService.ServiceSupSchemeMapping)
            {
                Assert.NotNull(serviceSupSchemeMapping);
                Assert.NotNull(serviceSupSchemeMapping.SupplementaryScheme);
                Assert.Equal(supplementarySchemes.Where(r => r.Id == serviceSupSchemeMapping.SupplementarySchemeId).Select(r => r.SchemeName).FirstOrDefault(), serviceSupSchemeMapping.SupplementaryScheme.SchemeName);
            }

        }


        [Fact]
        public async Task SaveService_InvalidServiceDetails_ReturnsFailure()
        {
            Service service = new();
            GenericResponse genericResponse = await cabRepository.SaveService(service, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }


        [Theory]
        [InlineData(ServiceStatusEnum.Submitted, "New submitted service")]
        [InlineData(ServiceStatusEnum.SavedAsDraft, "New draft service")]

        // Save a service first as draft, then again update samae service - first as draft then submit 
        public async Task SaveServiceWithNewVersion_Test(ServiceStatusEnum serviceStatus, string serviceName)
        {
            int serviceKey = 0;
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service draft 1", providerProfileId, ServiceStatusEnum.SavedAsDraft, true, true, true, serviceKey);
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            serviceKey = genericResponse.InstanceId;
            Service? savedService = await _dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);

            Assert.NotNull(savedService);

            var serviceslist = await _dbContext.Service.ToListAsync();
            // test for 2 service status values
            Service newVersionServiceTest = UnitTestHelper.CreateService(1, serviceName, providerProfileId, serviceStatus, null, null, null, savedService.ServiceKey); // to modify saved service

            newVersionServiceTest.ServiceRoleMapping =  [ new() { RoleId = 1}];
            GenericResponse newGenericResponse = await cabRepository.SaveService(newVersionServiceTest, "test.user123@test.com");

           Service? newSavedService = await _dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
          .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
          .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
          .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
          .FirstOrDefaultAsync(p => p.Id == newGenericResponse.InstanceId);


            Assert.True(newGenericResponse.Success);
            Assert.NotNull(newSavedService);
            Assert.Equal(serviceName, newSavedService.ServiceName);           
            Assert.Equal(genericResponse.InstanceId, newSavedService.ServiceKey); // id of first saved service and current service key will be same         
            Assert.NotNull(newSavedService.ServiceRoleMapping);
            Assert.Equal(0, newSavedService?.ServiceQualityLevelMapping?.Count);
            Assert.Equal(0, newSavedService?.ServiceIdentityProfileMapping?.Count);
            Assert.Equal(0,newSavedService?.ServiceSupSchemeMapping?.Count);
        }


        #endregion

        #region Private methods
        private async Task<int> SaveProviderProfileHelper(string registeredName, string loggedInUserId, int cabUserId)
        {
            ProviderProfile providerProfile2 = UnitTestHelper.CreateProviderProfile(cabUserId, registeredName);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile2, loggedInUserId);
            Assert.NotEqual(0, genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
            return genericResponse.InstanceId;
        }
        #endregion
    }
}
