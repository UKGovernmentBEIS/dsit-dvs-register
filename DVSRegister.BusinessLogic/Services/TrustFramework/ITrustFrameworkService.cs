using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ITrustFrameworkService
    {
        public Task<List<TrustFrameworkVersionDto>> GetTrustFrameworkVersions();
        public Task<List<CabDto>> GetCabs();
        public Task<List<ServiceDto>> GetServices(bool isPublished, string SearchText);
        public Task<ServiceDto> GetServiceDetails(int serviceId);

    }
}
