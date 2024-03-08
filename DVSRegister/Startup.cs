using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Middleware;
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

            if(webHostEnvironment.IsDevelopment() || webHostEnvironment.IsStaging())
            {
                services.Configure<BasicAuthMiddlewareConfiguration>(
                    configuration.GetSection(BasicAuthMiddlewareConfiguration.ConfigSection));
            }
            services.AddControllersWithViews();
            string connectionString = string.Format(configuration.GetValue<string>("DB_CONNECTIONSTRING"));

            services.AddDbContext<DVSRegisterDbContext>(opt =>
                opt.UseNpgsql(connectionString));

            ConfigureGovUkNotify(services);
        }

        private void ConfigureGovUkNotify(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, GovUkNotifyApi>();
            services.Configure<GovUkNotifyConfiguration>(
                configuration.GetSection(GovUkNotifyConfiguration.ConfigSection));
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
