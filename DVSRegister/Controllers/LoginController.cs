using DVSAdmin.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DVSRegister.Controllers
{
    [Route("cab-application-registration")]
    public class LoginController : Controller
    {
        private readonly ISignUpService signUpService ;
        private readonly IUserService userService ;
        private readonly IWebHostEnvironment webHostEnvironment ;
        private readonly CognitoClient cognitoClient;
        private readonly LoginEmailSender emailSender;

        public LoginController(ISignUpService signUpService, IUserService userService, IWebHostEnvironment webHostEnvironment , CognitoClient cognitoClient, LoginEmailSender emailSender)
        {
            this.signUpService = signUpService;
            this.userService = userService;
            this.webHostEnvironment = webHostEnvironment;
            this.cognitoClient = cognitoClient;
            this.emailSender = emailSender;
        }
        [HttpGet("")]
        public IActionResult StartPage()
        {
            return View("StartPage");
        }

        [HttpGet("account-not-found")]
        public IActionResult StartPageWithBanner()
        {
            return View();
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
            if (!ViewModelHelper.IsValidEmail(enterEmailViewModel.Email, webHostEnvironment))
            {
                ModelState.AddModelError(nameof(enterEmailViewModel.Email), Constants.EmailErrorMessage);
            }
            if (ModelState["Email"]?.Errors.Count == 0)
            {
                var forgotPasswordResponse = await signUpService.ForgotPassword(enterEmailViewModel.Email);

                if (forgotPasswordResponse == "OK")
                {
                    HttpContext.Session?.Set("Email", enterEmailViewModel.Email.ToLower());
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
            string Email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            EnterCodeViewModel enterCodeViewModel = new EnterCodeViewModel();
            enterCodeViewModel.Email = Email;
            enterCodeViewModel.PasswordReset = passwordReset;
            return View("EnterCode", enterCodeViewModel);
        }

        [HttpPost("email-code-validation")]
        public IActionResult EnterCodeValidation(EnterCodeViewModel enterCodeViewModel)
        {
            if (ModelState["Code"]?.Errors.Count  == 0)
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
            if (ModelState["Password"]?.Errors.Count == 0 && ModelState["ConfirmPassword"]?.Errors.Count == 0)
            {
                string email = HttpContext?.Session.Get<string>("Email") ?? string.Empty;
                string oneTimePassword = HttpContext?.Session.Get<string>("Code") ?? string.Empty;
                GenericResponse confirmPasswordResponse = new GenericResponse();
                if (confirmPasswordViewModel.PasswordReset != null && confirmPasswordViewModel.PasswordReset==true)
                {
                    confirmPasswordResponse = await signUpService.ResetPassword(email, confirmPasswordViewModel.Password??string.Empty, oneTimePassword);
                    if (confirmPasswordResponse.Success)
                    {
                        if (confirmPasswordViewModel.PasswordReset == true)

                            await emailSender.SendEmailCabPasswordReset(email, email, Helper.GetLocalDateTime(DateTime.UtcNow, "dd MMM yyyy h:mm tt"));
                            
                        return View("PasswordChanged");
                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMessage", "Error in resetting password");
                        return View("CreatePassword", confirmPasswordViewModel);
                    }
                }
                else
                {
                    confirmPasswordResponse = await signUpService.ConfirmPassword(email, confirmPasswordViewModel.Password ?? string.Empty, oneTimePassword);
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
            MFARegistrationViewModel MFARegistrationViewModel = HttpContext?.Session.Get<MFARegistrationViewModel>("MFARegistrationViewModel")!;
            return View("MFARegistration", MFARegistrationViewModel);
        }

        [HttpPost("mfa-confirmation")]
        public async Task<IActionResult> MFAConfirmationCheck(MFARegistrationViewModel viewModel)
        {
            MFARegistrationViewModel MFARegistrationViewModel = HttpContext?.Session.Get<MFARegistrationViewModel>("MFARegistrationViewModel")!;
            if (ModelState["MFACode"]?.Errors.Count == 0)
            {
                var mfaConfirmationCheckResponse = await signUpService.MFAConfirmation(MFARegistrationViewModel.Email?? string.Empty, MFARegistrationViewModel.Password?? string.Empty, viewModel.MFACode);

                if (mfaConfirmationCheckResponse == "OK")
                {
                    return View("SignUpComplete");
                }
                else
                {
                    viewModel.SecretToken = MFARegistrationViewModel.SecretToken;
                    viewModel.Email = MFARegistrationViewModel.Email;
                    ModelState.AddModelError("MFACode", "The code you entered is not correct, or may have expired, try entering it again or request a new code");
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

            if (!ViewModelHelper.IsValidEmail(loginViewModel.Email, webHostEnvironment))
            {
                ModelState.AddModelError(nameof(loginViewModel.Email), Constants.EmailErrorMessage);
            }
            if (ModelState["Email"]?.Errors.Count ==0 && ModelState["Password"]?.Errors.Count ==0)
            {
                var loginResponse = await signUpService.SignInAndWaitForMfa(loginViewModel.Email ?? string.Empty, loginViewModel.Password);
                if (loginResponse!= null && loginResponse.Length > 0 && loginResponse != Constants.IncorrectLoginDetails && loginResponse != Constants.UserDisabled)
                {
                    HttpContext?.Session.Set("Email", loginViewModel.Email);
                    HttpContext?.Session.Set("Session", loginResponse);
                    return RedirectToAction("MFAConfirmation");
                }
                else if (loginResponse == Constants.UserDisabled)
                {
                    ModelState.AddModelError("Email", loginResponse);
                    return View("LoginPage", loginViewModel);
                }
                else
                {
                    ModelState.AddModelError("Email", Constants.IncorrectLoginDetails);
                    return View("LoginPage", loginViewModel);
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
            if (ModelState["MFACode"]?.Errors.Count == 0)
            {
                var mfaResponse = await signUpService.ConfirmMFAToken(HttpContext?.Session.Get<string>("Session"), HttpContext?.Session.Get<string>("Email"), MFACodeViewModel.MFACode);

                if (mfaResponse != null && mfaResponse.IdToken.Length > 0)
                {
                    HttpContext?.Session.Set("IdToken", mfaResponse.IdToken);
                    HttpContext?.Session.Set("AccessToken", mfaResponse.AccessToken);

                    Task<TokenValidationResult> result = TokenExtensions.ValidateToken(mfaResponse.IdToken, cognitoClient._userPoolId, cognitoClient._region, cognitoClient._clientId);
                    string? cab = result.Result?.Claims?.FirstOrDefault(c => c.Key == "profile").Value?.ToString();
                    string email = HttpContext?.Session.Get<string>("Email");

                    if (!string.IsNullOrEmpty(cab) && !string.IsNullOrEmpty(email))
                    {                      
                        CabUserDto cabUser = await userService.UpdateCabUser(email);                        
                        if(cabUser.CabId>0)                            
                        HttpContext?.Session.Set("CabId", cabUser.CabId); // setting logged in cab id in session
                        return RedirectToAction("DraftApplications", "Home");
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid email or cab.");
                    }
                }
                else
                {
                    ModelState.AddModelError("MFACode", "The code you entered is not correct, or may have expired, try entering it again or start again");
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
            string accesstoken = HttpContext?.Session.Get<string>("AccessToken")??string.Empty;
            HttpContext?.Session.Clear();
            signUpService.SignOut(accesstoken);        
            return RedirectToAction("LoginPage", "Login");
        }
    }
}