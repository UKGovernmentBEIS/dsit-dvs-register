using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {

        public Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs();
        public Task<PaginatedResult<ProviderProfileDto>> GetProviders(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "");
        public Task<PaginatedResult<ServiceDto>> GetServices(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "");
        public Task<ProviderProfileDto> GetProviderWithServiceDeatils(int providerId);
        public Task<ServiceDto> GetServiceDetails(int serviceId);
    }
}
