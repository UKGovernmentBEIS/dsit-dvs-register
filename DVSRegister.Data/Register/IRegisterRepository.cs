using DVSRegister.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVSRegister.Data
{
    public interface IRegisterRepository
    {
        public Task<List<RegisterPublishLog>> GetRegisterPublishLogs();
        public Task<List<Provider>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
    }
}
