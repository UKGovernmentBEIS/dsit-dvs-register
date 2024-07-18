using AutoMapper;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.PreRegistration;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
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
                URNDto UniqueReferenceNumberDTO = URNService.GenerateURN(preRegistrationDto);
                preRegistrationDto.URN = UniqueReferenceNumberDTO.URN;

                DVSRegister.Data.Entities.PreRegistration preRegistration = new DVSRegister.Data.Entities.PreRegistration();
                automapper.Map(preRegistrationDto, preRegistration);
                GenericResponse genericResponse = await preRegistrationRepository.SavePreRegistration(preRegistration);



                DVSRegister.Data.Entities.UniqueReferenceNumber URN = new DVSRegister.Data.Entities.UniqueReferenceNumber()
                {
                    URN = UniqueReferenceNumberDTO.URN,
                    URNStatus = URNStatusEnum.Created,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    PreRegistrationId = genericResponse.InstanceId
                };
                GenericResponse URNSaveResponse = await preRegistrationRepository.SaveURN(URN);


                genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.Email, preRegistrationDto.FullName);
                if (!string.IsNullOrEmpty(preRegistrationDto.SponsorEmail))
                {
                    genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.SponsorEmail, preRegistrationDto.SponsorFullName);
                }
                //Sent email to Ofdia common mail box
                DateTime expirationdate = Convert.ToDateTime(preRegistrationDto.CreatedDate).AddDays(Constants.DaysLeftToComplete);
                genericResponse.EmailSent = await emailSender.SendEmailConfirmationToOfdia(expirationdate.ToLocalTime().ToString("d MMM yyyy h:mm tt"));
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
