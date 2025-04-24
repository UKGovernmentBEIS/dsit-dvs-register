using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IConsentService
    {
        //opening loop
        public Task<bool> RemoveConsentToken(string token, string tokenId, string loggedInUserEmail);
        public Task<ServiceDto?> GetProviderAndCertificateDetailsByToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string companyName, string serviceName);


        //closing loop
        public Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedInUserEmail);
        public Task<ServiceDto?> GetProviderAndCertificateDetailsByConsentToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceAndProviderStatus(string token, string tokenId, ServiceDto serviceDto, string loggedInUserEmail);

        public Task<(TokenStatusEnum, TokenStatusEnum)> GetTokenStatus(TokenDetails tokenDetails);



    }
}
