using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;

namespace DVSRegister.Data
{
    public interface IHomeRepository
    {
        public Task<PaginatedResult<Service>> GetDraftApplications(int cabId, int pageNumber, string sort, string sortAction);
        public Task<PaginatedResult<Service>> GetSentBackApplications(int cabId, int pageNumber, string sort, string sortAction);
        public Task<PaginatedResult<Service>> GetPendingReassignmentRequests(int cabId, int pageNumber, string sort, string sortAction);
        public Task<Dictionary<string, int>> GetPendingCounts(int cabId);
        public Task<PaginatedResult<ProviderProfile>> GetAllProviders(int cabId, int pageNumber, string sort, string sortAction, string searchText);
        public Task<Service> GetServiceDetails(int serviceId);
    }
}
