using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;

namespace DVSRegister.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BasicAuthMiddlewareConfiguration configuration;

        public BasicAuthMiddleware(RequestDelegate next, IOptions<BasicAuthMiddlewareConfiguration> options)
        {
            _next = next;
            configuration = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (IsAuthorised(httpContext))
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                SendUnauthorisedResponse(httpContext);
            }
        }
        private bool IsAuthorised(HttpContext httpContext)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                var returnValue = configuration.Username == username
                       && configuration.Password == password;
                Console.WriteLine("Configuration Username and Password: "+configuration.Username+"-"+configuration.Password);
                Console.WriteLine("Given Username and Password: " + username + "-" + password);
                Console.WriteLine("Computed Return Value: " +returnValue);

                return returnValue;
            }
            catch(Exception ex)
            {
                // Default to denying access if anything goes wrong
                Console.WriteLine("Basic Auth Details Entered Wrong " + ex.Message);
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

