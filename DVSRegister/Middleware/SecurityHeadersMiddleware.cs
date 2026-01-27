using Microsoft.AspNetCore.Html;

namespace DVSRegister.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
       
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
                var nonce = Guid.NewGuid().ToString("N");
                context.Items["Nonce"] = nonce;

                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";
                context.Response.Headers["Content-Security-Policy"] =
                "object-src 'none'; " +
                "base-uri 'none';" +
                $"script-src 'nonce-{nonce}' 'strict-dynamic' https:; ";

            }

            await _next(context);
        }


    }
    public static class SecurityContextExtensions
    {
        public static HtmlString GetScriptNonce(this HttpContext context)
        {
            return new HtmlString(Convert.ToString(context.Items["Nonce"]));
        }
    }
}
