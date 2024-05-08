using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CAB
{
    public interface ICabRepository
    {
        public Task<GenericResponse> SaveCertificateInformation(CertificateInformation certificateInformation);
    }
}
