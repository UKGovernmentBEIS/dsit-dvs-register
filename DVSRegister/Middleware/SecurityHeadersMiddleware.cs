using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using DVSRegister.CommonUtility;
using Microsoft.Extensions.Options;

namespace DVSRegister.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;//Storing the reference to next middleware in pipeline
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
                // Added security headers
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Content-Security-Policy"] = "frame-ancestors 'none'";
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";
            }

            // Calling the next middleware in the pipeline
            await _next(context);
        }
    }
}