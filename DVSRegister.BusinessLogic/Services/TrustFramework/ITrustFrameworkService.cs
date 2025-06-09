using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ITrustFrameworkService
    {
        public Task<List<TrustFrameworkVersionDto>> GetTrustFrameworkVersions();
        public Task<List<CabDto>> GetCabs();

    }
}
