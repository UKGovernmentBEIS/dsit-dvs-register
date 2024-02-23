
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data
{
    public class DVSRegisterDbContext : DbContext
    {
        public DVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options) : base(options)
        {
        }
        
    }
}
