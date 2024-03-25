using AutoMapper;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
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

        public PreRegistrationService(IPreRegistrationRepository preRegistrationRepository, IMapper automapper, IEmailSender emailSender)
        {
            this.preRegistrationRepository = preRegistrationRepository;
            this.automapper = automapper;
            this.emailSender = emailSender;
        }
        public async Task<List<CountryDto>> GetCountries()
        {
            try
            {
                var countryList = await preRegistrationRepository.GetCountries();
                return automapper.Map<List<CountryDto>>(countryList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<GenericResponse> SavePreRegistration(PreRegistrationDto preRegistrationDto)
        {
            try
            {
                 PreRegistration preRegistration = new PreRegistration();
                automapper.Map(preRegistrationDto, preRegistration);                
                GenericResponse genericResponse = await preRegistrationRepository.SavePreRegistration(preRegistration);
                genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.Email, preRegistrationDto.FullName);
                if (!string.IsNullOrEmpty(preRegistrationDto.SponsorEmail))
                {
                    genericResponse.EmailSent = await emailSender.SendEmailConfirmation(preRegistrationDto.SponsorEmail, preRegistrationDto.SponsorFullName);
                }
                //ToDo: Correct Error Messages
                genericResponse.Message  = genericResponse.EmailSent && genericResponse.Success ? "Success" : "Failed";
                return genericResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse { Success = false, Message = "Some error occured" };
            }
        }
    }
}
