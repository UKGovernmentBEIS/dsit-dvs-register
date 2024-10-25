namespace DVSRegister.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private const string sources = "https://www.google-analytics.com https://ssl.google-analytics.com https://www.googletagmanager.com https://www.region1.google-analytics.com https://region1.google-analytics.com; ";
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

                //CSP with nonce for inline scripts
                context.Response.Headers["Content-Security-Policy"] =
                "script-src 'unsafe-inline' 'self' " + sources +
                "object-src 'none'; " +
                "connect-src 'self' " + sources +
                "img-src 'self' " + sources +
                "style-src 'self'; " +
                "base-uri 'self'; " +
                "font-src 'self'; " +
                "form-action 'self';";
            }
            // Calling the next middleware in the pipeline
            await _next(context);
        }
    }
}