using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services.CabTransfer
{
    public interface ICabTransferService
    {
        public Task<List<CabTransferRequestDto>> GetServiceTransferRequests(int cabId);
        public Task<ServiceDto> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId);
    }
}
