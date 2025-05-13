using DVSRegister.Data;
using DVSRegister.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.Security.Claims;

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
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT","Test");
            builder.ConfigureServices(services =>
            {
              var dbContextDescriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<DVSRegisterDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);
                services.AddDbContext<DVSRegisterDbContext>(options =>options.UseNpgsql(fixture.GetConnectionString()));
                MockCognitoClientAndTokenHandler(services);

            });
            //to do: remove congig other than testing
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            });

            builder.UseEnvironment("Test");
        }

        private static void MockCognitoClientAndTokenHandler(IServiceCollection services)
        {
            var mockCognitoClient = Substitute.For<CognitoClient>("test", "test", "test");
            services.AddScoped(provider => mockCognitoClient);
            var mockTokenHandler = Substitute.For<JsonWebTokenHandler>();
            var claimsIdentity = new ClaimsIdentity(new Claim[] { new("profile", "DSIT") });
            var tokenValidationResult = new TokenValidationResult
            {
                IsValid = true,
                ClaimsIdentity = claimsIdentity
            };
            mockTokenHandler.ValidateTokenAsync(Arg.Any<string>(), Arg.Any<TokenValidationParameters>()).Returns(tokenValidationResult);
          
            TokenHandlerProvider.TokenHandler = mockTokenHandler;
        }
    }
}
