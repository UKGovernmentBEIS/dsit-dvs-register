using DVSRegister.BusinessLogic.Models;
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
    public class ConsentController(IJwtService jwtService, IConsentService consentService, IActionLogService actionLogService, ILogger<ConsentController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;        
        private readonly IConsentService consentService = consentService;
        private readonly ILogger<ConsentController> _logger = logger;
        private readonly IActionLogService actionLogService = actionLogService;


        #region Opening Loop

        [HttpGet("proceed-application-consent")]
        public async Task<ActionResult> ProceedApplicationConsent(string token)
        {
            ConsentViewModel consentViewModel = new()
            {
                token = token
            };
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByOpeningLoopToken(tokenDetails.Token, tokenDetails.TokenId);           
            var invalidRequestResult = await HandleOpeningLoopInvalidRequest(tokenDetails, serviceDto);
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
        public async Task<ActionResult> ProceedApplicationGiveConsent(ConsentViewModel consentViewModel, string agree)
        {              
            TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);
            ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByOpeningLoopToken(tokenDetails.Token, tokenDetails.TokenId);                  
            string email = serviceDto ==null?string.Empty: serviceDto.Provider.PrimaryContactEmail + ";"+ serviceDto.Provider.SecondaryContactEmail;

                var invalidRequestResult = await HandleOpeningLoopInvalidRequest(tokenDetails, serviceDto);
                if (invalidRequestResult != null)
                {
                    return invalidRequestResult;
                }
                else
                {
                GenericResponse genericResponse = await consentService.UpdateServiceStatus(serviceDto.Id, email, serviceDto?.Provider?.RegisteredName ?? string.Empty, 
                    serviceDto?.ServiceName ?? string.Empty, agree);
                if (genericResponse.Success)
                {
                    await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                    if(agree == "accept")
                    {
                        return View("ProceedApplicationConsentSuccess");
                    }
                    else if(agree == "decline")
                    {
                        await AddActionLog(serviceDto, ActionDetailsEnum.CR_DeclinedByProvider, genericResponse.InstanceId, Constants.InvitationDeclined);
                        return View("ProceedApplicationConsentDeclined");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Invalid action"));
                        return View("ProceedApplicationConsentError");
                    }
                   
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ProceedApplicationGiveConsent failed: Unable to update service status."));
                    return View("ProceedApplicationConsentError");
                }
            }
        }


        private async Task<ActionResult?> HandleOpeningLoopInvalidRequest(TokenDetails tokenDetails, ServiceDto? serviceDto)
        {
            var openingLoopStatus  = await consentService.GetTokenStatus(tokenDetails);
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
            else if (serviceDto?.ServiceStatus == ServiceStatusEnum.Received || serviceDto?.CertificateReview.Where(x=>x.IsLatestReviewVersion).SingleOrDefault()?.CertificateReviewStatus != CertificateReviewEnum.Approved)
            {//keep this condition for old tokens without a token status
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop:Already consented"));
                return View("ProceedApplicationAlreadyAgreed");
            }
            else
            {
                return null;
            }
        }

        private async Task AddActionLog(ServiceDto serviceDto, ActionDetailsEnum actionDetails, int reviewId, string displayMessage)
        {

            ActionLogsDto actionLogsDto = new()
            {
                
                ActionCategoryEnum = ActionCategoryEnum.CR,
                ActionDetailsEnum = actionDetails,
                ServiceId = serviceDto.Id,
                ProviderId = serviceDto.Provider.Id,
                ServiceName = serviceDto.ServiceName,
                ProviderName = serviceDto.Provider.RegisteredName,
                DisplayMessage = displayMessage,
                CertificateReviewId = reviewId
            };


            actionLogsDto.ActionDetailsEnum = actionDetails;
            await actionLogService.SaveActionLogs(actionLogsDto);
        }

        #endregion







    }
}