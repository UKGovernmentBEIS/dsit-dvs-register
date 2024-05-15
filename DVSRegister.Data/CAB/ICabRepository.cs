using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CAB
{
    public interface ICabRepository
    {
        public Task<GenericResponse> SaveCertificateInformation(CertificateInformation certificateInformation);
        public Task<UniqueReferenceNumber> GetURNDetails(string URN);
        public Task<GenericResponse> UpdateURNStatus(UniqueReferenceNumber uniqueReferenceNumber);
        public Task<PreRegistration> GetPreRegistrationDetails(string URN);

        public Task<List<Role>> GetRoles();
        public Task<List<IdentityProfile>> GetIdentityProfiles();
        public Task<List<SupplementaryScheme>> GetSupplementarySchemes();
    }
}
