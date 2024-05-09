using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<GenericResponse> SaveCertificateInformation(CertificateInfoDto certificateInfo);
        public Task<bool> ValidateURN(string URN);
        public Task<PreRegistrationDto> GetPreRegistrationDetails(string URN);
    }
}
