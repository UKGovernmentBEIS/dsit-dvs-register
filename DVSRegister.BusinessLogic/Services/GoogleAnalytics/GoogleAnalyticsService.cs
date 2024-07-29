using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DVSRegister.BusinessLogic.Services.GoogleAnalytics;
using DVSRegister.CommonUtility;

namespace DVSRegister.BusinessLogic.Services.GoogleAnalytics;

public class GoogleAnalyticsService
{
    public readonly GoogleAnalyticsConfiguration Configuration;
    private readonly ILogger<GoogleAnalyticsService> logger;

    // If these strings are ever updated you will need to update the Google Analytics custom events on the GA site to match.
    //Pre-Reg
    private const string EventNameSummaryPageViewed = "summary_page_viewed";
    private const string EventNameSponsorPageViewed = "sponsor_page_viewed";
    //CAB
    private const string EventNameCertificateInfoCompleted = "ertificate_info_completed";
    //Register
    private const string EventNameProviderDetailsViewed = "provider_details_viewed";
    private const string EventNamePublishLogViewed = "publish_logs_viewed";


    public GoogleAnalyticsService(
        IOptions<GoogleAnalyticsConfiguration> options,
        ILogger<GoogleAnalyticsService> logger)
    {
        this.Configuration = options.Value;
        this.logger = logger;
    }


    public async Task SendSummaryPageViewedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameSummaryPageViewed
                }
            }
        });
    }

    public async Task SendSponsorPageViewedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameSponsorPageViewed
                }
            }
        });
    }

  
    public async Task SendCertificateInfoCompletedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameCertificateInfoCompleted
                }
            }
        });
    }
    
    public async Task SendProviderDetailsViewedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNameProviderDetailsViewed
                }
            },
        });
    }

    public async Task SendPublishLogViewedEventAsync(HttpRequest request)
    {
        await SendEventAsync(new GaRequestBody
        {
            ClientId = GetClientId(request),
            GaEvents = new List<GaEvent>
            {
                new()
                {
                    Name = EventNamePublishLogViewed
                }
            },
        });
    }

    private async Task SendEventAsync(GaRequestBody body)
    {
        try
        {
            await HttpRequestHelper.SendPostRequestAsync<string>(new RequestParameters
            {
                BaseAddress = Configuration.BaseUrl,
                Path = $"/mp/collect?api_secret={Configuration.ApiSecret}&measurement_id={Configuration.MeasurementId}",
                Body = new StringContent(JsonConvert.SerializeObject(body))
            });
        }
        catch (Exception e)
        {
            logger.LogError("There was an error sending an event to google analytics: {}", e.Message);
        }
    }
    
    // Cookie format: GAx.y.zzzzzzzzz.tttttttttt.
    // The z section is the client id
    // If we can't find the _ga cookie, return a new id
    private string GetClientId(HttpRequest request)
    {
        return request.Cookies.TryGetValue(Configuration.CookieName, out var cookie)
            ? cookie.Split('.')[2] 
            : Guid.NewGuid().ToString();
    }
}

public class GaRequestBody
{
    [JsonProperty(PropertyName = "client_id")]
    public string ClientId { get; set; }
    
    [JsonProperty(PropertyName = "events")]
    public List<GaEvent> GaEvents { get; set; }
}

public class GaEvent
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "params")]
    public Dictionary<string, object> Parameters { get; set; }
}