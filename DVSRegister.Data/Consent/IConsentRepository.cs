using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IConsentRepository
    {
        #region opening the loop
        public Task<ProceedApplicationConsentToken> GetProceedApplicationConsentToken(string token, string tokenId);
        public Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedInUserEmail);    
        public Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string agree);
        #endregion


        public Task<Service> GetServiceDetails(int serviceId);
        public Task<List<Service>> GetServiceList(int providerId);

        public Task<Service> GetService(int serviceId);
    }
}
