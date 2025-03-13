using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("consent")]
    public class ConsentController : Controller
    {
        private readonly IJwtService jwtService;
        
        private readonly IConsentService consentService;
        private readonly ILogger<ConsentController> _logger;
        public ConsentController(IJwtService jwtService, IConsentService consentService, ILogger<ConsentController> logger)
        {
            this.jwtService = jwtService;            
            this.consentService = consentService;
            _logger = logger;
        }


        #region Opening Loop

        [HttpGet("proceed-application-consent")]
        public async Task<ActionResult> ProceedApplicationConsent(string token)
        {
            ConsentViewModel consentViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                consentViewModel.token = token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);
                    if (serviceDto == null || serviceDto?.ServiceStatus == ServiceStatusEnum.Received || serviceDto?.CertificateReview?.CertificateReviewStatus != CertificateReviewEnum.Approved)
                    {                       
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationConsent failed: Service is not approved."));
                        return RedirectToAction("ProceedApplicationConsentError");
                    }
                    consentViewModel.Service = serviceDto;
                }
                else
                {                    
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationConsent failed: Token is invalid or unauthorised."));
                    return RedirectToAction("ProceedApplicationConsentError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationConsent failed: Token is missing."));
                return RedirectToAction("ProceedApplicationConsentError");
            }
            return View(consentViewModel);
        }

        [HttpPost("proceed-application-consent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProceedApplicationGiveConsent(ConsentViewModel consentViewModel)
        {
            string email = "";
            if (!string.IsNullOrEmpty(consentViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);

                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);                  
                    email = string.IsNullOrEmpty(email) ? serviceDto?.Provider.PrimaryContactEmail + ";"+ serviceDto?.Provider.SecondaryContactEmail : email;

                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await consentService.UpdateServiceStatus(serviceDto.Id, email, serviceDto?.Provider?.RegisteredName??string.Empty, serviceDto?.ServiceName??string.Empty);
                        if (genericResponse.Success)
                        {
                            await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                            return RedirectToAction("ProceedApplicationConsentSuccess");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Unable to update service status."));
                            return RedirectToAction("ProceedApplicationConsentError");
                        }
                    }
                    else
                    {

                        consentViewModel.Service = serviceDto;
                        return View("ProceedApplicationConsent", consentViewModel);
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Token is invalid or unauthorised."));
                    await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                    return RedirectToAction("ProceedApplicationConsentError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Token is missing."));
                return RedirectToAction("ProceedApplicationConsentError");
            }

        }

        [HttpGet("proceed-application-consent-success")]
        public ActionResult ProceedApplicationConsentSuccess()
        {
            return View();
        }

        [HttpGet("proceed-application-consent-error")]
        public ActionResult ProceedApplicationConsentError()
        {
            return View();
        }
        #endregion


        #region Closing the loop        

        [HttpGet("publish-service-give-consent")]
        public  async Task<ActionResult> Consent(string token)
        {
            ConsentViewModel consentViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                consentViewModel.token =  token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token);
                if (tokenDetails!= null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByConsentToken(tokenDetails.Token, tokenDetails.TokenId);
                    if(serviceDto== null || serviceDto?.ServiceStatus == ServiceStatusEnum.ReadyToPublish)
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Consent failed: Service is already ready to publish."));
                        return RedirectToAction("ConsentErrorAlreadyAgreed");
                    }
                    consentViewModel.Service = serviceDto;
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Consent failed: Token is invalid or expired."));
                    return RedirectToAction("ConsentErrorURLExpired");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Consent failed: Token is missing."));
                return RedirectToAction("ConsentError");
            }
               
           
            return View(consentViewModel);
        }

        [HttpPost("publish-service-give-consent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GiveConsent(ConsentViewModel consentViewModel)
        {
            string email = "";
            if (!string.IsNullOrEmpty(consentViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);
               
                if(tokenDetails!= null && tokenDetails.IsAuthorised)
                {
                    ServiceDto ServiceDto = await consentService.GetProviderAndCertificateDetailsByConsentToken(tokenDetails.Token, tokenDetails.TokenId);
                    email = string.IsNullOrEmpty(email) ? ServiceDto.Provider.PrimaryContactEmail + ";"+ ServiceDto.Provider.SecondaryContactEmail : email;
                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await consentService.UpdateServiceAndProviderStatus(tokenDetails.Token, tokenDetails.TokenId, ServiceDto,email);
                        if (genericResponse.Success)
                        {
                            await consentService.RemoveConsentToken(tokenDetails.Token, tokenDetails.TokenId,email);
                            return RedirectToAction("ConsentSuccess");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("GiveConsent failed: Unable to update service/provider status."));
                            return RedirectToAction("ConsentError");
                        } 
                    }
                    else
                    {
                       
                        consentViewModel.Service = ServiceDto;
                        return View("Consent", consentViewModel);
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("GiveConsent failed: Token is invalid or unauthorised."));
                    await consentService.RemoveConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                    return RedirectToAction("ConsentErrorURLExpired");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("GiveConsent failed: Token is missing."));
                return RedirectToAction("ConsentError");
            }         
            
        }

        [HttpGet("consent-success")]
        public ActionResult ConsentSuccess()
        {
            return View();
        }

        [HttpGet("consent-error-already-agreed")]
        public ActionResult ConsentErrorAlreadyAgreed()
        {
            return View();
        }

        [HttpGet("consent-error")]
        public ActionResult ConsentError()
        {
            return View();
        }

        [HttpGet("consent-error-url-expired")]
        public ActionResult ConsentErrorURLExpired()
        {
            return View();
        }


        #endregion




    }
}
