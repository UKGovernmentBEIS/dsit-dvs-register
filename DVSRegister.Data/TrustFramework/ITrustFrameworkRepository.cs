using DVSRegister.Data.Entities;

namespace DVSRegister.Data.TrustFramework
{
    public interface ITrustFrameworkRepository
    {
        public Task<List<TrustFrameworkVersion>> GetTrustFrameworkVersions();
        public Task<List<Cab>> GetCabs();
        public Task<List<String>> GetProviderNames();
        public Task<List<String>> GetServiceNames();
    }
}
