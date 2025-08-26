using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
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


        public async Task<TokenStatusEnum> GetTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum openingLoopStatus = TokenStatusEnum.NA;
           

            if (tokenDetails.ServiceIds!= null && tokenDetails.ServiceIds.Count == 1)
            {
                var service = await consentRepository.GetService(tokenDetails.ServiceIds[0]);

                if (service.Id>0)
                {
                    openingLoopStatus = service.OpeningLoopTokenStatus;
                   
                }
            }
           
            return (openingLoopStatus);
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
        public async Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string companyName,string serviceName, string agree)
        {
            GenericResponse genericResponse = await consentRepository.UpdateServiceStatus(serviceId, providerEmail, agree);
            if(genericResponse.Success) 
            {
                if (agree == "accept")
                {
                    await emailSender.SendAgreementToProceedApplicationToDSIT(companyName, serviceName);
                    foreach (var email in providerEmail.Split(';'))
                    {
                        await emailSender.SendConfirmationToProceedApplicationToDIP(serviceName, email);
                    }
                }
                else
                {
                    await emailSender.SendDeclineToProceedApplicationToDSIT(companyName, serviceName);
                    foreach (var email in providerEmail.Split(';'))
                    {
                        await emailSender.SendConfirmationOfDeclineToProceedApplicationToDIP(serviceName, email);
                    }
                }
                
            }
            return genericResponse;
        }


       

       

    }
}
