using DVSRegister.Models.CAB;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-registration-login")]
    public class LoginController : Controller
    {
        [HttpGet("")]
        public IActionResult StartPage()
        {
            return View("StartPage");
        }
        [HttpGet("sign-up")]
        public IActionResult SignUp()
        {
            return View("SignUp");
        }
        [HttpGet("enter-code")]
        public IActionResult EnterCode()
        {
            return View("EnterCode");
        }
        [HttpGet("mfa-registration")]
        public IActionResult MFARegistration()
        {
            MFARegistrationViewModel MFARegistrationViewModel = new MFARegistrationViewModel();
            return View("MFARegistration", MFARegistrationViewModel);
        }
        [HttpGet("create-password")]
        public IActionResult CreatePassword()
        {
            return View("CreatePassword");
        }
        [HttpGet("sign-up-complete")]
        public IActionResult SignUpComplete()
        {
            return View("SignUpComplete");
        }
        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return View("LoginPage");
        }
        [HttpGet("mfa-confirmation")]
        public IActionResult MFAConfirmation()
        {
            return View("MFAConfirmation");
        }
    }
}
