using AutoMapper;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.PreAssessment;
using DVSRegister.BusinessLogic.Services.PreRegistration;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace DVSRegister.UnitTests
{
    public class PreAssessmentServiceTests
    {
        public class PreRegistrationServiceTest
        {
            [Fact]
            public async Task PreRegistrationService_GetCountries()
            {
                var mockRepo = new Mock<IPreRegistrationRepository>();
                var mockMapper = new Mock<IMapper>();
                var mockLogger = new Mock<ILogger<PreRegistrationService>>();
                var mockURNService = new Mock<URNService>();
                List<Country> countries = new List<Country> { new Country { Id = 1, CountryName = "United Kingdom" } };
                mockRepo.Setup(repo => repo.GetCountries()).ReturnsAsync(countries);
                mockMapper.Setup(m => m.Map<List<CountryDto>>(countries)).Returns(new List<CountryDto> { new CountryDto { Id = 1, CountryName = "United Kingdom" } });
                var service = new PreRegistrationService(mockRepo.Object, mockMapper.Object, null,mockLogger.Object, null);
                var result = await service.GetCountries();
                Assert.IsType<List<CountryDto>>(result);
                mockRepo.Verify(repo => repo.GetCountries(), Times.Once);
            }

            [Fact]
            public async Task PreRegistrationService_SavePreRegistration()
            {

                var mockRepo = new Mock<IPreRegistrationRepository>();
                var mockMapper = new Mock<IMapper>();
                var mockEmailSender = new Mock<IEmailSender>();
                var mockLogger = new Mock<ILogger<PreRegistrationService>>();
                var mockURNService = new Mock<URNService>();
                var dto = new PreRegistrationDto();
                mockRepo.Setup(repo => repo.SavePreRegistration(It.IsAny<PreRegistration>())).ReturnsAsync(new GenericResponse { Success = true });
                var service = new PreRegistrationService(mockRepo.Object, mockMapper.Object, mockEmailSender.Object, mockLogger.Object, null);
                var result = await service.SavePreRegistration(dto);
                Assert.True(result.Success);
                mockRepo.Verify(repo => repo.SavePreRegistration(It.IsAny<PreRegistration>()), Times.Once);
            }
        }

    }
}
