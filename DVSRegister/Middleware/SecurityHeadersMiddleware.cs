using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;

namespace DVSRegister.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public const string NonceContextKey = "CSP-Nonce";
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;//Storing the reference to next middleware in pipeline
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate a nonce for the current request
            var nonce = Guid.NewGuid().ToString("N");
            context.Items[NonceContextKey] = nonce;

            if (!context.Response.HasStarted)
            {
                // Added security headers
                context.Response.Headers["X-Frame-Options"] = "DENY";

                //CSP with nonce for inline scripts
                context.Response.Headers["Content-Security-Policy"] =
                $"script-src 'nonce-{nonce}' 'unsafe-inline' 'self' https:; " +
                "connect-src 'self'; " +
                "img-src 'self'; " +
                "style-src 'self'; " +
                "base-uri 'self'; " +
                "font-src 'self'; " +
                "form-action 'self';";
            }
            // Calling the next middleware in the pipeline
            await _next(context);
        }
    }
    public static class SecurityContextExtensions
    {
        public static HtmlString GetScriptNonce(this HttpContext context)
        {
            return new HtmlString((string)context.Items[SecurityHeadersMiddleware.NonceContextKey] ?? "");
        }
    }
}