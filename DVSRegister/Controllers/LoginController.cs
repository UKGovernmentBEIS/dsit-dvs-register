using DVSAdmin.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

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

        #region Create Account /Forgot password

        [HttpGet("enter-email")]
        public IActionResult EnterEmail(bool passwordReset)
        {
            if(passwordReset) 
            {
                EnterEmailViewModel enterEmailViewModel = new EnterEmailViewModel();
                enterEmailViewModel.PasswordReset = passwordReset;
                return View("EnterEmail", enterEmailViewModel);
            }
            return View("EnterEmail");
        }

        [HttpPost("enter-email")]
        public async Task<IActionResult> EnterEmailAsync(EnterEmailViewModel enterEmailViewModel)
        {
            if (ModelState["Email"].Errors.Count == 0)
            {
                var forgotPasswordResponse = await _signUpService.ForgotPassword(enterEmailViewModel.Email);

                if (forgotPasswordResponse == "OK")
                {
                    HttpContext.Session?.Set("Email", enterEmailViewModel.Email);
                    return RedirectToAction("EnterCode", "Login", new { passwordReset = enterEmailViewModel.PasswordReset});
                }
                else
                {
                    ModelState.AddModelError("Email", forgotPasswordResponse);
                    return View("EnterEmail");
                }
            }
            else
            {
                return View("EnterEmail");
            };
        }


        [HttpGet("enter-code")]
        public IActionResult EnterCode(bool passwordReset)
        {
            string Email = HttpContext?.Session.Get<string>("Email");
            EnterCodeViewModel enterCodeViewModel = new EnterCodeViewModel();
            enterCodeViewModel.Email = Email;
            enterCodeViewModel.PasswordReset = passwordReset;
            return View("EnterCode", enterCodeViewModel);
        }

        [HttpPost("email-code-validation")]
        public IActionResult EnterCodeValidation(EnterCodeViewModel enterCodeViewModel)
        {
            if (ModelState["Code"].Errors.Count  == 0)
            {
                HttpContext.Session?.Set("Code", enterCodeViewModel.Code);
                return RedirectToAction("CreatePassword", new { passwordReset = enterCodeViewModel .PasswordReset});
            }
            else
            {
                return View("EnterCode", enterCodeViewModel);
            }
        }
       

        [HttpGet("create-password")]
        public IActionResult CreatePassword(bool passwordReset)
        {
            if(passwordReset)
            {
                ConfirmPasswordViewModel confirmPasswordViewModel = new ConfirmPasswordViewModel();
                confirmPasswordViewModel.PasswordReset = passwordReset;
                return View("CreatePassword", confirmPasswordViewModel);
            }
            return View("CreatePassword");
        }

        [HttpPost("confirm-password-check")]
        public async Task<IActionResult> ConfirmPasswordCheck(ConfirmPasswordViewModel confirmPasswordViewModel)
        {
            if (ModelState["Password"].Errors.Count == 0 && ModelState["ConfirmPassword"].Errors.Count == 0)
            {
                string email = HttpContext?.Session.Get<string>("Email");
                string oneTimePassword = HttpContext?.Session.Get<string>("Code");
                GenericResponse confirmPasswordResponse = new GenericResponse();
                if (confirmPasswordViewModel.PasswordReset != null && confirmPasswordViewModel.PasswordReset==true)
                {
                    confirmPasswordResponse = await _signUpService.ResetPassword(email, confirmPasswordViewModel.Password, oneTimePassword);
                    if (confirmPasswordResponse.Success)
                    {
                        return View("PasswordChanged");
                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMessage", "Error in resetting password");
                        return View("ConfirmPassword", confirmPasswordViewModel);
                    }
                }
                else
                {
                    confirmPasswordResponse = await _signUpService.ConfirmPassword(email, confirmPasswordViewModel.Password, oneTimePassword);
                    if (confirmPasswordResponse.Success)
                    {
                        HttpContext?.Session.Set("MFARegistrationViewModel", new MFARegistrationViewModel { Email = email, Password = confirmPasswordViewModel.Password, SecretToken = confirmPasswordResponse.Data });
                        return RedirectToAction("MFADescription", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMessage", confirmPasswordResponse.ErrorMessage);
                        return View("CreatePassword", confirmPasswordViewModel);
                    }
                }

                
            }
            else
            {
                return View("CreatePassword", confirmPasswordViewModel);
            }
        }

        [HttpGet("mfa-description")]
        public IActionResult MFADescription()
        {
            return View();
        }

        [HttpGet("mfa-registration")]
        public IActionResult MFARegistration()
        {
            MFARegistrationViewModel MFARegistrationViewModel = HttpContext?.Session.Get<MFARegistrationViewModel>("MFARegistrationViewModel");
            return View("MFARegistration", MFARegistrationViewModel);
        }

        [HttpPost("mfa-confirmation")]
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
                    viewModel.SecretToken = MFARegistrationViewModel.SecretToken;
                    viewModel.Email = MFARegistrationViewModel.Email;
                    ModelState.AddModelError("MFACode", "Enter a valid MFA code");
                    return View("MFARegistration", viewModel);
                }
            }
            else
            {
                viewModel.SecretToken = MFARegistrationViewModel.SecretToken;
                viewModel.Email = MFARegistrationViewModel.Email;
                return View("MFARegistration", viewModel);
            }

        }




        [HttpGet("sign-up-complete")]
        public IActionResult SignUpComplete()
        {
            return View("SignUpComplete");
        }

        #endregion

        #region Login
        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return View("LoginPage");
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginToAccountAsync(LoginViewModel loginViewModel)
        {

            if (ModelState["Email"].Errors.Count ==0 && ModelState["Password"].Errors.Count ==0)
            {
                var loginResponse = await _signUpService.SignInAndWaitForMfa(loginViewModel.Email, loginViewModel.Password);
                if (loginResponse!= null && loginResponse.Length > 0 && loginResponse != Constants.IncorrectPassword)
                {
                    HttpContext?.Session.Set("Email", loginViewModel.Email);
                    HttpContext?.Session.Set("Session", loginResponse);
                    return RedirectToAction("MFAConfirmation");
                }
                else
                {
                    if (loginResponse == Constants.IncorrectPassword)
                    {
                        ModelState.AddModelError("Password", Constants.IncorrectLoginDetails);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", Constants.IncorrectLoginDetails);
                    }

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

        [HttpPost("mfa-confirmation-login")]
        public async Task<IActionResult> ConfirmMFACodeLogin(MFACodeViewModel MFACodeViewModel)
        {
            if (ModelState["MFACode"].Errors.Count == 0)
            {
                var mfaResponse = await _signUpService.ConfirmMFAToken(HttpContext?.Session.Get<string>("Session"), HttpContext?.Session.Get<string>("Email"), MFACodeViewModel.MFACode);

                if (mfaResponse != null && mfaResponse.IdToken.Length > 0)
                {
                    HttpContext?.Session.Set("IdToken", mfaResponse.IdToken);
                    HttpContext?.Session.Set("AccessToken", mfaResponse.AccessToken);
                    return RedirectToAction("LandingPage", "Cab");
                }
                else
                {
                    ModelState.AddModelError("MFACode", "Enter a valid MFA code");
                    return View("MFAConfirmation", MFACodeViewModel);
                }
            }
            else
            {
                return View("MFAConfirmation");
            }
        }



        #endregion

        [HttpGet("sign-out")]
        public async Task<IActionResult> CabSignOut()
        {
            string accesstoken = HttpContext?.Session.Get<string>("AccessToken");
            HttpContext?.Session.Clear();
            _signUpService.SignOut(accesstoken);        
            return RedirectToAction("LoginPage", "Login");
        }
    }
}
