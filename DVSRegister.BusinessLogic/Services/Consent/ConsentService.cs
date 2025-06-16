using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;

namespace DVSRegister.BusinessLogic.Services
{
    public class ConsentService : IConsentService
    {

        private readonly IConsentRepository consentRepository;     
        private readonly IMapper mapper;
        private readonly ConsentEmailSender emailSender;


        public ConsentService(IConsentRepository consentRepository, IMapper mapper, ConsentEmailSender emailSender)
        {           
            this.consentRepository = consentRepository;    
            this.mapper = mapper;
            this.emailSender = emailSender;
        }


        public async Task<(TokenStatusEnum, TokenStatusEnum)> GetTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum openingLoopStatus = TokenStatusEnum.NA;
            TokenStatusEnum closingLoopStatus = TokenStatusEnum.NA;

            if (tokenDetails.ServiceIds!= null && tokenDetails.ServiceIds.Count == 1)
            {
                var service = await consentRepository.GetService(tokenDetails.ServiceIds[0]);

                if (service.Id>0)
                {
                    openingLoopStatus = service.OpeningLoopTokenStatus;
                    closingLoopStatus = service.ClosingLoopTokenStatus;
                }
            }
           
            return (openingLoopStatus,closingLoopStatus);
        }

        //opening loop
        public async Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedInUserEmail)
        {
            return await consentRepository.RemoveProceedApplicationConsentToken(token, tokenId, loggedInUserEmail);
        }

        public async Task<ServiceDto?> GetProviderAndCertificateDetailsByOpeningLoopToken(string token, string tokenId)
        {
            ProceedApplicationConsentToken consentToken = await consentRepository.GetProceedApplicationConsentToken(token, tokenId);
            if (consentToken.Service != null)
            {
                var service = await consentRepository.GetServiceDetails(consentToken.ServiceId);
                ServiceDto serviceDto = mapper.Map<ServiceDto>(service);
                return serviceDto;
            }
            else
            {
                return null ;
            }
        }
        public async Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string companyName, string serviceName)
        {
            GenericResponse genericResponse = await consentRepository.UpdateServiceStatus(serviceId, ServiceStatusEnum.Received, providerEmail);
            if(genericResponse.Success) 
            {
                await emailSender.SendAgreementToProceedApplicationToDSIT(companyName, serviceName);
            }
            return genericResponse;
        }


        //closing loop

        public async Task<bool> RemoveProceedPublishConsentToken(string token, string tokenId, string loggedInUserEmail)
        {
            return await consentRepository.RemoveProceedPublishConsentToken(token, tokenId, loggedInUserEmail);
        }

        public async Task<ServiceDto?> GetProviderAndCertificateDetailsByClosingLoopToken(string token, string tokenId)
        {
            ProceedPublishConsentToken consentToken = await consentRepository.GetProceedPublishConsentToken(token, tokenId);
            if (consentToken.Service!=null)
            {
                var service = await consentRepository.GetServiceDetails(consentToken.ServiceId);
                ServiceDto serviceDto = mapper.Map<ServiceDto>(service);
                return serviceDto;
            }
            else
            {
                return null;
            }           
           
        }
        public async Task<GenericResponse> UpdateServiceAndProviderStatus(string token, string tokenId, ServiceDto serviceDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new GenericResponse();
            ProceedPublishConsentToken consentToken = await consentRepository.GetProceedPublishConsentToken(token, tokenId);
            if (!string.IsNullOrEmpty(consentToken.Token) && !string.IsNullOrEmpty(consentToken.TokenId))   //proceed update status if token exists           
            {                
                genericResponse = await consentRepository.UpdateServiceAndProviderStatus(serviceDto.Id, loggedInUserEmail);
                if (genericResponse.Success)
                {

                    genericResponse.Success = await emailSender.SendAgreementToPublishToDIP(serviceDto.Provider.RegisteredName, serviceDto.ServiceName,
                    serviceDto.Provider.PrimaryContactFullName, serviceDto.Provider.PrimaryContactEmail);
                    genericResponse.Success = await emailSender.SendAgreementToPublishToDIP(serviceDto.Provider.RegisteredName, serviceDto.ServiceName,
                    serviceDto.Provider.SecondaryContactFullName, serviceDto.Provider.SecondaryContactEmail);
                    await emailSender.SendAgreementToPublishToDSIT(serviceDto.Provider.RegisteredName, serviceDto.ServiceName);
                }

            }
            return genericResponse;
        }

    }
}
