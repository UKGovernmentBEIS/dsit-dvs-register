using AutoMapper;
using AutoMapper.Internal;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using NSubstitute;

namespace DVSRegister.UnitTests.Services
{
    public class CabServiceTests
    {

        private readonly ICabRepository _cabRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly CabService _cabService;



        public CabServiceTests()
        {
            _cabRepository = Substitute.For<ICabRepository>();
            _commonRepository = Substitute.For<ICommonRepository>();

            var config = new MapperConfiguration(cfg =>
            {              
                cfg.AddProfile(new AutoMapperProfile());                
                cfg.Internal().ForAllMaps((typeMap, _) =>
                {
                    if (typeMap.MaxDepth == 0)
                    {
                        typeMap.MaxDepth = 64;
                    }
                });

            });

            var automapper1 = config.CreateMapper();

            _cabService = new CabService(_cabRepository, _commonRepository, automapper1);

        }

        [Fact]
        public async Task SaveProviderProfile_ReturnsSuccess()
        {  
            var providerProfileDto = ServiceTestHelper.CreateProviderProfile(1, "Test company");
            var providerProfile = RepositoryTestHelper.CreateProviderProfile(1, "Test company");
            var loggedInUserEmail = "test@example.com";
            var expectedResponse = new GenericResponse {InstanceId = 1 , Success = true };
            _cabRepository.SaveProviderProfile(Arg.Any<ProviderProfile>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await _cabService.SaveProviderProfile(providerProfileDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await _cabRepository.Received(1).SaveProviderProfile(Arg.Is<ProviderProfile>(p =>
            p.Id == providerProfile.Id &&
            p.RegisteredName == providerProfile.RegisteredName),
            Arg.Is<string>(email => email == loggedInUserEmail));
        }

        [Fact]
        public async Task SaveProviderProfile_ReturnsFailure()
        {
            var providerProfileDto = ServiceTestHelper.CreateProviderProfile(1, "Test company");           
            var loggedInUserEmail = "test@example.com";
            var expectedResponse = new GenericResponse { InstanceId = 0, Success = false };
            _cabRepository.SaveProviderProfile(Arg.Any<ProviderProfile>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await _cabService.SaveProviderProfile(providerProfileDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

           
        }

        [Fact]
        public async Task SaveService_ReturnsSuccess()
        {
            var serviceDto = ServiceTestHelper.CreateService(1, "Test service", 1,ServiceStatusEnum.Submitted,true,true,true,0);
            var service = ServiceTestHelper.CreateService(1, "Test service", 1, ServiceStatusEnum.Submitted, true, true, true, 0);

            var loggedInUserEmail = "test@example.com";
            var expectedResponse = new GenericResponse { InstanceId = 1, Success = true };
            _cabRepository.SaveService(Arg.Any<Service>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await _cabService.SaveService(serviceDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await _cabRepository.Received(1).SaveService(Arg.Is<Service>(p =>
            p.Id == service.Id &&
            p.ServiceName == service.ServiceName),
            Arg.Is<string>(email => email == loggedInUserEmail));
        }

        [Fact]
        public async Task SaveService_ReturnsFailure()
        {
            var serviceDto = ServiceTestHelper.CreateService(1, "Test service", 1, ServiceStatusEnum.Submitted, true, true, true, 0);
            var service = ServiceTestHelper.CreateService(1, "Test service", 1, ServiceStatusEnum.Submitted, true, true, true, 0);

            var loggedInUserEmail = "test@example.com";
            var expectedResponse = new GenericResponse { InstanceId = 1, Success = false };
            _cabRepository.SaveService(Arg.Any<Service>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await _cabService.SaveService(serviceDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await _cabRepository.Received(1).SaveService(Arg.Is<Service>(p =>
            p.Id == service.Id &&
            p.ServiceName == service.ServiceName),
            Arg.Is<string>(email => email == loggedInUserEmail));
        }

        [Fact]
        public void CanDeleteCertificate_ValidInputs_ReturnsTrue()
        {
        
            string currentFileLink = "currentFile.pdf";
            string existingFileLink = "existingFile.pdf";
            int existingServiceCabId = 1;
            int cabId = 1;
            bool result = _cabService.CanDeleteCertificate(currentFileLink, existingFileLink, existingServiceCabId, cabId);     
            Assert.True(result);
        }

       

        [Theory]
        [InlineData("currentFile.pdf", "", false)]
        [InlineData("currentFile.pdf", null, false)]
        [InlineData("", "existingFile.pdf", false)]
        [InlineData(null, "existingFile.pdf", false)]
        [InlineData("existingFile.pdf", "existingFile.pdf", false)]
        public void CanDeleteCertificate_InvalidFileLinks_ReturnsFalse(string? currentFileLink, string? existingFileLink, bool expected)
        {        
          
            int existingServiceCabId = 1;
            int cabId = 1;          
            bool result = _cabService.CanDeleteCertificate(currentFileLink!, existingFileLink!, existingServiceCabId, cabId);         
            Assert.Equal( result,expected);
        }

        [Fact]
        public void CanDeleteCertificate_CabIdMismatch_ThrowsInvalidOperationException()
        {
          
            string currentFileLink = "currentFile.pdf";
            string existingFileLink = "existingFile.pdf";
            int existingServiceCabId = 1;
            int cabId = 2;           
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _cabService.CanDeleteCertificate(currentFileLink, existingFileLink, existingServiceCabId, cabId));
            Assert.Equal("Invalid CabId, Cab Id in Service  1, Cab Id in Session 2", exception.Message);
        }

        [Fact]
        public void CheckCompanyInfoEditable_NoServices_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto { Services = null };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_EmptyServicesList_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto { Services = new List<ServiceDto>() };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_ServicesSubmittedNoReview_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto
            {
             Services =
            [
                new (){ CertificateReview = null!, ServiceStatus = ServiceStatusEnum.Submitted }
            ]
            };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_AllSubmittedWithReviewNonApproved_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto
            {
                Services =
                [
                    new()
                    {
                        ServiceStatus = ServiceStatusEnum.Submitted,
                        CertificateReview =
                        [
                            new() { IsLatestReviewVersion = true, CertificateReviewStatus = CertificateReviewEnum.Rejected }
                        ]
                    }
                ]
            };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_AnyServiceApproved_ReturnsFalse()
        {
            var providerProfileDto = new ProviderProfileDto
            {
                Services =
                [
                    new()
                    {
                        ServiceStatus = ServiceStatusEnum.Submitted,
                        CertificateReview =
                        [
                            new() { IsLatestReviewVersion = true, CertificateReviewStatus = CertificateReviewEnum.Approved }
                        ]
                    }
                ]
            };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.False(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_MixedWithOneApproved_ReturnsFalse()
        {
            var providerProfileDto = new ProviderProfileDto
            {
                Services =
                [
                    new()
                    {
                        ServiceStatus = ServiceStatusEnum.Submitted,
                        CertificateReview =
                        [
                            new() { IsLatestReviewVersion = true, CertificateReviewStatus = CertificateReviewEnum.Approved }
                        ]
                    },
                    new()
                    {
                        ServiceStatus = ServiceStatusEnum.Submitted,
                        CertificateReview =
                        [
                            new() { IsLatestReviewVersion = true, CertificateReviewStatus = CertificateReviewEnum.Rejected }
                        ]
                    }
                ]
            };
            var result = _cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.False(result);
        }

        [Fact]
        public async Task CheckProviderRegisteredNameExists_WithProviderId_ReturnsRepoResult()
        {
            var name = "Test Provider";
            var id = 5;
            _cabRepository.CheckProviderRegisteredNameExists(name, id)
                .Returns(Task.FromResult(true));

            var result = await _cabService.CheckProviderRegisteredNameExists(name, id);
            Assert.True(result);
        }

        [Fact]
        public async Task CheckProviderRegisteredNameExists_WithoutProviderId_ReturnsRepoResult()
        {
            var name = "Test Provider";
            _cabRepository.CheckProviderRegisteredNameExists(name)
                .Returns(Task.FromResult(false));

            var result = await _cabService.CheckProviderRegisteredNameExists(name);
            Assert.False(result);
        }

        [Fact]
        public async Task GetRoles_ValidVersion_ReturnsMappedList()
        {
            var version = 1m;
            var roles = new List<Role> { new() { Id = 1 } };
            _cabRepository.GetRoles(version).Returns(Task.FromResult(roles));

            var result = await _cabService.GetRoles(version);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetRoles_NoData_ReturnsEmptyList()
        {
            var version = 1m;
            _cabRepository.GetRoles(version).Returns(Task.FromResult(new List<Role>()));

            var result = await _cabService.GetRoles(version);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSupplementarySchemes_ReturnsMappedList()
        {
            var schemes = new List<SupplementaryScheme> { new() { Id = 1 } };
            _cabRepository.GetSupplementarySchemes().Returns(Task.FromResult(schemes));

            var result = await _cabService.GetSupplementarySchemes();
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetSupplementarySchemes_NoData_ReturnsEmptyList()
        {
            _cabRepository.GetSupplementarySchemes().Returns(Task.FromResult(new List<SupplementaryScheme>()));

            var result = await _cabService.GetSupplementarySchemes();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIdentityProfiles_WithVersion_ReturnsMappedList()
        {
            var version = 1m;
            var profiles = new List<IdentityProfile> { new() { Id = 1 } };
            _cabRepository.GetIdentityProfiles(version).Returns(Task.FromResult(profiles));

            var result = await _cabService.GetIdentityProfiles(version);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetIdentityProfiles_WithoutVersion_ReturnsAllProfiles()
        {
            var profiles = new List<IdentityProfile> { new() { Id = 1 } };
            _cabRepository.GetIdentityProfiles().Returns(Task.FromResult(profiles));

            var result = await _cabService.GetIdentityProfiles();
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetTfVersion_ReturnsMappedList()
        {
            var versions = new List<TrustFrameworkVersion> { new() { Id = 1 } };
            _commonRepository.GetActiveTfVersion().Returns(Task.FromResult(versions));

            var result = await _commonRepository.GetActiveTfVersion();
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetProviders_NoSearchText_ReturnsMappedList()
        {
            var cabId = 1;
            var providers = new List<ProviderProfile> { new() { Id = 1, RegisteredName = "Test" } };
            _cabRepository.GetProviders(cabId).Returns(Task.FromResult(providers));

            var result = await _cabService.GetProviders(cabId);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetProviders_WithSearchText_PassesToRepository()
        {
            var cabId = 1;
            var text = "ABC";
            _cabRepository.GetProviders(cabId, text).Returns(Task.FromResult(new List<ProviderProfile>()));

            var result = await _cabService.GetProviders(cabId, text);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProvider_Valid_ReturnsMappedProvider()
        {
            var providerId = 1;
            var cabId = 1;
            var provider = new ProviderProfile { Id = providerId, RegisteredName = "Test" };
            _cabRepository.GetProvider(providerId, cabId).Returns(Task.FromResult(provider));

            var result = await _cabService.GetProvider(providerId, cabId);
            Assert.NotNull(result);
            Assert.Equal(providerId, result.Id);
        }

        [Fact]
        public async Task GetServiceDetails_Valid_ReturnsMappedService()
        {
            var serviceId = 1;
            var cabId = 1;
            var service = new Service { Id = serviceId, ServiceName = "Test Service" };
            _cabRepository.GetServiceDetails(serviceId, cabId).Returns(Task.FromResult(service));

            var result = await _cabService.GetServiceDetails(serviceId, cabId);
            Assert.NotNull(result);
            Assert.Equal(serviceId, result.Id);
        }

        [Fact]
        public async Task GetServiceList_Valid_ReturnsMappedList()
        {
            var serviceKey = 10;
            var cabId = 1;
            var services = new List<Service> { new() { ServiceKey = serviceKey } };
            _cabRepository.GetServiceList(serviceKey, cabId).Returns(Task.FromResult(services));

            var result = await _cabService.GetServiceList(serviceKey, cabId);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task IsManualServiceLinkedToMultipleServices_True_ReturnsTrue()
        {
            var id = 1;
            _cabRepository.IsManualServiceLinkedToMultipleServices(id).Returns(Task.FromResult(true));

            var result = await _cabService.IsManualServiceLinkedToMultipleServices(id);
            Assert.True(result);
        }

        [Fact]
        public async Task IsManualServiceLinkedToMultipleServices_False_ReturnsFalse()
        {
            var id = 2;
            _cabRepository.IsManualServiceLinkedToMultipleServices(id).Returns(Task.FromResult(false));

            var result = await _cabService.IsManualServiceLinkedToMultipleServices(id);
            Assert.False(result);
        }

        [Fact]
        public async Task GetServiceDetailsWithProvider_Valid_ReturnsMappedService()
        {
            var serviceId = 1;
            var cabId = 1;
            var service = new Service { Id = serviceId, ServiceName = "Test" };
            _cabRepository.GetServiceDetailsWithProvider(serviceId, cabId).Returns(Task.FromResult(service));

            var result = await _cabService.GetServiceDetailsWithProvider(serviceId, cabId);
            Assert.NotNull(result);
            Assert.Equal(serviceId, result.Id);
        }

        [Fact]
        public async Task GetQualitylevels_ReturnsMappedList()
        {
            var levels = new List<QualityLevel> { new() { Id = 1 } };
            _cabRepository.QualityLevels().Returns(Task.FromResult(levels));

            var result = await _cabService.GetQualitylevels();
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetQualitylevels_NoData_ReturnsEmptyList()
        {
            _cabRepository.QualityLevels().Returns(Task.FromResult(new List<QualityLevel>()));

            var result = await _cabService.GetQualitylevels();
            Assert.Empty(result);
        }

        [Fact]
        public async Task CheckValidCabAndProviderProfile_Valid_ReturnsTrue()
        {
            var providerId = 1;
            var cabId = 1;
            _cabRepository.CheckValidCabAndProviderProfile(providerId, cabId).Returns(Task.FromResult(true));

            var result = await _cabService.CheckValidCabAndProviderProfile(providerId, cabId);
            Assert.True(result);
        }

        [Fact]
        public async Task CheckValidCabAndProviderProfile_Invalid_ReturnsFalse()
        {
            var providerId = 99;
            var cabId = 1;
            _cabRepository.CheckValidCabAndProviderProfile(providerId, cabId).Returns(Task.FromResult(false));

            var result = await _cabService.CheckValidCabAndProviderProfile(providerId, cabId);
            Assert.False(result);
        }

        [Fact]
        public async Task GetPendingReassignRequests_WithPending_ReturnsTrueAndList()
        {
            var cabId = 1;
            var count = 3;
            var requests = new List<CabTransferRequest> { new() { Id = 1 } };
            _cabRepository.GetPendingReassignRequestsCount(cabId).Returns(Task.FromResult((count, requests)));

            var (hasPending, list) = await _cabService.GetPendingReassignRequests(cabId);
            Assert.True(hasPending);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task GetPendingReassignRequests_NoPending_ReturnsFalseAndEmptyList()
        {
            var cabId = 1;
            _cabRepository.GetPendingReassignRequestsCount(cabId)
                .Returns(Task.FromResult((0, new List<CabTransferRequest>())));

            var (hasPending, list) = await _cabService.GetPendingReassignRequests(cabId);
            Assert.False(hasPending);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetProviderWithLatestVersionServices_Valid_ReturnsMappedProvider()
        {
            var providerId = 1;
            var cabId = 1;
            var provider = new ProviderProfile { Id = providerId };
            _cabRepository.GetProviderWithLatestVersionServices(providerId, cabId).Returns(Task.FromResult(provider));

            var result = await _cabService.GetProviderWithLatestVersionServices(providerId, cabId);
            Assert.NotNull(result);
            Assert.Equal(providerId, result.Id);
        }

        [Fact]
        public async Task SaveServiceReApplication_Success_ReturnsResponse()
        {
            var dto = ServiceTestHelper.CreateService(1, "ReApp", 1, ServiceStatusEnum.Submitted, true, true, true, 1);
            var email = "test@example.com";
            var expected = new GenericResponse { Success = true, InstanceId = 3 };
            _cabRepository.SaveServiceReApplication(Arg.Any<Service>(), email, true, null)
                .Returns(Task.FromResult(expected));

            var result = await _cabService.SaveServiceReApplication(dto, email, true, null);

            Assert.True(result.Success);
            Assert.Equal(expected.InstanceId, result.InstanceId);
        }

        [Fact]
        public async Task SaveServiceReApplication_Failure_ReturnsFailureResponse()
        {
            var dto = ServiceTestHelper.CreateService(1, "ReApp", 1, ServiceStatusEnum.Submitted, true, true, true, 1);
            var email = "test@example.com";
            var expected = new GenericResponse { Success = false, InstanceId = 0 };
            _cabRepository.SaveServiceReApplication(Arg.Any<Service>(), email, false, null)
                .Returns(Task.FromResult(expected));

            var result = await _cabService.SaveServiceReApplication(dto, email, false, null);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task SaveServiceAmendments_SuccessAndCanDeleteCertificateTrue_UseDeleteBranch()
        {
            var dto = ServiceTestHelper.CreateService(1, "Amend", 1, ServiceStatusEnum.Submitted, true, true, true, 1);
            var existingServiceCabId = 1;
            var cabId = 1;
            var email = "test@example.com";
            var expected = new GenericResponse { Success = true, InstanceId = 4 };

            _cabRepository.SaveServiceAmendments(Arg.Any<Service>(), email)
                .Returns(Task.FromResult(expected));

            var result = await _cabService.SaveServiceAmendments(
                dto, "new.pdf", existingServiceCabId, cabId, email);

            Assert.True(result.Success);
            Assert.Equal(expected.InstanceId, result.InstanceId);
        }

        [Fact]
        public async Task SaveServiceAmendments_SuccessButCannotDelete_NoDeletePath()
        {
            var dto = ServiceTestHelper.CreateService(1, "Amend", 1, ServiceStatusEnum.Submitted, true, true, true, 1);
            var existingLink = "file.pdf";
            var existingServiceCabId = 1;
            var cabId = 1;
            var email = "test@example.com";
            var expected = new GenericResponse { Success = true, InstanceId = 5 };

            _cabRepository.SaveServiceAmendments(Arg.Any<Service>(), email)
                .Returns(Task.FromResult(expected));

            var result = await _cabService.SaveServiceAmendments(
                dto, existingLink, existingServiceCabId, cabId, email);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task SaveServiceAmendments_Failure_ReturnsFailure()
        {
            var dto = ServiceTestHelper.CreateService(1, "Amend", 1, ServiceStatusEnum.Submitted, true, true, true, 1);
            var existingServiceCabId = 1;
            var cabId = 1;
            var email = "test@example.com";
            var expected = new GenericResponse { Success = false, InstanceId = 0 };

            _cabRepository.SaveServiceAmendments(Arg.Any<Service>(), email)
                .Returns(Task.FromResult(expected));

            var result = await _cabService.SaveServiceAmendments(
                dto, "new.pdf",  existingServiceCabId, cabId, email);

            Assert.False(result.Success);
        }

    }
}
