using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DVSRegister.Models.Cookies;

namespace DVSRegister.Services.Cookies
{
    public class CookieService 
    {
        public readonly CookieServiceConfiguration Configuration;
        private readonly ILogger<CookieService> logger;

        public CookieService(IOptions<CookieServiceConfiguration> options, ILogger<CookieService> logger)
        {
            Configuration = options.Value;
            this.logger = logger;
        }

        public bool TryGetCookie<T>(HttpRequest request, string cookieName, out T cookie)
        {
            if (request.Cookies.TryGetValue(cookieName, out var cookieString))
            {
                try
                {
                    cookie = JsonConvert.DeserializeObject<T>(cookieString);
                    return true;
                }
                catch (JsonException)
                {
                    logger.LogWarning("There was an error in deserializing the cookie string '{}' to the type '{}'", cookieString, nameof(T));
                    // In case of failure, return false as if there was no cookie
                }
            }

            cookie = default;
            return false;
        }

        public bool CookieSettingsAreUpToDate(HttpRequest request)
        {
            return TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie) &&
                   cookie.Version == Configuration.CurrentCookieMessageVersion;
        }

        public bool HasAcceptedGoogleAnalytics(HttpRequest request)
        {
            return CookieSettingsAreUpToDate(request)
                   && TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie)
                   && cookie.GoogleAnalytics;
        }

        public BannerState GetAndUpdateBannerState(HttpRequest request, HttpResponse response)
        {
            // Cookie settings page doesn't display the banner
            if (request.GetEncodedUrl().Contains("/cookies"))
            {
                return BannerState.Hide;
            }

            // Banner is displayed if the most recent cookie version wasn't reviewed.
            if (!CookieSettingsAreUpToDate(request))
            {
                return BannerState.ShowBanner;
            }

            if (TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie))
            {
                // We don't need to show anything else after showing the confirmation
                if (cookie.ConfirmationShown)
                {
                    return BannerState.Hide;
                }

                // Otherwise, update the cookie to only show the confirmation banner once
                var bannerState = cookie.GoogleAnalytics ? BannerState.ShowAccepted : BannerState.ShowRejected;
                cookie.ConfirmationShown = true;
                SetCookie(response, Configuration.CookieSettingsCookieName, cookie);
                return bannerState;
            }

            return BannerState.Hide;
        }

        public void SetCookie<T>(HttpResponse response, string cookieName, T cookie)
        {
            var cookieString = JsonConvert.SerializeObject(cookie);
            response.Cookies.Append(
                cookieName,
                cookieString,
                new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    MaxAge = TimeSpan.FromDays(Configuration.DefaultDaysUntilExpiry),
                    HttpOnly = true
                });
        }
        public void RemoveCookie(HttpResponse response, string cookieName)
        {
            var options = new CookieOptions 
            {
                Secure = true,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Path = "/",
                Domain = "localhost",
                Expires = DateTime.UnixEpoch
            };

            // Both delete and overwrite with expired cookie
            response.Cookies.Delete(cookieName, options);
            response.Cookies.Append(cookieName, "", options);
        }
    }
}