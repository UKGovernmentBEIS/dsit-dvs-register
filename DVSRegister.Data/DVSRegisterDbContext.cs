
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data
{
    public class DVSRegisterDbContext : DbContext
    {
        public DVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options) : base(options)
        {
        }
        public DbSet<PreAssessment> PreAssessment { get; set; }
    }

}
