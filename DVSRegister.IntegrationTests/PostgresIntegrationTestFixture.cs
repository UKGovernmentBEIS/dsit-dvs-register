using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace DVSRegister.IntegrationTests
{

    /// <summary>
    /// Set of pre conditions need to run the tests
    /// </summary>
    public class PostgresIntegrationTestFixture : IAsyncLifetime
    {
        private PostgreSqlContainer _postgresContainer;
        public DVSRegisterDbContext DbContext { get; private set; }


        public PostgresIntegrationTestFixture()
        {
            _postgresContainer = new PostgreSqlBuilder()
               .WithDatabase("integrationtestdb")
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
           await DbContext.CabUser.AddAsync(new CabUser { CabEmail = "test.user@dsit.gov.uk", CabId = 2, CreatedTime = DateTime.UtcNow });          
            await DbContext.SaveChangesAsync();

        }

        public string GetConnectionString()
        {
            return _postgresContainer.GetConnectionString();
        }

        public async Task DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
        }
    }
}
