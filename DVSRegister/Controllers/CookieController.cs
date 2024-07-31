using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.Models.Cookies;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Models.Cookies;
using Microsoft.Extensions.Options;
using DVSRegister.BusinessLogic.Services.Cookies;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Controllers;

public class CookieController : Controller
{
    private readonly ICookieService _cookieService;
    public readonly CookieServiceConfiguration _configuration;

    public CookieController(ICookieService cookieService, IOptions<CookieServiceConfiguration> config)
    {
        this._cookieService = cookieService;
        this._configuration = config.Value;
    }

    [HttpGet("cookies")]
    public IActionResult CookieSettings_Get(bool changesHaveBeenSaved = false)
    {
        _cookieService.TryGetCookie<CookieSettings>(Request, _configuration.CookieSettingsCookieName, out var cookie);

        var viewModel = new CookieSettingsViewModel
        {
            GoogleAnalytics = cookie?.GoogleAnalytics is true,
            ChangesHaveBeenSaved = changesHaveBeenSaved
        };
        return View("CookiePage", viewModel);
    }

    [HttpPost("cookies")]
    public IActionResult CookieSettings_Post(CookieSettingsViewModel viewModel)
    {
        var cookieSettings = new CookieSettings
        {
            Version = _configuration.CurrentCookieMessageVersion,
            ConfirmationShown = true,
            GoogleAnalytics = (bool)viewModel.GoogleAnalytics,
        };
        _cookieService.SetCookie(Response, _configuration.CookieSettingsCookieName, cookieSettings);
        return CookieSettings_Get(changesHaveBeenSaved: true);
    }

    [HttpPost("cookie-consent")]
    public IActionResult CookieConsent(CookieConsentViewModel cookieConsent)
    {
        if (cookieConsent.Consent == "hide")
        {
            return Redirect(cookieConsent.ReturnUrl);
        }
        var cookiesAccepted = cookieConsent.Consent == "accept";
        var cookieSettings = new CookieSettings
        {
            Version = _configuration.CurrentCookieMessageVersion,
            ConfirmationShown = false,
            GoogleAnalytics = cookiesAccepted
        };
        _cookieService.SetCookie(Response, _configuration.CookieSettingsCookieName, cookieSettings);
        return Redirect(cookieConsent.ReturnUrl);
    }

    [HttpGet("cookie-details")]
    public IActionResult CookieDetails()
    {
        return View("CookieDetails");
    }

}