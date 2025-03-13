using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVSRegister.Controllers
{
    [ValidCognitoToken]
    public class BaseController : Controller
    {
        public string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        public int CabId => HttpContext.Session.Get<int>("CabId") ;
        public string Cab
        {
            get
            {
                var identity = HttpContext?.User.Identity as ClaimsIdentity;
                var profileClaim = identity?.Claims.FirstOrDefault(c => c.Type == "profile");
                return profileClaim?.Value ?? string.Empty;
            }
        }

        public void SetRefererURL()
        {
            string refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            ViewBag.RefererUrl = refererUrl;
        }
    }
}
