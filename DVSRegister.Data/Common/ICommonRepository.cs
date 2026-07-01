using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface ICommonRepository
    {
        public Task<List<TrustFrameworkVersion>> GetActiveTfVersion();
    }
}
