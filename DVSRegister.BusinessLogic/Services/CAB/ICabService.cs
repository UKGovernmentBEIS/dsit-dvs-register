using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<GenericResponse> SaveCertificateInformation(ProviderDto providerDto);
        public Task<bool> ValidateURN(string URN, string cabUserId);
        public Task<PreRegistrationDto> GetPreRegistrationDetails(string URN);

        public Task<List<RoleDto>> GetRoles();
        public Task<List<IdentityProfileDto>> GetIdentityProfiles();
        public Task<List<SupplementarySchemeDto>> GetSupplementarySchemes();
        public Task<DVSRegister.Data.Entities.PreRegistration> GetURNDetails(string URN);
        public Task<bool> CheckURNValidatedByCab(string URN);
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName);
        public Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfile);
        public Task<GenericResponse> SaveService(ServiceDto serviceDto);
        public Task<List<ProviderProfileDto>> GetProviders(int cabId, string searchText = "");
        public Task<ProviderProfileDto> GetProvider(int providerId, int cabId);
        public Task<ServiceDto> GetServiceDetails(int serviceId, int cabId);
        public Task<List<QualityLevelDto>> GetQualitylevels();
    }
}
