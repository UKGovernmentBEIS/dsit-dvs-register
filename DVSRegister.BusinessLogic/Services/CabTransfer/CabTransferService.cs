using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data.CabTransfer;

namespace DVSRegister.BusinessLogic.Services.CabTransfer
{
    public class CabTransferService: ICabTransferService
    {
        private readonly ICabTransferRepository cabTransferRepository;
        private readonly IMapper automapper;

        public CabTransferService(ICabTransferRepository cabTransferRepository, IMapper automapper)
        {
            this.cabTransferRepository = cabTransferRepository;
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
    }
}
