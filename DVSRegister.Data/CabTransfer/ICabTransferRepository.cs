using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CabTransfer
{
    public interface ICabTransferRepository
    {
        public Task<List<CabTransferRequest>> GetServiceTransferRequests(int cabId);
        public Task<Service> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId);
        public Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, int providerProfileId, string loggedInUserEmail);
        public Task<CabTransferRequest> GetCabTransferRequestDeatils(int requestId);
    }
}
