using DVSRegister.Data.Entities;

namespace DVSRegister.Data.TrustFramework
{
    public interface ITrustFrameworkRepository
    {
        public Task<List<TrustFrameworkVersion>> GetTrustFrameworkVersions();
        public Task<List<Cab>> GetCabs();
        public Task<List<Service>> GetPublishedUnderpinningServices(string searchText);
        public Task<Service> GetServiceDetails(int serviceId);
        public Task<List<Service>> GetServicesWithManualUnderinningService(string searchText);
    }
}
