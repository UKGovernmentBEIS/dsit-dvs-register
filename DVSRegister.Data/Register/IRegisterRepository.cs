using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRegisterRepository
    {
        public Task<List<Provider>> GetProviders(string providerName = "");
    }
}
