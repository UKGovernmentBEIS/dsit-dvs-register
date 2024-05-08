using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public class CabService : ICabService
    {
        private readonly ICabRepository cabRepository;
        private readonly IMapper automapper;      

        public CabService(ICabRepository cabRepository, IMapper automapper)
        {
            this.cabRepository = cabRepository;
            this.automapper = automapper;           
        }
        public async Task<GenericResponse> SaveCertificateInformation(CertificateInfoDto certificateInfo)
        {           

            CertificateInformation certificateInformation = new CertificateInformation();
            automapper.Map(certificateInfo, certificateInformation);            
            GenericResponse genericResponse = await cabRepository.SaveCertificateInformation(certificateInformation);
            return genericResponse;
        }
    }
}
