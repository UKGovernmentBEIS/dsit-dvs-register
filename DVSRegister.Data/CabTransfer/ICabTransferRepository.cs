using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CabTransfer
{
    public interface ICabTransferRepository
    {      
        public Task<Service> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId);
        public Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, int providerProfileId, string loggedInUserEmail);
        public Task<CabTransferRequest> GetCabTransferRequestDetails(int requestId);
        public Task<List<CabUser>> GetActiveCabUsers(int cabId);
    }
}
