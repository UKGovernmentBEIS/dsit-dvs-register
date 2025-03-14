using System.Net.Http.Headers;
using System.Text;
using DVSRegister.CommonUtility;
using Microsoft.Extensions.Options;

namespace DVSRegister.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly BasicAuthMiddlewareConfiguration configuration;
        private readonly ILogger<BasicAuthMiddleware> logger;
        public BasicAuthMiddleware(RequestDelegate next, IOptions<BasicAuthMiddlewareConfiguration> options, ILogger<BasicAuthMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
            configuration = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string requestPath = httpContext.Request.Path.ToString().ToLower();
            try
            {
                if (IsAuthorised(httpContext))
                {
                    await next.Invoke(httpContext);
                }
                else
                {
                    SendUnauthorisedResponse(httpContext);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred in BasicAuthMiddleware.");

                if (ex.InnerException != null)
                {
                    logger.LogError(ex.InnerException, "Inner exception in BasicAuthMiddleware.");
                }
                // Redirect to error page 
                string redirectPath = requestPath.Contains("cab-service")
                    ? Constants.CabRegistrationErrorPath
                    : Constants.CommonErrorPath;
                
                httpContext.Response.Redirect(redirectPath);
            }
        }
        private bool IsAuthorised(HttpContext httpContext)
        {
            try
            {
                if (!httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    logger.LogWarning("Basic authentication header missing in request.");
                    return false;
                }
                
                var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                
                if (credentials.Length != 2)
                {
                    logger.LogError("Invalid Basic Authentication format.");
                    return false;
                }
                
                var username = credentials[0];
                var password = credentials[1];

                bool isValidUser = configuration.Username == username && configuration.Password == password;
                
                if (!isValidUser ) 
                {
                    logger.LogWarning("Basic authentication failed due to incorrect credentials.");
                }
                return isValidUser;
            }
            catch(Exception ex)
            {
                // Default to denying access if anything goes wrong
                logger.LogError(ex, "Error during Basic authentication.");
                return false;
            }
        }

        private static void SendUnauthorisedResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            AddOrUpdateHeader(httpContext, "WWW-Authenticate", $"Basic realm=DVS-Register-Beta");
        }

        private static void AddOrUpdateHeader(HttpContext httpContext, string headerName, string headerValue)
        {
            if (httpContext.Response.Headers.ContainsKey(headerName))
            {
                httpContext.Response.Headers[headerName] = headerValue;
            }
            else
            {
                httpContext.Response.Headers.Add(headerName, headerValue);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}

