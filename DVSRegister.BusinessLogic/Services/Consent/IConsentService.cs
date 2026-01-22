using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IConsentService
    {
        //opening loop
        public Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedInUserEmail);
        public Task<ServiceDto?> GetProviderAndCertificateDetailsByOpeningLoopToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string companyName, string serviceName, string agree);
        public Task<TokenStatusEnum> GetTokenStatus(TokenDetails tokenDetails);
        public Task<ServiceDto> GetService(int serviceId);



    }
}
