using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.CabTransfer;

namespace DVSRegister.BusinessLogic.Services.CabTransfer
{
    public class CabTransferService: ICabTransferService
    {
        private readonly ICabTransferRepository cabTransferRepository;
        private readonly IRemoveProviderService removeProviderService;
        private readonly IMapper automapper;

        public CabTransferService(ICabTransferRepository cabTransferRepository, IRemoveProviderService removeProviderService, IMapper automapper)
        {
            this.cabTransferRepository = cabTransferRepository;
            this.removeProviderService = removeProviderService;
            this.automapper = automapper;            
        }


        public async Task<List<CabTransferRequestDto>> GetServiceTransferRequests(int cabId)
        {
            var list = await cabTransferRepository.GetServiceTransferRequests(cabId);
            return automapper.Map<List<CabTransferRequestDto>>(list);
           
        }

        public async Task<ServiceDto> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId)
        {
            var service = await cabTransferRepository.GetServiceDetailsWithCabTransferDetails(serviceId, cabId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }
        public async Task<CabTransferRequestDto> GetCabTransferRequestDeatils(int requestId)
        {
            var requestDetails = await cabTransferRepository.GetCabTransferRequestDeatils(requestId);
            return automapper.Map<CabTransferRequestDto>(requestDetails);

        }

        public async Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, int providerProfileId, string loggedInUserEmail)
        {            
            GenericResponse genericResponse = await cabTransferRepository.ApproveOrCancelTransferRequest(approve,requestId, providerProfileId,loggedInUserEmail);
            if(genericResponse.Success) 
            {
                genericResponse = await removeProviderService.UpdateProviderStatus(providerProfileId, loggedInUserEmail, EventTypeEnum.ApproveOrRejectReAssign, TeamEnum.CAB);
            }
            return genericResponse;
        }
    }
}
