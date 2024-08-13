using AutoMapper;
using DVSRegister.BusinessLogic.Extensions;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public class CabService : ICabService
    {
        private readonly ICabRepository cabRepository;
        private readonly IMapper automapper;
        private readonly IEmailSender emailSender;

        public CabService(ICabRepository cabRepository, IMapper automapper, IEmailSender emailSender)
        {
            this.cabRepository = cabRepository;
            this.automapper = automapper;
            this.emailSender = emailSender;
        }
        public async Task<PreRegistrationDto> GetPreRegistrationDetails(string URN)
        {
            var preRegistration = await cabRepository.GetPreRegistrationDetails(URN);
            PreRegistrationDto preRegistrationDto = new PreRegistrationDto();
            automapper.Map(preRegistration, preRegistrationDto);
            return preRegistrationDto;
        }
        public async Task<List<RoleDto>> GetRoles()
        {
            var list = await cabRepository.GetRoles();
            return automapper.Map<List<RoleDto>>(list);
        }

        public async Task<List<SupplementarySchemeDto>> GetSupplementarySchemes()
        {
            var list = await cabRepository.GetSupplementarySchemes();
            return automapper.Map<List<SupplementarySchemeDto>>(list);
        }
        public async Task<List<IdentityProfileDto>> GetIdentityProfiles()
        {
            var list = await cabRepository.GetIdentityProfiles();
            return automapper.Map<List<IdentityProfileDto>>(list);
        }



        public async Task<GenericResponse> SaveCertificateInformation(ProviderDto providerDto)
        {
            Provider provider = new Provider();
            automapper.Map(providerDto, provider);
            GenericResponse genericResponse = await cabRepository.SaveCertificateInformation(provider);
            genericResponse.EmailSent = await emailSender.SendCertificateInfoSubmittedToDSIT();
            return genericResponse;
        }

        public async Task<bool> ValidateURN(string URN, string cabUserId)
        {
            bool valid = false;
            UniqueReferenceNumber uniqueReferenceNumber = await cabRepository.GetURNDetails(URN);
            //URN is valid only if the status is Approved(Approved - CAB Validation pending) and not exceed 60 days
            //after application approved


            if (uniqueReferenceNumber != null)
            {
                if (uniqueReferenceNumber.URNStatus == URNStatusEnum.Approved)
                {
                    if (!ExpiredDateValidator.CheckExpired(uniqueReferenceNumber.ModifiedDate))
                    {
                        valid = true;
                        // if valid change status to verified by cab
                        uniqueReferenceNumber.URNStatus = URNStatusEnum.ValidatedByCAB;
                        uniqueReferenceNumber.CheckedByCAB = cabUserId;
                        uniqueReferenceNumber.ModifiedBy = cabUserId;
                        uniqueReferenceNumber.ModifiedDate = DateTime.UtcNow;
                        await cabRepository.UpdateURNStatus(uniqueReferenceNumber);
                    }
                    else
                    {
                        valid = false;
                        uniqueReferenceNumber.URNStatus = URNStatusEnum.Expired;
                        uniqueReferenceNumber.ModifiedBy = cabUserId;
                        await cabRepository.UpdateURNStatus(uniqueReferenceNumber);
                    }
                }               
            }
            return valid;
        }

        public async Task<bool> CheckURNValidatedByCab(string URN)
        {
            bool valid = false;
            UniqueReferenceNumber uniqueReferenceNumber = await cabRepository.GetURNDetails(URN);
            //URN is valid only if the status is Validated by cab           


            if (uniqueReferenceNumber != null)
            {
                if (uniqueReferenceNumber.URNStatus == URNStatusEnum.ValidatedByCAB)
                {
                    valid = true;
                }
            }
            return valid;
        }

        public async Task<DVSRegister.Data.Entities.PreRegistration> GetURNDetails(string URN)
        {
            DVSRegister.Data.Entities.PreRegistration preRegistration = await cabRepository.GetPreRegistrationDetails(URN);

            return preRegistration;
        }

        public async Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfileDto)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile);
            return genericResponse;
        }

        public async Task<GenericResponse> SaveService(ServiceDto serviceDto)
        {
            Service service = new Service();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveService(service);
            return genericResponse;
        }

        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName)
        {
            return await cabRepository.CheckProviderRegisteredNameExists(registeredName);
        }

        public async Task<List<ProviderProfileDto>> GetProviders(string searchText = "")
        {
            var list = await cabRepository.GetProviders(searchText);
            List<ProviderProfileDto> providerDtos = automapper.Map<List<ProviderProfileDto>>(list);
            return providerDtos;
        }

        public async Task<ProviderProfileDto> GetProvider(int providerId, int cabUserId)
        {
            var provider = await cabRepository.GetProvider(providerId, cabUserId);
            ProviderProfileDto providerDto = automapper.Map<ProviderProfileDto>(provider);
            return providerDto;
        }

    }
}
