using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-registration")]
    public class CabController : Controller
    {
        [HttpGet("landing-page")]
        public IActionResult LandingPage()
        {
            return View();
        }
    }
}
