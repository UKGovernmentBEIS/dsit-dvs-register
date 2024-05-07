using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-registration")]
    public class LoginController : Controller
    {
        [HttpGet("sign-up")]
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
