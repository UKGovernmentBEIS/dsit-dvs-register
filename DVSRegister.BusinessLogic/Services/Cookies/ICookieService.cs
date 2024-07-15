using System;
using Microsoft.AspNetCore.Http;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.Cookies;

namespace DVSRegister.BusinessLogic.Services.Cookies
{
    public interface ICookieService
    {
        bool TryGetCookie<T>(HttpRequest request, string cookieName, out T cookie);
        bool CookieSettingsAreUpToDate(HttpRequest request);
        bool HasAcceptedGoogleAnalytics(HttpRequest request);
        BannerState GetAndUpdateBannerState(HttpRequest request, HttpResponse response);
        void SetCookie<T>(HttpResponse response, string cookieName, T cookie);
     
    }
}
