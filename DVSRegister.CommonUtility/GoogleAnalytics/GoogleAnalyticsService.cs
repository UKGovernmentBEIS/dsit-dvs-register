using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.GoogleAnalytics;
public class GoogleAnalyticsService
{
    public readonly GoogleAnalyticsConfiguration Configuration;
    private readonly ILogger<GoogleAnalyticsService> logger;

    public GoogleAnalyticsService(
        IOptions<GoogleAnalyticsConfiguration> options,
        ILogger<GoogleAnalyticsService> logger)
    {
        this.Configuration = options.Value;
        this.logger = logger;
    }
}