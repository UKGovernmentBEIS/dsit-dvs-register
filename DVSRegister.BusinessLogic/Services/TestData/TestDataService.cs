using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DVSRegister.BusinessLogic.Services.TestData
{
    public class TestDataService : ITestDataService
    {
        private readonly ICabRepository cabRepository;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly ILogger<TestDataService> logger;
        
        public TestDataService(ICabRepository cabRepository, IUserService userService, IMapper mapper, ILogger<TestDataService> logger)
        {

            this.cabRepository = cabRepository;
            this.userService = userService;
            this.mapper = mapper;
            this.logger = logger;
        }



        /// <summary>
        /// Create sample provider
        /// Fetch and seed data from json file in TestData Folder
        /// Sample json can created by adding line of code for serializing the dto in service layer 
        /// </summary>
        /// <param name="loggedInUserEmail"></param>
        /// <param name="allowTestDataCreation"></param>
        /// <param name="providerProfileDto"></param>
        /// <returns></returns>
        public async Task CreateTestProvider(string loggedInUserEmail, bool allowTestDataCreation, ProviderProfileDto providerProfileDto)
        {
            try
            {
                Guard(allowTestDataCreation);
                CabUserDto cabUserDto = await userService.GetUser(loggedInUserEmail);

                ProviderProfile providerProfile = new();
                mapper.Map(providerProfileDto, providerProfile);
                providerProfile.PrimaryContactEmail = loggedInUserEmail;
                providerProfile.SecondaryContactEmail = AddReviewerSuffix(loggedInUserEmail);

                providerProfile.RegisteredName = $"{providerProfile.RegisteredName} {Guid.NewGuid().ToString("N")[..6]}";
                providerProfile.ProviderProfileCabMapping = [];
                providerProfile.ProviderProfileCabMapping.Add(new ProviderProfileCabMapping { CabId = cabUserDto.CabId });
                GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile, loggedInUserEmail);
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateTestProvider");
            }

        }

        /// <summary>
        /// Create sample services
        /// Fetch and seed data from json file in TestData Folder
        /// Sample json can created by adding line of code for serializing the dto in service layer  
        /// For new tf versions, create json files and add it to TestData folder and add new button for it
        /// </summary>
        /// <param name="loggedInUserEmail"></param>
        /// <param name="allowTestDataCreation"></param>
        /// <param name="providerId"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public async Task CreateTestServices(string loggedInUserEmail, bool allowTestDataCreation, int providerId, List<ServiceDto> services)
        {
            try
            {
                Guard(allowTestDataCreation);
                CabUserDto cabUserDto = await userService.GetUser(loggedInUserEmail);

                foreach (ServiceDto serviceDto in services)
                {
                    Service service = new();
                    mapper.Map(serviceDto, service);
                    service.ProviderProfileId = providerId;
                    service.CabUserId = cabUserDto.Id;
                    service.ServiceName = $"{service.ServiceName} {Guid.NewGuid().ToString("N")[..6]}";
                    await cabRepository.SaveService(service, loggedInUserEmail);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateTestServices");
            }

        }

        /// <summary>
        /// Secondary contact email should be added in notify
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>

        public static string AddReviewerSuffix(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            var atIndex = email.IndexOf('@');
            if (atIndex < 0)
                throw new ArgumentException("Invalid email format.", nameof(email));

            return email.Insert(atIndex, "+reviewer");
        }

        /// <summary>
        /// Allow test data only for dev and staging
        /// </summary>
        /// <param name="allowTestDataCreation"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void Guard(bool allowTestDataCreation)
        {
            if (!allowTestDataCreation)
            {
                throw new InvalidOperationException(
                    "Test data creation is disabled in production."
                );
            }
        }



    }
}
