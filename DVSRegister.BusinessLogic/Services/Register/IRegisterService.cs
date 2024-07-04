using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {
        public Task<List<ProviderDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
    }
}
