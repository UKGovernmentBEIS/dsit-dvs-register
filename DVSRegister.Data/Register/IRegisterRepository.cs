using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRegisterRepository
    {
        public Task<List<RegisterPublishLog>> GetRegisterPublishLogs();
        public Task<List<ProviderProfile>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
        public  Task<ProviderProfile> GetProviderDetails(int providerId);
        public Task<Service> GetServiceDetails(int serviceId);
    }
}
