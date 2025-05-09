using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace DVSRegister.UnitTests.Repository
{

    /// <summary>
    /// Set of pre conditions need to run the tests
    /// </summary>
    public class PostgresTestFixture : IDisposable
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
            Initialize();
        }
        private void Initialize()
        {

            _postgresContainer.StartAsync().Wait();
            var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
                .UseNpgsql(_postgresContainer.GetConnectionString())
                .Options;

            DbContext = new DVSRegisterDbContext(options);

            DbContext.Database.MigrateAsync().Wait();
            DbContext.User.Add(new User { UserName = "test.user@dsit.gov.com", Email = "test.user@dsit.gov.com", Profile = "DSIT", CreatedDate = DateTime.UtcNow });
            DbContext.User.Add(new User { UserName = "test.user123@dsit.gov.com", Email = "test.user123@dsit.gov.com", Profile = "DSIT", CreatedDate = DateTime.UtcNow });

            DbContext.CabUser.Add(new CabUser { CabEmail = "test.user123@ie.ey.com", CabId = 1, CreatedTime = DateTime.UtcNow });
            DbContext.CabUser.Add(new CabUser { CabEmail = "test.user@dsit.gov.com", CabId = 2, CreatedTime = DateTime.UtcNow });
            DbContext.SaveChanges();
        }

       

        public string GetConnectionString()
        {
            return _postgresContainer.GetConnectionString();
        }

        public void Dispose()
        {
            _postgresContainer.StopAsync().Wait(); // Stop the container
        }
    }
}
