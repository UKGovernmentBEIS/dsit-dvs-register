using AutoMapper;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using NSubstitute;

namespace DVSRegister.UnitTests.Services
{
    public class CabServiceTests
    {

        private readonly ICabRepository cabRepository;
        private readonly IMapper automapper;        
        private readonly CabService cabService;



        public CabServiceTests()
        {
            this.cabRepository = Substitute.For<ICabRepository>();  
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.automapper = config.CreateMapper();            
            this.cabService = new CabService(this.cabRepository, this.automapper);

        }

        [Fact]
        public async Task SaveProviderProfile_ReturnsSuccess()
        {  
            var providerProfileDto = ServiceTestHelper.CreateProviderProfile(1, "Test company");
            var providerProfile = RepositoryTestHelper.CreateProviderProfile(1, "Test company");
            var loggedInUserEmail = "test@example.com";
            var expectedResponse = new GenericResponse {InstanceId = 1 , Success = true };
            cabRepository.SaveProviderProfile(Arg.Any<ProviderProfile>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await cabService.SaveProviderProfile(providerProfileDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await cabRepository.Received(1).SaveProviderProfile(Arg.Is<ProviderProfile>(p =>
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
            cabRepository.SaveProviderProfile(Arg.Any<ProviderProfile>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await cabService.SaveProviderProfile(providerProfileDto, loggedInUserEmail);

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
            cabRepository.SaveService(Arg.Any<Service>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await cabService.SaveService(serviceDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await cabRepository.Received(1).SaveService(Arg.Is<Service>(p =>
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
            cabRepository.SaveService(Arg.Any<Service>(), Arg.Any<string>()).Returns(Task.FromResult(expectedResponse));
            var result = await cabService.SaveService(serviceDto, loggedInUserEmail);

            Assert.Equal(expectedResponse.InstanceId, result.InstanceId);
            Assert.Equal(expectedResponse.Success, result.Success);

            await cabRepository.Received(1).SaveService(Arg.Is<Service>(p =>
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
            bool result = cabService.CanDeleteCertificate(currentFileLink, existingFileLink, existingServiceCabId, cabId);     
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
            bool result = cabService.CanDeleteCertificate(currentFileLink!, existingFileLink!, existingServiceCabId, cabId);         
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
                cabService.CanDeleteCertificate(currentFileLink, existingFileLink, existingServiceCabId, cabId));
            Assert.Equal("Invalid CabId, Cab Id in Service  1, Cab Id in Session 2", exception.Message);
        }

        [Fact]
        public void CheckCompanyInfoEditable_NoServices_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto { Services = null };
            var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_EmptyServicesList_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto { Services = new List<ServiceDto>() };
            var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
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
            var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        [Fact]
        public void CheckCompanyInfoEditable_ServicesNotApproved_ReturnsTrue()
        {
            var providerProfileDto = new ProviderProfileDto
            {
                Services =
            [
              //  new() { CertificateReview = new CertificateReviewDto { CertificateReviewStatus = CertificateReviewEnum.InReview }, ServiceStatus = ServiceStatusEnum.Submitted },
                //new() { CertificateReview = new CertificateReviewDto { CertificateReviewStatus = CertificateReviewEnum.Rejected }, ServiceStatus = ServiceStatusEnum.Submitted }
            ]
            };
            var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
            Assert.True(result);
        }

        //[Fact]
        //public void CheckCompanyInfoEditable_ServicesWithApproved_ReturnsFalse()
        //{
        //    var providerProfileDto = new ProviderProfileDto
        //    {
        //        Services =
        //    [
        //        new() { CertificateReview = new CertificateReviewDto { CertificateReviewStatus = CertificateReviewEnum.Approved }, ServiceStatus = ServiceStatusEnum.Submitted }
        //    ]
        //    };
        //    var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
        //    Assert.False(result);
        //}

        //[Fact]
        //public void CheckCompanyInfoEditable_MixedServices_ReturnsFalse()
        //{
        //    var providerProfileDto = new ProviderProfileDto
        //    {
        //        Services =
        //    [
        //        new () { CertificateReview = new CertificateReviewDto { CertificateReviewStatus = CertificateReviewEnum.Approved }, ServiceStatus = ServiceStatusEnum.Submitted },
        //      // new () { CertificateReview = new CertificateReviewDto { CertificateReviewStatus = CertificateReviewEnum.InReview }, ServiceStatus = ServiceStatusEnum.Submitted }
        //    ]
        //    };
        //    var result = cabService.CheckCompanyInfoEditable(providerProfileDto);
        //    Assert.False(result);
        //}


    }
}
