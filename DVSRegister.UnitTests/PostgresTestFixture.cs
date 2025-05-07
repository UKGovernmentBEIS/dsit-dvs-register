using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace DVSRegister.UnitTests
{

    /// <summary>
    /// Set of pre conditions need to run the tests
    /// </summary>
    public class PostgresTestFixture : IAsyncLifetime
    {
        private PostgreSqlContainer _postgresContainer;
        public DVSRegisterDbContext DbContext { get; private set; }




        public PostgresTestFixture()
        {
            _postgresContainer = new PostgreSqlBuilder()
               .WithDatabase("unittestdb")
               .WithUsername("postgres")
               .WithPassword("postgres")
               .Build();
            DbContext = null!;
        }
        public async Task InitializeAsync()
        {          

            await _postgresContainer.StartAsync();
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(_postgresContainer.GetConnectionString())
                .Options;

            DbContext = new DVSRegisterDbContext(options);
           
            await DbContext.Database.MigrateAsync();
            await DbContext.User.AddAsync(new User { UserName = "test.user@dsit.gov.com", Email = "test.user@dsit.gov.com", Profile = "DSIT", CreatedDate = DateTime.UtcNow });
            await DbContext.User.AddAsync(new User { UserName = "test.user123@dsit.gov.com", Email = "test.user123@dsit.gov.com", Profile = "DSIT", CreatedDate = DateTime.UtcNow });

           await DbContext.CabUser.AddAsync(new CabUser { CabEmail = "test.user123@ie.ey.com", CabId = 1, CreatedTime = DateTime.UtcNow });
           await DbContext.CabUser.AddAsync(new CabUser { CabEmail = "test.user@dsit.gov.com", CabId = 2, CreatedTime = DateTime.UtcNow });          
           await DbContext.SaveChangesAsync();


           

        }

        public async Task DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
        }
    }
}
