using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<RegisterRepository> logger;

        public RegisterRepository(DVSRegisterDbContext context, ILogger<RegisterRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }

        public async Task<List<Provider>> GetProviders(string providerName = "")
        {
            return await context.Provider.ToListAsync();
        }
    }
}
