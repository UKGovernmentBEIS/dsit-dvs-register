using Amazon;
using Amazon.Extensions.NETCore.Setup;
using DVSRegister.CommonUtility;
using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace DVSRegister
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            string connectionString = string.Format(Constants.ConnectionStringBuiler,
            configuration.GetValue<string>("host"), configuration.GetValue<string>("user"),
             configuration.GetValue<string>("password"),configuration.GetValue<string>("name"));           
            services.AddDbContext<DVSRegisterDbContext>(opt =>
                opt.UseNpgsql(connectionString));
        }

        public void ConfigureSystemsManager(ConfigurationManager configuration, string environment)
        {
            if (environment != Environments.Development)
            {
                configuration.AddSystemsManager(source =>
                {
                    source.AwsOptions = new AWSOptions()
                    {
                        Region = RegionEndpoint.EUWest2
                    };
                    source.Optional = true;
                    source.Path = "/rds/dvsrds/";
                });
            }
        }
        public void ConfigureDatabaseHealthCheck(DVSRegisterDbContext? dbContext)
        {
            try
            {
                if (dbContext == null) throw new InvalidOperationException(Constants.DbContextNull);
                DbConnection conn = dbContext.Database.GetDbConnection();
                conn.Open();   // Check the database connection
                Console.WriteLine(Constants.DbConnectionSuccess);
                conn.Close();   // close the database connection               
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.DbConnectionFailed + ex.Message);
                throw;
            }
        }
    }
}
