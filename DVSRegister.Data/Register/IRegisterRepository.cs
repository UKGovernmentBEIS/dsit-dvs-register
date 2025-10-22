using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;

namespace DVSRegister.Data
{
    public interface IRegisterRepository
    {
        public Task<PaginatedResult<GroupedResult<ActionLogs>>> GetUpdateLogs(int pageNumber);
        public Task<DateTime> GetLastUpdatedDate();
        public Task<PaginatedResult<ProviderProfile>> GetProviders(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "");
        public Task<PaginatedResult<Service>> GetServices(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "");
        public  Task<ProviderProfile> GetProviderDetails(int providerId);
        public Task<Service> GetServiceDetails(int serviceId);
        public Task<List<Service>> GetPublishedServices();
    }
}
