using DVSRegister.PublishedService;

namespace DVSRegister.Middleware
{
    public sealed class LegacyServiceLinkRedirectMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IResolveLegacyServiceLink resolveLegacyServiceLink)
        {
            var path = context.Request.Path.Value?.TrimEnd('/');
            var isLegacyPath = string.Equals(path, "/service-details", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(path, "/register/service-details", StringComparison.OrdinalIgnoreCase);

            if (!HttpMethods.IsGet(context.Request.Method) || !isLegacyPath)
            {
                await next(context);
                return;
            }

            if (!int.TryParse(context.Request.Query["serviceId"], out var serviceId))
            {
                context.Response.Redirect("/register/page-not-found");
                return;
            }

            var result = await resolveLegacyServiceLink.HandleAsync(serviceId, context.RequestAborted);         
            context.Response.Redirect(result.Match(url => url, _ => "/register/page-not-found"));
        }
    }
}
