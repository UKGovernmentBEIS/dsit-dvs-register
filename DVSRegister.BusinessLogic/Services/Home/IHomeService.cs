using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IHomeService
    {
        public Task<PaginatedResult<ServiceDto>> GetDraftApplications(int cabId, int pageNumber, string sort, string sortAction);
        public Task<PaginatedResult<ServiceDto>> GetSentBackApplications(int cabId, int pageNumber, string sort, string sortAction);
        public Task<PaginatedResult<ServiceDto>> GetPendingReassignmentRequests(int cabId, int pageNumber, string sort, string sortAction);
        public Task<Dictionary<string, int>> GetPendingCounts(int cabId);
        public Task<PaginatedResult<ProviderProfileDto>> GetAllProviders(int cabId, int pageNumber, string sort, string sortAction, string searchText);       
    }
}
