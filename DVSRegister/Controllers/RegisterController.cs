using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("register")]
    public class RegisterController : Controller
    {
        [HttpGet("register-search")]
        public IActionResult Register()
        {
            return View("Register");
        }

    }
}
