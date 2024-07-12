using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cookie")]
    public class CookieController : Controller
    {
        [HttpGet("")]
        public IActionResult CookiePage()
        {
            return View();
        }
    }
}
