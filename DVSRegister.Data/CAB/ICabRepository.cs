using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CAB
{
    public interface ICabRepository
    {
        public Task<GenericResponse> SaveCertificateInformation(Provider provider);
        public Task<UniqueReferenceNumber> GetURNDetails(string URN);
        public Task<GenericResponse> UpdateURNStatus(UniqueReferenceNumber uniqueReferenceNumber);
        public Task<PreRegistration> GetPreRegistrationDetails(string URN);
        public Task<List<Role>> GetRoles();
        public Task<List<IdentityProfile>> GetIdentityProfiles();
        public Task<List<SupplementaryScheme>> GetSupplementarySchemes();
        public Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile);
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName);
        public Task<GenericResponse> SaveService(Service service);
    }
}
