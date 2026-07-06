using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ITrustFrameworkService
    {
        public Task<List<TrustFrameworkVersionDto>> GetActiveTrustFrameworkVersions();
        public Task<List<CabDto>> GetCabs();
        public Task<List<ServiceDto>> GetPublishedUnderpinningServices( string SearchText);
        public Task<List<ServiceDto>> GetServicesWithManualUnderinningService(string searchText);
        public Task<ServiceDto> GetServiceDetails(int serviceId);

    }
}
