using DVSRegister.BusinessLogic.Models;

namespace DVSRegister.Models.CabTransfer
{
    public class ServiceManagementRequestsViewModel
    {
        public List<CabTransferRequestDto> PendingRequests { get; set; }
        public List<CabTransferRequestDto> CompletedRequests { get; set; }
    }
}
