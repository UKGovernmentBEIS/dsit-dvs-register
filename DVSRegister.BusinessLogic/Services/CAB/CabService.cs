using AutoMapper;
using DVSRegister.BusinessLogic.Extensions;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
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
        public async Task<PreRegistrationDto> GetPreRegistrationDetails(string URN)
        {
            var preRegistration = await cabRepository.GetPreRegistrationDetails(URN);
            PreRegistrationDto preRegistrationDto = new PreRegistrationDto();
            automapper.Map(preRegistration, preRegistrationDto);
            return preRegistrationDto;
        }


        public async Task<GenericResponse> SaveCertificateInformation(CertificateInfoDto certificateInfo)
        {

            CertificateInformation certificateInformation = new CertificateInformation();
            automapper.Map(certificateInfo, certificateInformation);
            GenericResponse genericResponse = await cabRepository.SaveCertificateInformation(certificateInformation);
            return genericResponse;
        }

        public async Task<bool> ValidateURN(string URN)
        {
            bool valid = false;
            UniqueReferenceNumber uniqueReferenceNumber = await cabRepository.GetURNDetails(URN);
            //URN is valid only if the status is Approved(Approved - CAB Validation pending) and not exceed 60 days
            //after application approved


            if (uniqueReferenceNumber != null)
            {
                if (uniqueReferenceNumber.URNStatus == URNStatusEnum.Approved)
                {
                    if (!ExpiredDateValidator.CheckExpired(uniqueReferenceNumber.ModifiedDate))
                    {
                        valid = true;
                        // if valid change status to verified by cab
                        uniqueReferenceNumber.URNStatus = URNStatusEnum.ValidatedByCAB;
                        await cabRepository.UpdateURNStatus(uniqueReferenceNumber);
                    }
                    else
                    {
                        valid = false;
                        uniqueReferenceNumber.URNStatus = URNStatusEnum.Expired;
                        await cabRepository.UpdateURNStatus(uniqueReferenceNumber);
                    }
                }               
            }
            return valid;
        }
    }
}
