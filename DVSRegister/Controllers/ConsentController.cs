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
    public class ConsentController(IJwtService jwtService, IConsentService consentService, ILogger<ConsentController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;        
        private readonly IConsentService consentService = consentService;
        private readonly ILogger<ConsentController> _logger = logger;


        #region Opening Loop

        [HttpGet("proceed-application-consent")]
        public async Task<ActionResult> ProceedApplicationConsent(string token)
        {
            ConsentViewModel consentViewModel = new()
            {
                token = token
            };
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);
            string email = serviceDto?.Provider.PrimaryContactEmail + ";" + serviceDto?.Provider.SecondaryContactEmail;
            var invalidRequestResult = await HandleOpeningLoopInvalidRequest(tokenDetails, serviceDto,email);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                consentViewModel.Service = serviceDto;
                return View(consentViewModel);
            }
            
         }       


        [HttpPost("proceed-application-consent")]     
        public async Task<ActionResult> ProceedApplicationGiveConsent(ConsentViewModel consentViewModel)
        {            
            
            TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);                  
            string email = serviceDto ==null?string.Empty: serviceDto.Provider.PrimaryContactEmail + ";"+ serviceDto.Provider.SecondaryContactEmail;

                var invalidRequestResult = await HandleOpeningLoopInvalidRequest(tokenDetails, serviceDto, email);
                if (invalidRequestResult != null)
                {
                    return invalidRequestResult;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await consentService.UpdateServiceStatus(serviceDto.Id, email, serviceDto?.Provider?.RegisteredName ?? string.Empty, serviceDto?.ServiceName ?? string.Empty);
                        if (genericResponse.Success)
                        {
                            await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                            return View("ProceedApplicationConsentSuccess");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Unable to update service status."));
                            return View("ProceedApplicationConsentError");
                        }
                    }
                    else
                    {

                        consentViewModel.Service = serviceDto;
                        return View("ProceedApplicationConsent", consentViewModel);
                    }
                }            
               

        }


        private async Task<ActionResult?> HandleOpeningLoopInvalidRequest(TokenDetails tokenDetails, ServiceDto? serviceDto, string email)
        {
            var (openingLoopStatus, _) = await consentService.GetTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop: Token is expired"));
                return View("ProceedApplicationConsentURLExpired");// to do : check for new content
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop:Invalid token"));
                return View("ProceedApplicationConsentError");
            }
            else if (openingLoopStatus == TokenStatusEnum.RequestResent && serviceDto == null)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop: Request resent"));
                return View("ProceedApplicationConsentError"); // to do : check for new content for request resent
            }

            else if (openingLoopStatus == TokenStatusEnum.RequestCompleted)
            {
                return View("ProceedApplicationAlreadyAgreed");
            }
            else if (serviceDto?.ServiceStatus == ServiceStatusEnum.Received || serviceDto?.CertificateReview?.CertificateReviewStatus != CertificateReviewEnum.Approved)
            {//keep this condition for old tokens without a token status
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop:Already consented"));
                return View("ProceedApplicationAlreadyAgreed");
            }
            else
            {
                return null;
            }
        }

       
        #endregion


        #region Closing the loop        

        [HttpGet("publish-service-give-consent")]
        public  async Task<ActionResult> Consent(string token)
        {
            ConsentViewModel consentViewModel = new()
            {
                token = token
            };
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);
            string email = serviceDto?.Provider.PrimaryContactEmail + ";" + serviceDto?.Provider.SecondaryContactEmail;
            var invalidRequestResult = await HandleClosingLoopInvalidRequest(tokenDetails, serviceDto, email);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                consentViewModel.Service = serviceDto;
                return View(consentViewModel);
            }
        }

        [HttpPost("publish-service-give-consent")]
        public async Task<ActionResult> GiveConsent(ConsentViewModel consentViewModel)
        {

            TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);
            string email = serviceDto == null ? string.Empty : serviceDto.Provider.PrimaryContactEmail + ";" + serviceDto.Provider.SecondaryContactEmail;

            var invalidRequestResult = await HandleClosingLoopInvalidRequest(tokenDetails, serviceDto, email);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    GenericResponse genericResponse = await consentService.UpdateServiceAndProviderStatus(tokenDetails.Token, tokenDetails.TokenId, serviceDto, email);
                    if (genericResponse.Success)
                    {
                        await consentService.RemoveConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
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
                    consentViewModel.Service = serviceDto;
                    return View("Consent", consentViewModel);
                }
            }
        }

        private async Task<ActionResult?> HandleClosingLoopInvalidRequest(TokenDetails tokenDetails, ServiceDto? serviceDto, string email)
        {
            var (_, closingLoopStatus) = await consentService.GetTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Closing loop: Token is expired"));
                return View("ConsentErrorURLExpired");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Closing loop:Invalid token"));
                return View("ConsentError");
            }
            else if (closingLoopStatus == TokenStatusEnum.RequestResent && serviceDto == null)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Closing loop: Request resent"));
                return View("ConsentError"); // to do : check for new content for request resent
            }

            else if (closingLoopStatus == TokenStatusEnum.RequestCompleted)
            {
                return View("ConsentErrorAlreadyAgreed");
            }
            else if ( serviceDto?.ServiceStatus == ServiceStatusEnum.ReadyToPublish)
            {//keep this condition for old tokens without a token status
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Closing loop:Already consented"));
                return View("ConsentErrorAlreadyAgreed");
            }
            else
            {
                return null;
            }
        }


        #endregion




    }
}