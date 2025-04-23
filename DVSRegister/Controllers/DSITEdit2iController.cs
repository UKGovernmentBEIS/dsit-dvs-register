using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("update-request")]
    public class DSITEdit2iController(IJwtService jwtService, IDSITEdit2iService dSITEdit2IService, IBucketService bucketService, ILogger<DSITEdit2iController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;
        private readonly IDSITEdit2iService dSITEdit2IService = dSITEdit2IService;
        private readonly IBucketService bucketService = bucketService;
        private readonly ILogger<DSITEdit2iController> _logger = logger;

        [HttpGet("provider-changes")]
        public async Task<IActionResult> ProviderChanges(string token)
        {
            ProviderReviewViewModel providerReviewViewModel = new();           
            TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
            ProviderDraftTokenDto providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidProviderUpdateRequest(tokenDetails, providerDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                providerReviewViewModel.token = tokenDetails.Token;
                providerReviewViewModel.PreviousProviderData = providerDraftTokenDto.ProviderProfileDraft.Provider;
                var (previous, current) = dSITEdit2IService.GetProviderKeyValue(providerDraftTokenDto.ProviderProfileDraft, providerDraftTokenDto.ProviderProfileDraft.Provider);
                providerReviewViewModel.PreviousDataKeyValuePair = previous;
                providerReviewViewModel.CurrentDataKeyValuePair = current;
                return View(providerReviewViewModel);

            }

            
        }



        [HttpPost("provider-changes")]
        public async Task<IActionResult> ApproveProviderChanges(ProviderReviewViewModel providerReviewViewModel, string action)
        {
            TokenDetails tokenDetails = await jwtService.ValidateToken(providerReviewViewModel.token, "DSIT");
            ProviderDraftTokenDto providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId); 
            var invalidRequestResult = await HandleInvalidProviderUpdateRequest(tokenDetails, providerDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "approve")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.UpdateProviderAndServiceStatusAndData(providerDraftTokenDto.ProviderProfileDraft);
                    if (genericResponse.Success)
                    {
                        //Token  removed when draft is deleted after update
                        return View("ProviderChangesApproved");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to update provider and service status."));
                        return View("UpdatesError");
                    }
                }
                else if (action == "cancel")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.CancelProviderUpdates(providerDraftTokenDto.ProviderProfileDraft);
                    if (genericResponse.Success)
                    {
                        //Token removed when draft is deleted after cancel
                        return View("ProviderChangesCancelled");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to cancel provider updates."));
                        return View("UpdatesError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Invalid action provided."));
                    return View("UpdatesError");
                }

            }
            
        }




        private async Task<ActionResult?> HandleInvalidProviderUpdateRequest(TokenDetails tokenDetails, ProviderDraftTokenDto? providerDraftTokenDto)
        {
            TokenStatusEnum tokenStatus = await dSITEdit2IService.GetEditProviderTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes: token expired"));
                return View("URLExpiredError");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes:Invalid token"));
                return View("UpdatesError");
            }
             //to do : resent logic
            else if (tokenStatus == TokenStatusEnum.RequestCompleted || tokenStatus == TokenStatusEnum.UserCancelled)
            {
                return View("UpdatesAlreadyApproved");
            }
            else if (providerDraftTokenDto == null || providerDraftTokenDto.ProviderProfileDraft == null || providerDraftTokenDto.ProviderProfileDraft.Provider == null ||
             (providerDraftTokenDto.ProviderProfileDraft.Provider != null && providerDraftTokenDto.ProviderProfileDraft.Provider.ProviderStatus != ProviderStatusEnum.UpdatesRequested))
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes were already approved."));
                return View("UpdatesAlreadyApproved");
            }
            else
            {
                return null;
            }
        }



        [HttpGet("service-changes")]
        public async Task<IActionResult> ServiceChanges(string token)
        {
                   
            ServiceReviewViewModel serviceReviewViewModel = new();
            TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
            ServiceDraftTokenDto serviceDraftTokenDto = await dSITEdit2IService.GetServiceChangesByToken(tokenDetails.Token, tokenDetails.TokenId);
                        
            var invalidRequestResult = await HandleInvalidServiceUpdateRequest(tokenDetails, serviceDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                serviceReviewViewModel.token = tokenDetails.Token;
                serviceReviewViewModel.CurrentServiceData = serviceDraftTokenDto.ServiceDraft;
                serviceReviewViewModel.PreviousServiceData = serviceDraftTokenDto.ServiceDraft.Service;
                var (previous, current) = dSITEdit2IService.GetServiceKeyValue(serviceDraftTokenDto.ServiceDraft, serviceDraftTokenDto.ServiceDraft.Service);
                serviceReviewViewModel.PreviousDataKeyValuePair = previous;
                serviceReviewViewModel.CurrentDataKeyValuePair = current;
                return View(serviceReviewViewModel);
            }
          
        }
       


        [HttpPost("service-changes")]
        public async Task<IActionResult> ApproveServiceChanges(ServiceReviewViewModel serviceReviewViewModel, string action)
        {
           
            TokenDetails tokenDetails = await jwtService.ValidateToken(serviceReviewViewModel.token, "DSIT");
            ServiceDraftTokenDto serviceDraftTokenDto = await dSITEdit2IService.GetServiceChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidServiceUpdateRequest(tokenDetails, serviceDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "approve")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.UpdateServiceStatusAndData(serviceDraftTokenDto.ServiceDraft);
                    if (genericResponse.Success)
                    {
                        //Token will be removed when draft is deleted after update
                        return View("ServiceChangesApproved");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Failed to update service status."));
                        return View("UpdatesError");
                    }
                }
                else if (action == "cancel")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.CancelServiceUpdates(serviceDraftTokenDto.ServiceDraft);
                    if (genericResponse.Success)
                    {
                        //Token will be removed when service draft is deleted                          
                        return View("ServiceChangesCancelled");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Failed to cancel service updates."));
                        return View("UpdatesError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Invalid action provided."));
                    return View("UpdatesError");
                }

            }
           

        }

        private async Task<ActionResult?> HandleInvalidServiceUpdateRequest(TokenDetails tokenDetails, ServiceDraftTokenDto? serviceDraftTokenDto)
        {
            TokenStatusEnum tokenStatus = await dSITEdit2IService.GetEditServiceTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Service changes: token expired"));
                return View("URLExpiredError");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Service changes:Invalid token"));
                return View("UpdatesError");
            }
            else if (tokenStatus == TokenStatusEnum.RequestResent && serviceDraftTokenDto?.ServiceDraft == null)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Opening loop: Request resent"));
                return View("UpdatesError"); // to do : check for new content for request resent
            }
            else if (tokenStatus == TokenStatusEnum.RequestCompleted || tokenStatus == TokenStatusEnum.UserCancelled)
            {
                return View("UpdatesAlreadyApproved");
            }
            else if (serviceDraftTokenDto?.ServiceDraft == null || serviceDraftTokenDto.ServiceDraft.Service == null ||
                         (serviceDraftTokenDto.ServiceDraft.Service != null && serviceDraftTokenDto.ServiceDraft.Service.ServiceStatus != ServiceStatusEnum.UpdatesRequested))
            {// for old tokens to work
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Service changes already approved"));
                return View("UpdatesAlreadyApproved");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Download from s3
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        [HttpGet("download-certificate")]
        public async Task<IActionResult> DownloadCertificate(string key, string filename)
        {
            try
            {
                byte[]? fileContent = await bucketService.DownloadFileAsync(key);

                if (fileContent == null || fileContent.Length == 0)
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("DownloadCertificate: No file content found for key."));
                    return RedirectToAction("UpdatesError");
                }
                string contentType = "application/octet-stream";
                return File(fileContent, contentType, filename);
            }
            catch (Exception)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("DownloadCertificate: An error occurred while downloading the certificate."));
                return RedirectToAction("UpdatesError");
            }
        }
    }
}