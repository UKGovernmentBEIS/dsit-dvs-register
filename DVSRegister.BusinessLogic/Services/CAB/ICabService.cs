using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<GenericResponse> SaveCertificateInformation(CertificateInfoDto certificateInfo);
    }
}
