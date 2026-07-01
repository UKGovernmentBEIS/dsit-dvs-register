using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data
{
    public class CommonRepository : ICommonRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CommonRepository> logger;

        public CommonRepository(DVSRegisterDbContext context, ILogger<CommonRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<TrustFrameworkVersion>> GetActiveTfVersion()
        {
            return await context.TrustFrameworkVersion.AsNoTracking()
                .Where(x => x.IsActive).OrderBy(c => c.Order).ToListAsync();
        }
    }
}
