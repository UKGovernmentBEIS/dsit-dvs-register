using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Poc.Models;

namespace OneLogin.Poc.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        return View(new OneLoginClaimsViewModel
        {
            Subject = GetClaim("sub"),
            Email = GetClaim("email"),
            PhoneNumber = GetClaim("phone_number"),
            VectorOfTrust = GetClaim("vot"),
            SessionId = GetClaim("sid")
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OneLoginDefaults.AuthenticationScheme);
    }

    private string GetClaim(string claimType) =>
        User.FindFirst(claimType)?.Value ?? "Claim not returned";
}
