using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {

        public Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs();
        public Task<List<ProviderDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
        public Task<ProviderDto> GetProviderWithServiceDeatils(int providerId);
    }
}
