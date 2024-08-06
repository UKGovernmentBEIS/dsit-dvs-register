using Amazon;
using Amazon.S3;
using DVSAdmin.BusinessLogic.Services;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Models.Cookies;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Services.Cookies;
using DVSRegister.BusinessLogic.Services.GoogleAnalytics;
using DVSRegister.BusinessLogic.Services.PreAssessment;
using DVSRegister.BusinessLogic.Services.PreRegistration;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Repositories;
using DVSRegister.Middleware;
using Microsoft.AspNetCore.Mvc;
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

            services.Configure<BasicAuthMiddlewareConfiguration>(
            configuration.GetSection(BasicAuthMiddlewareConfiguration.ConfigSection));
            services.AddControllersWithViews();          
            string connectionString = string.Format(configuration.GetValue<string>("DB_CONNECTIONSTRING"));
            services.AddDbContext<DVSRegisterDbContext>(opt =>
                opt.UseNpgsql(connectionString));

            ConfigureGovUkNotify(services);
            ConfigureSession(services);
            ConfigureDvsRegisterServices(services);
            ConfigureAutomapperServices(services);
            ConfigureCookieService(services);
            ConfigureGoogleAnalyticsService(services);

            ConfigureS3Client(services);
            ConfigureS3FileWriter(services);
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
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;               
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
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IBucketService, BucketService>();
            services.AddScoped<IAVService, AVService>();
            services.AddSingleton<CookieService>();
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

        private void ConfigureS3Client(IServiceCollection services)
        {
            var s3Config = new S3Configuration();
            configuration.GetSection(S3Configuration.ConfigSection).Bind(s3Config);
            string localAccessKey = configuration.GetValue<string>("S3:LocalDevOnly_AccessKey")??string.Empty;
            string localSecreKey = configuration.GetValue<string>("S3:LocalDevOnly_SecretKey")??string.Empty;
            string localServiceURL = configuration.GetValue<string>("S3:LocalDevOnly_ServiceUrl")??string.Empty;

            if (!string.IsNullOrEmpty(localAccessKey) && !string.IsNullOrEmpty(localSecreKey) && !string.IsNullOrEmpty(localSecreKey))
            {

                // For local development connect to a local instance of Minio add the access key , secret key and service url in local user secrets only
                var clientConfig = new AmazonS3Config
                {
                    ServiceURL = localServiceURL,
                    ForcePathStyle = true,
                };

                services.AddScoped(_ => new AmazonS3Client(localAccessKey, localSecreKey, clientConfig));
            }

            else
            {
                services.AddScoped(_ => new AmazonS3Client(RegionEndpoint.GetBySystemName(s3Config.Region)));
            } 

        }

        private void ConfigureS3FileWriter(IServiceCollection services)
        {
            services.Configure<S3Configuration>(
                configuration.GetSection(S3Configuration.ConfigSection));
            services.AddScoped<IBucketService, BucketService>();
            services.AddScoped<S3FileKeyGenerator>();
        }

        private void ConfigureCookieService(IServiceCollection services)
        {
            services.Configure<CookieServiceConfiguration>(
                configuration.GetSection(CookieServiceConfiguration.ConfigSection));

            // Change the default antiforgery cookie name so it doesn't include Asp.Net for security reasons
            services.AddAntiforgery(options =>
            {

                options.Cookie.Name = "Antiforgery";
                options.Cookie.HttpOnly = true;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            services.AddScoped<ICookieService, CookieService>();
        }
        private void ConfigureGoogleAnalyticsService(IServiceCollection services)
        {
            services.Configure<GoogleAnalyticsConfiguration>(
                configuration.GetSection(GoogleAnalyticsConfiguration.ConfigSection));
            services.AddScoped<GoogleAnalyticsService, GoogleAnalyticsService>();
        }

    }
}
