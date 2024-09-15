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
        private readonly ILogger<ExceptionHandlerMiddleware> logger;
        public BasicAuthMiddleware(RequestDelegate next, IOptions<BasicAuthMiddlewareConfiguration> options, ILogger<ExceptionHandlerMiddleware> logger)
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
                logger.LogError($"An unexpected error occurred: {ex}");
                logger.LogError($"Stacktrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception");
                    Console.Write(String.Concat(ex.InnerException.StackTrace, ex.InnerException.Message));
                }
                // Redirect to error page 
                
                if (requestPath.Contains("cab-service"))
                    httpContext.Response.Redirect(Constants.CabRegistrationErrorPath);
                else
                    httpContext.Response.Redirect(Constants.CommonErrorPath);
            }
        }
        private bool IsAuthorised(HttpContext httpContext)
        {
            try
            {
                bool returnValue = false;
                bool hasAuthorizationHeader = httpContext.Request.Headers.ContainsKey("Authorization");
                if (hasAuthorizationHeader)
                {
                    var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers["Authorization"]);
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                    var username = credentials[0];
                    var password = credentials[1];

                    returnValue = configuration.Username == username
                           && configuration.Password == password;  
                    if(!returnValue ) 
                    {
                        Console.WriteLine("Basic Auth Details Entered Wrong ");
                    }
                }
                return returnValue;
            }
            catch(Exception ex)
            {
                // Default to denying access if anything goes wrong
                Console.WriteLine("Basic Auth Details Entered Wrong " + ex);
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

