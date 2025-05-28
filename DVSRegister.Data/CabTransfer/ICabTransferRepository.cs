using DVSRegister.Data.Entities;

namespace DVSRegister.Data.CabTransfer
{
    public interface ICabTransferRepository
    {
        public Task<List<CabTransferRequest>> GetServiceTransferRequests(int cabId);
        public Task<Service> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId);
    }
}
