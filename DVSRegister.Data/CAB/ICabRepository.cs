using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CAB
{
    public interface ICabRepository
    { 
        public Task<List<Role>> GetRoles();
        public Task<List<IdentityProfile>> GetIdentityProfiles();
        public Task<List<SupplementaryScheme>> GetSupplementarySchemes();
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName);
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId);       
        public Task<List<ProviderProfile>> GetProviders(int cabId,string searchText = "");
        public Task<ProviderProfile> GetProvider(int providerId, int cabId);
        public Task<Service> GetServiceDetails(int serviceId, int cabId);
        public Task<List<QualityLevel>> QualityLevels();
        public Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId);
        public Task<Service> GetServiceDetailsWithProvider(int serviceId, int cabId);


        #region Save/update
        public Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> SaveService(Service service, string loggedInUserEmail);
        public Task<GenericResponse> UpdateCompanyInfo(ProviderProfile providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> UpdatePrimaryContact(ProviderProfile providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> UpdateSecondaryContact(ProviderProfile providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> UpdatePublicProviderInformation(ProviderProfile providerProfile, string loggedInUserEmail);
        #endregion
    }
}
