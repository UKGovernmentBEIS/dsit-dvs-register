using DVSAdmin.BusinessLogic.Services;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Services.PreAssessment;
using DVSRegister.BusinessLogic.Services.PreRegistration;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Repositories;
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
            ConfigureSession(services);
            ConfigureDvsRegisterServices(services);
            ConfigureAutomapperServices(services);
        }

       

        private void ConfigureGovUkNotify(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, GovUkNotifyApi>();
            services.Configure<GovUkNotifyConfiguration>(
                configuration.GetSection(GovUkNotifyConfiguration.ConfigSection));
        }
        private void ConfigureSession(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // ToDo:Adjust the timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        public void ConfigureDvsRegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPreRegistrationRepository, PreRegistrationRepository>();
            services.AddScoped<IPreRegistrationService, PreRegistrationService>();
            services.AddScoped<IURNService, URNService>();
            services.AddScoped<ICabService, CabService>();
            services.AddScoped<ISignUpService, SignUpService>();
            services.AddScoped<ICabRepository, CabRepository>();
            services.AddScoped<IBucketService, BucketService>(opt =>
            {
                // TODO: uncomment below lines once aws provisioned
                //string bucketName = string.Format(configuration.GetValue<string>("BucketName"));
                //string accessKey = string.Format(configuration.GetValue<string>("AWSAccessKey"));
                //string password = string.Format(configuration.GetValue<string>("AWSSecretPassword"));
                //return new BucketService(bucketName, accessKey, password);
                return new BucketService("", "", ""); // TODO: remove this line once aws provisioned
            });
            services.AddScoped<IAVService, AVService>();
            services.AddScoped(opt =>
            {
                string userPoolId = string.Format(configuration.GetValue<string>("UserPoolId"));
                string clientId = string.Format(configuration.GetValue<string>("ClientId")); ;
                string region = string.Format(configuration.GetValue<string>("Region"));
                return new CognitoClient(userPoolId, clientId, region);
            });

        }
        public void ConfigureAutomapperServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
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
