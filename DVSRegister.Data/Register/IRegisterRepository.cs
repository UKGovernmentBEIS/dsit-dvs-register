using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRegisterRepository
    {
        public Task<List<RegisterPublishLog>> GetRegisterPublishLogs();
        public Task<List<Provider>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
        public  Task<Provider> GetProviderDetails(int providerId);
    }
}
