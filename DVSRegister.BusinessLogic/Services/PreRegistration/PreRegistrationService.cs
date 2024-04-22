using AutoMapper;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.PreRegistration;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace DVSRegister.BusinessLogic.Services.PreAssessment
{
    public class PreRegistrationService : IPreRegistrationService
    {
        private readonly IPreRegistrationRepository preRegistrationRepository;
        private readonly IMapper automapper;
        private readonly IEmailSender emailSender;
        private readonly ILogger<PreRegistrationService> logger;
        private readonly IURNService URNService;

        public PreRegistrationService(IPreRegistrationRepository preRegistrationRepository, IMapper automapper, IEmailSender emailSender, 
         ILogger<PreRegistrationService> logger, IURNService URNService)
        {
            this.preRegistrationRepository = preRegistrationRepository;
            this.automapper = automapper;
            this.emailSender = emailSender;
            this.logger = logger;
            this.URNService = URNService;
        }
        public async Task<List<CountryDto>> GetCountries()
        {
            var countryList = await preRegistrationRepository.GetCountries();
            return automapper.Map<List<CountryDto>>(countryList);
        }

        public async Task<GenericResponse> SavePreRegistration(PreRegistrationDto preRegistrationDto)
        {
            try
            {
                DVSRegister.Data.Entities.PreRegistration preRegistration = new DVSRegister.Data.Entities.PreRegistration();

                URNDto UniqueReferenceNumberDTO = URNService.GenerateURN(preRegistrationDto);
                DVSRegister.Data.Entities.UniqueReferenceNumber URN = new DVSRegister.Data.Entities.UniqueReferenceNumber() { URN = UniqueReferenceNumberDTO.URN};
                preRegistrationDto.URN = UniqueReferenceNumberDTO.URN;

                automapper.Map(preRegistrationDto, preRegistration);
                automapper.Map(UniqueReferenceNumberDTO, URN);

                GenericResponse genericResponse = await preRegistrationRepository.SavePreRegistration(preRegistration);
                GenericResponse URNSaveResponse = await preRegistrationRepository.SaveURN(URN);
                

                genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.Email, preRegistrationDto.FullName);
                if (!string.IsNullOrEmpty(preRegistrationDto.SponsorEmail))
                {
                    genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.SponsorEmail, preRegistrationDto.SponsorFullName);
                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse { Success = false};
            }
        }
    }
}
