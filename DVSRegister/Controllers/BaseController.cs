using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVSRegister.Controllers
{
    [ValidCognitoToken]
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
        protected string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        protected int CabId => HttpContext.Session.Get<int>("CabId") ;

        protected string ControllerName => ControllerContext.ActionDescriptor.ControllerName;
        protected string ActionName => ControllerContext.ActionDescriptor.ActionName;
        protected string Cab
        {
            get
            {
                var identity = HttpContext?.User.Identity as ClaimsIdentity;
                var profileClaim = identity?.Claims.FirstOrDefault(c => c.Type == "profile");
                return profileClaim?.Value ?? string.Empty;
            }
        }

        protected bool IsValidCabId(int cabId)
        {
            return cabId > 0;
        }

        protected void SetRefererURL()
        {
            string refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            ViewBag.RefererUrl = refererUrl;
        }

        protected IActionResult HandleInvalidCabId(int cabId)
        {  
            _logger.LogError("Invalid CabId: {CabId}. Controller: {ControllerName}, Action: {ActionName}",
                cabId, ControllerName, ActionName);
            return RedirectToAction("CabHandleException", "Error");
        }
    }
}
