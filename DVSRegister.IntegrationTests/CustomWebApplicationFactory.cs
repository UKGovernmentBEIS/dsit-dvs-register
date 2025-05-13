using DVSRegister.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
    
        private readonly PostgresIntegrationTestFixture fixture;


        public CustomWebApplicationFactory(PostgresIntegrationTestFixture fixture)
        {
            this.fixture = fixture;


        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<DVSRegisterDbContext>));

                if(dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);
             
                services.AddDbContext<DVSRegisterDbContext>(options =>
                    options.UseNpgsql(fixture.GetConnectionString()));
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            });

            builder.UseEnvironment("Development");
        }
    }
}
