using DVSRegister.Models.CAB;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using DVSAdmin.BusinessLogic.Services;

namespace DVSRegister.Controllers
{
    [Route("cab-application-registration")]
    public class LoginController : Controller
    {
        private readonly ISignUpService _signUpService;

        public LoginController(ISignUpService signUpService)
        {
            _signUpService = signUpService;
        }

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


        [HttpPost("login-validation")]
        public async Task<IActionResult> LoginToAccountAsync(LoginPageViewModel loginPageViewModel)
        {
            ModelState.Remove(nameof(LoginPageViewModel.MFACode));
            if (ModelState["Email"].Errors.Count ==0 && ModelState["Password"].Errors.Count ==0)
            {
                var loginResponse = await _signUpService.SignInAndWaitForMfa(loginPageViewModel.Email, loginPageViewModel.Password);
                if (loginResponse.Length > 0)
                {
                    HttpContext?.Session.Set("Email", loginPageViewModel.Email);
                    HttpContext?.Session.Set("Session", loginResponse);
                    return RedirectToAction("MFAConfirmation");
                }
                else
                {
                    ModelState.AddModelError("Email", "Incorrect email or password");
                    ModelState.AddModelError("Password", "Incorrect email or password");
                    return View("LoginPage");
                }
            }
            else
            {
                return View("LoginPage");
            }
        }

        [HttpGet("mfa-confirmation")]
        public IActionResult MFAConfirmation()
        {
            return View("MFAConfirmation");
        }

        [HttpPost("")]
        public async Task<IActionResult> ConfirmMFACodeLogin(LoginPageViewModel loginPageViewModel)
        {
            if (ModelState["MFACode"].Errors.Count == 0)
            {
                string Session = HttpContext?.Session.Get<string>("Session");
                string Email = HttpContext?.Session.Get<string>("Email");
                var mfaConfirmationCheckResponse = await _signUpService.ConfirmMFAToken(Session, Email, loginPageViewModel.MFACode);

                if (mfaConfirmationCheckResponse.Length > 0)
                {
                    HttpContext?.Session.Set("IdToken", mfaConfirmationCheckResponse);
                    return RedirectToAction("LandingPage", "Cab");
                }
                else
                {
                    ModelState.AddModelError("MFACode", "Wrong MFA Code Provided from Authenticator App");
                    return View("MFAConfirmation", loginPageViewModel);
                }
            }
            else
            {
                return View("MFAConfirmation");
            }
        }
    }
}
