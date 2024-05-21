using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<GenericResponse> SaveCertificateInformation(CertificateInfoDto certificateInfo);
        public Task<bool> ValidateURN(string URN, string cabUserId);
        public Task<PreRegistrationDto> GetPreRegistrationDetails(string URN);

        public Task<List<RoleDto>> GetRoles();
        public Task<List<IdentityProfileDto>> GetIdentityProfiles();
        public Task<List<SupplementarySchemeDto>> GetSupplementarySchemes();
        public Task<DVSRegister.Data.Entities.PreRegistration> GetURNDetails(string URN);
        public Task<bool> CheckURNValidatedByCab(string URN);
    }
}
