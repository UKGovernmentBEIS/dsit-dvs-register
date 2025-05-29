using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CabTransfer
{
    public interface ICabTransferService
    {
        public Task<List<CabTransferRequestDto>> GetServiceTransferRequests(int cabId);
        public Task<ServiceDto> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId);
        public Task<CabTransferRequestDto> GetCabTransferRequestDeatils(int requestId);
        public Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, int providerProfileId, string loggedInUserEmail);
    }
}
