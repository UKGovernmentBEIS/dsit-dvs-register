using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CAB
{
    public interface ICabRepository
    { 
        public Task<List<Role>> GetRoles();
        public Task<List<IdentityProfile>> GetIdentityProfiles();
        public Task<List<SupplementaryScheme>> GetSupplementarySchemes();
        public Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile);
        public Task<GenericResponse> UpdateCompanyInfo(ProviderProfile providerProfile);
        public Task<GenericResponse> UpdatePrimaryContact(ProviderProfile providerProfile);
        public Task<GenericResponse> UpdateSecondaryContact(ProviderProfile providerProfile);
        public Task<GenericResponse> UpdatePublicProviderInformation(ProviderProfile providerProfile);
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName);
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId);
        public Task<GenericResponse> SaveService(Service service);
        public Task<List<ProviderProfile>> GetProviders(int cabId,string searchText = "");
        public Task<ProviderProfile> GetProvider(int providerId, int cabId);
        public Task<Service> GetServiceDetails(int serviceId, int cabId);
        public Task<List<QualityLevel>> QualityLevels();
        public Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId);
    }
}
