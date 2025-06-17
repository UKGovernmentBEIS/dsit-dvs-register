using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.TrustFramework
{
    public class TrustFrameworkRepository:ITrustFrameworkRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<TrustFrameworkRepository> logger;

        public TrustFrameworkRepository(DVSRegisterDbContext context, ILogger<TrustFrameworkRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
    }
}
