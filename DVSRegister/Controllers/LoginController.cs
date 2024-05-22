using DVSRegister.Models.CAB;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using DVSAdmin.BusinessLogic.Services;
using System.Reflection.Metadata;
using DVSRegister.CommonUtility;

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
            string Email = HttpContext?.Session.Get<string>("Email");
            EnterCodeViewModel enterCodeViewModel = new EnterCodeViewModel();
            enterCodeViewModel.Email = Email;
            return View("EnterCode", enterCodeViewModel);
        }

        [HttpGet("mfa-registration")]
        public IActionResult MFARegistration()
        {
            MFARegistrationViewModel MFARegistrationViewModel = HttpContext?.Session.Get<MFARegistrationViewModel>("MFARegistrationViewModel"); ;
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

        [HttpPost("login-validation")]
        public async Task<IActionResult> LoginToAccountAsync(LoginPageViewModel loginPageViewModel)
        {
            ModelState.Remove(nameof(LoginPageViewModel.MFACode));
            if (ModelState["Email"].Errors.Count ==0 && ModelState["Password"].Errors.Count ==0)
            {
                var loginResponse = await _signUpService.SignInAndWaitForMfa(loginPageViewModel.Email, loginPageViewModel.Password);
                if (loginResponse.Length > 0 && loginResponse != Constants.IncorrectPassword)
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

        

        [HttpPost("confirm-mfa-code-login")]
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

        [HttpPost("enter-email")]
        public async Task<IActionResult> EnterEmailAsync(LoginPageViewModel loginPageViewModel)
        {
            if (ModelState["Email"].Errors.Count == 0)
            {
                var forgotPasswordResponse = await _signUpService.ForgotPassword(loginPageViewModel.Email);

                if (forgotPasswordResponse == "OK")
                {
                    HttpContext.Session?.Set("Email", loginPageViewModel.Email);
                    return RedirectToAction("EnterCode", "Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Incorrect Email provided");
                    return View("SignUp");
                }
            }

            else
            {
                return View("SignUp");
            };
        }

        [HttpPost("email-code-validation")]
        public IActionResult EnterCodeValidation(EnterCodeViewModel enterCodeViewModel)
        {
            if (ModelState["Code"].Errors.Count  == 0 )
            {
                HttpContext.Session?.Set("Code", enterCodeViewModel.Code);
                return RedirectToAction("CreatePassword");
            }
            else
            {
                ModelState.AddModelError("Code", "Incorrect Code entered");
                return View("EnterCode");
            }
        }

        [HttpPost("confirm-password-check")]
        public async Task<IActionResult> ConfirmPasswordCheck(ConfirmPasswordViewModel confirmPasswordViewModel)
        {
            if (ModelState["Password"].Errors.Count == 0 && ModelState["ConfirmPassword"].Errors.Count == 0)
            {
                string email = HttpContext?.Session.Get<string>("Email");
                string oneTimePassword = HttpContext?.Session.Get<string>("Code");
                var confirmPasswordResponse = await _signUpService.ConfirmPassword(email, confirmPasswordViewModel.Password,oneTimePassword);
                if(confirmPasswordResponse.Success)
                {
                    HttpContext?.Session.Set("MFARegistrationViewModel", new MFARegistrationViewModel { Email = email, Password = confirmPasswordViewModel.Password, SecretToken = confirmPasswordResponse.Data });
                    return RedirectToAction("MFARegistration", "Login");
                }
                else
                {
                    ModelState.AddModelError("ErrorMessage", "");
                    return View("ConfirmPassword", confirmPasswordViewModel);
                }
            }
            else
            {
                return View("CreatePassword", confirmPasswordViewModel);
            }
        }

        [HttpPost("mfa-confirmation-check")]
        public async Task<IActionResult> MFAConfirmationCheck(MFARegistrationViewModel viewModel)
        {
            MFARegistrationViewModel MFARegistrationViewModel = HttpContext?.Session.Get<MFARegistrationViewModel>("MFARegistrationViewModel");
            if (ModelState["MFACode"].Errors.Count == 0)
            {
                var mfaConfirmationCheckResponse = await _signUpService.MFAConfirmation(MFARegistrationViewModel.Email, MFARegistrationViewModel.Password, viewModel.MFACode);
                
                if (mfaConfirmationCheckResponse == "OK")
                {
                    return View("SignUpComplete");
                }
                else
                {
                    ModelState.AddModelError("MFACode", "Wrong MFA Code Provided from Authenticator App");
                    return View("MFARegistration", viewModel);
                }
            }
            else
            {
                return View("MFARegistration", viewModel);
            }
            
        }
    }
}
