using Microsoft.AspNetCore.Mvc.Razor;

namespace DVSRegister.PublishedService;

public static class PublishedServiceCollectionExtensions
{
    public static IServiceCollection AddPublishedService(this IServiceCollection services)
    {
        services.AddScoped<IGetPublishedService, GetPublishedService>();
        services.AddScoped<IResolveLegacyServiceLink, ResolveLegacyServiceLink>();
        services.Configure<RazorViewEngineOptions>(options =>
            options.ViewLocationExpanders.Add(new PublishedServiceViewLocationExpander()));

        return services;
    }
}
