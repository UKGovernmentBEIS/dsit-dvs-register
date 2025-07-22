using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {

        public Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs();
        public Task<List<ProviderProfileDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
        public Task<ProviderProfileDto> GetProviderWithServiceDeatils(int providerId);
        public Task<ServiceDto> GetServiceDetails(int serviceId);
    }
}
