using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DVSRegister.UnitTests.Repository
{
    [Collection("Postgres Collection")]
    public class CabRepositoryTests
    {
        private  readonly ILogger<CabRepository> logger;
        private readonly PostgresTestFixture fixture;

        public CabRepositoryTests(PostgresTestFixture fixture)
        {
            this.fixture = fixture;
            logger = Substitute.For<ILogger<CabRepository>>();
        }


        #region Provider

        [Fact]
        public async Task Save_ProviderProfile_ReturnsSuccess()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfile(1, "ABC Company");
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await dbContext.ProviderProfile.FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
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
        public async Task Save_ProviderProfile_With_DunsNumber_ReturnsSuccess()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfileWithDunsNumber(1, "ABC Company");

            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await dbContext.ProviderProfile.FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
            Assert.NotNull(providerProfile);
            Assert.False(providerProfile.HasRegistrationNumber);
            Assert.Null(providerProfile.CompanyRegistrationNumber);
            Assert.NotNull(providerProfile.DUNSNumber);
            Assert.Equal(providerProfileTest.DUNSNumber, providerProfile.DUNSNumber);

        }


        [Fact]
        public async Task Save_ProviderProfile_With_ParentCompany_ReturnsSuccess()
        {
            ProviderProfile providerProfileTest = UnitTestHelper.CreateProviderProfileWithParentCompany(1, "ABC Company");
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfileTest, "test.user123@test.com");
            ProviderProfile? providerProfile = await dbContext.ProviderProfile.FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
            Assert.NotNull(providerProfile);
            Assert.True(providerProfile.HasParentCompany);
            Assert.NotNull(providerProfile.ParentCompanyRegisteredName);
            Assert.NotNull(providerProfile.ParentCompanyLocation);

            Assert.Equal(providerProfileTest.ParentCompanyRegisteredName, providerProfile.ParentCompanyRegisteredName);
            Assert.Equal(providerProfileTest.ParentCompanyLocation, providerProfile.ParentCompanyLocation);

        }

        [Fact]
        public async Task Save_ProviderProfile_InvalidProfileDetails_ReturnsFailure()
        {
            ProviderProfile providerProfile = new();
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }
        [Fact]
        public async Task SaveProviderProfile_NullProviderProfile_ReturnsFailure()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(null!, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }

        [Fact]
        public async Task Check_RegisteredName_Exists()
        {

            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            await SaveProviderProfileHelper("Test Company", "test.user123@test.com", 1, cabRepository);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company");
            Assert.True(actual);
        }

        [Fact]
        public async Task Check_RegisteredName_NotExists()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            await SaveProviderProfileHelper("Test Company 123", "test.user123@test.com", 1, cabRepository);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company 1234");
            Assert.False(actual);
        }

        [Fact]
        public async Task Check_RegisteredNameExists_SameProvider()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            int providerProfileId = await SaveProviderProfileHelper("Test Company", "test.user123@test.com", 1, cabRepository);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company", providerProfileId);
            Assert.False(actual);
        }

        [Fact]
        public async Task Check_RegisteredNameExists_DifferentProvider()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            await SaveProviderProfileHelper("Test Company 1", "test.user123@test.com", 1, cabRepository);
            int providerProfileId = await SaveProviderProfileHelper("Test Company 2", "test.user123@test.com", 1, cabRepository);
            bool actual = await cabRepository.CheckProviderRegisteredNameExists("Test Company 1", providerProfileId);
            Assert.True(actual);
        }



        #endregion


        #region Service



        [Fact]
        public async Task SaveServiceWithRoles_ReturnsSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1, cabRepository);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service", providerProfileId, ServiceStatusEnum.Submitted, null, null, null, 0);
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            Service? savedService = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role).FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);
            List<Role> roles = await dbContext.Role.ToListAsync();

            Assert.True(genericResponse.Success);
            Assert.NotNull(savedService);
            Assert.Equal(serviceTest.ServiceName, savedService.ServiceName);
            Assert.Equal(1, savedService.ServiceVersion);
            Assert.Equal(genericResponse.InstanceId, savedService.ServiceKey);
            Assert.Null(savedService.HasGPG44);
            Assert.Null(savedService.HasGPG45);
            Assert.Null(savedService.HasSupplementarySchemes);
            Assert.NotNull(savedService.ServiceRoleMapping);
            foreach (var roleMapping in savedService.ServiceRoleMapping)
            {
                Assert.NotNull(roleMapping);
                Assert.NotNull(roleMapping.Role);
                Assert.Equal(roles.Where(r => r.Id == roleMapping.RoleId).Select(r => r.RoleName).FirstOrDefault(), roleMapping.Role.RoleName);
            }

        }

        [Fact]
        public async Task SaveServiceWithGpg44Gpg45Schemes_ReturnsSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1, cabRepository);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service", providerProfileId, ServiceStatusEnum.Submitted, true, true, true, 0);
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            Service? savedService = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);

            List<QualityLevel> qualityLevels = await dbContext.QualityLevel.ToListAsync();
            List<IdentityProfile> identityProfiles = await dbContext.IdentityProfile.ToListAsync();
            List<SupplementaryScheme> supplementarySchemes = await dbContext.SupplementaryScheme.ToListAsync();

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

            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            Service service = new();
            GenericResponse genericResponse = await cabRepository.SaveService(service, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }


        [Theory]
        [InlineData(ServiceStatusEnum.Submitted, "New submitted service")]
        [InlineData(ServiceStatusEnum.SavedAsDraft, "New draft service")]

        // Save a service first as draft, then again update samae service - first as draft then submit 
        public async Task SaveServiceDraft_ReturnsSuccess(ServiceStatusEnum serviceStatus, string serviceName)
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            int serviceKey = 0;
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1, cabRepository);
            Service serviceTest = UnitTestHelper.CreateService(1, "sample service draft 1", providerProfileId, ServiceStatusEnum.SavedAsDraft, true, true, true, serviceKey);
            GenericResponse genericResponse = await cabRepository.SaveService(serviceTest, "test.user123@test.com");
            serviceKey = genericResponse.InstanceId;
            Service? savedService = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);

            Assert.NotNull(savedService);

            var serviceslist = await dbContext.Service.ToListAsync();
            // test for 2 service status values
            Service newServiceTest = UnitTestHelper.CreateService(1, serviceName, providerProfileId, serviceStatus, null, null, null, savedService.ServiceKey); // to modify saved service

            newServiceTest.ServiceRoleMapping = [new() { RoleId = 1 }];
            GenericResponse newGenericResponse = await cabRepository.SaveService(newServiceTest, "test.user123@test.com");

            Service? newSavedService = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
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
            Assert.Equal(0, newSavedService?.ServiceSupSchemeMapping?.Count);
        }




        [Theory]
        [InlineData(ServiceStatusEnum.Submitted, "Version 2 submitted service")]
        [InlineData(ServiceStatusEnum.SavedAsDraft, "Version 2 draft service")]
        public async Task Save_Service_Reapplication_ReturnsSuccess(ServiceStatusEnum serviceStatus, string serviceName)
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            int serviceKey = 0;
            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1, cabRepository);
            Service version1Service = UnitTestHelper.CreateService(1, "sample service version1", providerProfileId, ServiceStatusEnum.Submitted, true, true, true, serviceKey);
            GenericResponse genericResponse = await cabRepository.SaveService(version1Service, "test.user123@test.com");
            serviceKey = genericResponse.InstanceId;
            Service? savedVersion1Service = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == genericResponse.InstanceId);


            Assert.NotNull(savedVersion1Service);
            Assert.Equal(1, savedVersion1Service.ServiceVersion);


            Service newVersionServiceTest = UnitTestHelper.CreateService(1, serviceName, providerProfileId, serviceStatus, null, null, null, savedVersion1Service.ServiceKey); // to modify saved service

            newVersionServiceTest.ServiceRoleMapping = [new() { RoleId = 1 }];
            GenericResponse newGenericResponse = await cabRepository.SaveServiceReApplication(newVersionServiceTest, "test.user123@test.com");

            Service? savedVersion2Service = await dbContext.Service.Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
           .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
           .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
           .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
           .FirstOrDefaultAsync(p => p.ServiceKey == newGenericResponse.InstanceId && p.IsCurrent == true);// fetch the saved new version


            Assert.True(newGenericResponse.Success);
            Assert.NotNull(savedVersion2Service);
            Assert.Equal(2, savedVersion2Service.ServiceVersion);
            Assert.Equal(serviceName, savedVersion2Service.ServiceName);
            Assert.Equal(genericResponse.InstanceId, savedVersion2Service.ServiceKey); // id of first saved service and current service key will be same         
            Assert.NotNull(savedVersion2Service.ServiceRoleMapping);
            Assert.Equal(0, savedVersion2Service?.ServiceQualityLevelMapping?.Count);
            Assert.Equal(0, savedVersion2Service?.ServiceIdentityProfileMapping?.Count);
            Assert.Equal(0, savedVersion2Service?.ServiceSupSchemeMapping?.Count);
        }

        [Fact]
        public async Task SaveServiceReApplication_InvalidServiceDetails_ReturnsFailure()
        {

            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);
            Service service = new();
            GenericResponse genericResponse = await cabRepository.SaveServiceReApplication(service, "test.user123@test.com");
            Assert.False(genericResponse.Success);
        }

        [Fact]
        public async Task Save_Service_Amendments_ReturnsSuccess()
        {
            InitializeDbContext(out DVSRegisterDbContext dbContext);
            CabRepository cabRepository = new(dbContext, logger);

            int providerProfileId = await SaveProviderProfileHelper("company name", "test@test.com", 1, cabRepository);
            Service sampleService = UnitTestHelper.CreateService(1, "sample service 1", providerProfileId, ServiceStatusEnum.AmendmentsRequired, true, true, true, 0);
            GenericResponse genericResponse = await cabRepository.SaveService(sampleService, "test.user123@test.com");

            CertificateReview certificateReview = UnitTestHelper.CreateFailedCertificateReview(1, genericResponse.InstanceId, providerProfileId,
            CertificateReviewEnum.AmendmentsRequired, "test comments", "comments for incorrect", "", "amendments needed comment");
            dbContext.CertificateReview.Add(certificateReview);
            dbContext.SaveChanges();



            Service amendedService = UnitTestHelper.CreateService(1, "sample service 1 amended", providerProfileId, ServiceStatusEnum.AmendmentsRequired, null, null, null, genericResponse.InstanceId);
            amendedService.Id = genericResponse.InstanceId;

            GenericResponse amendmentsResponse = await cabRepository.SaveServiceAmendments(amendedService, "test.user123@test.com");
            Assert.True(amendmentsResponse.Success);

            Service? savedAmendedService = await dbContext.Service.Include(p => p.CertificateReview).Include(p => p.ServiceRoleMapping).ThenInclude(p => p.Role)
            .Include(p => p.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .FirstOrDefaultAsync(p => p.Id == amendmentsResponse.InstanceId);

            Assert.NotNull(savedAmendedService);
            Assert.Null(savedAmendedService.CertificateReview);

            Assert.Equal(amendedService.ServiceVersion, savedAmendedService.ServiceVersion);
            Assert.Equal("sample service 1 amended", savedAmendedService.ServiceName);
            Assert.Equal(amendedService.Id, savedAmendedService.ServiceKey);
            Assert.NotNull(savedAmendedService.ServiceRoleMapping);
            Assert.Equal(0, savedAmendedService?.ServiceQualityLevelMapping?.Count);
            Assert.Equal(0, savedAmendedService?.ServiceIdentityProfileMapping?.Count);
            Assert.Equal(0, savedAmendedService?.ServiceSupSchemeMapping?.Count);
        }

        #endregion

        #region Private methods
        private async Task<int> SaveProviderProfileHelper(string registeredName, string loggedInUserId, int cabUserId, CabRepository cabRepository)
        {
            ProviderProfile providerProfile2 = UnitTestHelper.CreateProviderProfile(cabUserId, registeredName);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile2, loggedInUserId);
            Assert.NotEqual(0, genericResponse.InstanceId);
            Assert.True(genericResponse.Success);
            return genericResponse.InstanceId;
        }

        private void InitializeDbContext(out DVSRegisterDbContext dbContext)
        {
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
          .UseNpgsql(fixture.GetConnectionString())
          .Options;
            dbContext = new DVSRegisterDbContext(options);

        }
        #endregion
    }
}
