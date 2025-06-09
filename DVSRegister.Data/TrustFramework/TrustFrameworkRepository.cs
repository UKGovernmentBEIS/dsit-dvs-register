using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<TrustFrameworkVersion>> GetTrustFrameworkVersions()
        {
            return await context.TrustFrameworkVersion
                .OrderBy(tf => tf.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cab>> GetCabs()
        {
            return await context.Cab
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
