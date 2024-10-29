namespace DVSRegister.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private const string sources = "https://www.google-analytics.com https://ssl.google-analytics.com https://www.googletagmanager.com https://www.region1.google-analytics.com https://region1.google-analytics.com; ";
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
              
                context.Response.Headers["X-Frame-Options"] = "DENY"; 
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";
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
           
            await _next(context);
        }
    }
}