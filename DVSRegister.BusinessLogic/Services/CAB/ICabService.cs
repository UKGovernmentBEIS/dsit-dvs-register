using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<List<RoleDto>> GetRoles();
        public Task<List<IdentityProfileDto>> GetIdentityProfiles();
        public Task<List<SupplementarySchemeDto>> GetSupplementarySchemes();
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId = 0);
        public Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfile);
        public Task<GenericResponse> UpdateProviderProfile(ProviderProfileDto providerProfileDto);
        public Task<GenericResponse> SaveService(ServiceDto serviceDto);
        public Task<List<ProviderProfileDto>> GetProviders(int cabId, string searchText = "");
        public Task<ProviderProfileDto> GetProvider(int providerId, int cabId);
        public Task<ServiceDto> GetServiceDetails(int serviceId, int cabId);
        public Task<List<QualityLevelDto>> GetQualitylevels();
        public Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId);
    }
}
