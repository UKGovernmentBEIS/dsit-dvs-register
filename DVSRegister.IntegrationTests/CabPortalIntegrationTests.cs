using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace DVSRegister.IntegrationTests
{
    public class CabPortalIntegrationTests : IClassFixture<PostgresIntegrationTestFixture>
    {
        private readonly CustomWebApplicationFactory<Program> factory;
        private readonly IConfiguration configuration;


        public CabPortalIntegrationTests(PostgresIntegrationTestFixture fixture)
        {
            factory = new CustomWebApplicationFactory<Program>(fixture);
            configuration = factory.Services.GetRequiredService<IConfiguration>(); 
        }


        [Fact]
        public async Task Test1()
        {
            var client = factory.CreateClient();
            var authenticationString = $"{configuration["BasicAuth:UserName"]}:{configuration["BasicAuth:Password"]}";
            string base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            var request = new HttpRequestMessage(HttpMethod.Get, "/cab-service/home");
            request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode(); 

        }
    }
}