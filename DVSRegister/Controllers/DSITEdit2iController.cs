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
    [Route("update-request")]
    public class DSITEdit2iController : Controller
    {
        private readonly IJwtService jwtService;
        private readonly IDSITEdit2iService dSITEdit2IService;
        private readonly IBucketService bucketService;
        private readonly ILogger<DSITEdit2iController> _logger;


        public DSITEdit2iController(IJwtService jwtService, IDSITEdit2iService dSITEdit2IService, IBucketService bucketService, ILogger<DSITEdit2iController> logger)
        {
            this.jwtService = jwtService;
            this.dSITEdit2IService = dSITEdit2IService;
            this.bucketService = bucketService;
            _logger = logger;
        }

        [HttpGet("provider-changes")]
        public async Task<IActionResult> ProviderChanges(string token)
        {
            ProviderDraftTokenDto providerDraftTokenDto = new();
            ProviderReviewViewModel providerReviewViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
                if (tokenDetails != null)
                {
                    if (tokenDetails.IsAuthorised)
                    {
                        providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

                        if (providerDraftTokenDto == null || providerDraftTokenDto.ProviderProfileDraft == null || providerDraftTokenDto.ProviderProfileDraft.Provider == null ||
                        (providerDraftTokenDto.ProviderProfileDraft.Provider != null && providerDraftTokenDto.ProviderProfileDraft.Provider.ProviderStatus != ProviderStatusEnum.UpdatesRequested))
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes were already approved."));
                            return RedirectToAction("UpdatesAlreadyApproved");
                        }

                        providerReviewViewModel.token = tokenDetails.Token;
                        providerReviewViewModel.PreviousProviderData = providerDraftTokenDto.ProviderProfileDraft.Provider;
                        var (previous, current) = dSITEdit2IService.GetProviderKeyValue(providerDraftTokenDto.ProviderProfileDraft, providerDraftTokenDto.ProviderProfileDraft.Provider);
                        providerReviewViewModel.PreviousDataKeyValuePair = previous;
                        providerReviewViewModel.CurrentDataKeyValuePair = current;
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Unauthorised access attempt for provider changes."));
                        return RedirectToAction("URLExpiredError");
                    }
                   
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid token details retrieved."));
                    return RedirectToAction("UpdatesError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("No token provided for provider changes request."));
                return RedirectToAction("UpdatesError");
            }

            return View(providerReviewViewModel);
        }



        [HttpPost("provider-changes")]
        public async Task<IActionResult> ApproveProviderChanges(ProviderReviewViewModel providerReviewViewModel, string action)
        {

            if (!string.IsNullOrEmpty(providerReviewViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(providerReviewViewModel.token, "DSIT");
                ProviderDraftTokenDto providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    if (action == "approve")
                    {
                        GenericResponse genericResponse = await dSITEdit2IService.UpdateProviderAndServiceStatusAndData(providerDraftTokenDto.ProviderProfileDraft);
                        if (genericResponse.Success)
                        {
                            //Token  removed when draft is deleted after update
                            return RedirectToAction("ProviderChangesApproved");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to update provider and service status."));
                            return RedirectToAction("UpdatesError");
                        }
                    }
                    else if (action == "cancel")
                    {
                        GenericResponse genericResponse = await dSITEdit2IService.CancelProviderUpdates(providerDraftTokenDto.ProviderProfileDraft);
                        if (genericResponse.Success)
                        {
                            //Token removed when draft is deleted after cancel
                            return RedirectToAction("ProviderChangesCancelled");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to cancel provider updates."));
                            return RedirectToAction("UpdatesError");
                        }
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Invalid action provided."));
                        return RedirectToAction("UpdatesError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Unauthorised provider removal attempt."));
                    await dSITEdit2IService.RemoveProviderDraftToken(tokenDetails.Token, tokenDetails.TokenId);
                    return RedirectToAction("UpdatesError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: No token provided for provider changes."));
                return RedirectToAction("UpdatesError");
            }

        }

        [HttpGet("provider-changes/approved")]

        public IActionResult ProviderChangesApproved()
        {
            return View();
        }

        [HttpGet("provider-changes/cancelled")]
        public IActionResult ProviderChangesCancelled()
        {
            return View();
        }

      

        [HttpGet("service-changes")]
        public async Task<IActionResult> ServiceChanges(string token)
        {
            ServiceDraftTokenDto serviceDraftTokenDto = new();
            ServiceReviewViewModel serviceReviewViewModel = new();   
            if (!string.IsNullOrEmpty(token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
                if (tokenDetails != null )
                {
                    if(tokenDetails.IsAuthorised)
                    {
                        serviceDraftTokenDto = await dSITEdit2IService.GetServiceChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

                        if (serviceDraftTokenDto == null || serviceDraftTokenDto.ServiceDraft == null || serviceDraftTokenDto.ServiceDraft.Service == null ||
                        (serviceDraftTokenDto.ServiceDraft.Service != null && serviceDraftTokenDto.ServiceDraft.Service.ServiceStatus != ServiceStatusEnum.UpdatesRequested))
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ServiceChanges: Attempted to access already approved or invalid service changes."));
                            return RedirectToAction("UpdatesAlreadyApproved");
                        }

                        serviceReviewViewModel.token = tokenDetails.Token;
                        serviceReviewViewModel.CurrentServiceData = serviceDraftTokenDto.ServiceDraft;
                        serviceReviewViewModel.PreviousServiceData = serviceDraftTokenDto.ServiceDraft.Service;
                        var (previous, current) = dSITEdit2IService.GetServiceKeyValue(serviceDraftTokenDto.ServiceDraft, serviceDraftTokenDto.ServiceDraft.Service);
                        serviceReviewViewModel.PreviousDataKeyValuePair = previous;
                        serviceReviewViewModel.CurrentDataKeyValuePair = current;
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ServiceChanges: Unauthorised access attempt."));
                        return RedirectToAction("URLExpiredError");
                    }
                   
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ServiceChanges: Invalid or expired token provided."));
                    return RedirectToAction("UpdatesError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ServiceChanges: No token provided for service changes."));
                return RedirectToAction("UpdatesError");
            }

            return View(serviceReviewViewModel);
        }


        [HttpPost("service-changes")]
        public async Task<IActionResult> ApproveServiceChanges(ServiceReviewViewModel serviceReviewViewModel, string action)
        {

            if (!string.IsNullOrEmpty(serviceReviewViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(serviceReviewViewModel.token, "DSIT");
                ServiceDraftTokenDto serviceDraftTokenDto = await dSITEdit2IService.GetServiceChangesByToken(tokenDetails.Token, tokenDetails.TokenId);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    if (action == "approve")
                    {
                        GenericResponse genericResponse = await dSITEdit2IService.UpdateServiceStatusAndData(serviceDraftTokenDto.ServiceDraft);
                        if (genericResponse.Success)
                        {
                           //Token will be removed when draft is deleted after update
                            return RedirectToAction("ServiceChangesApproved");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Failed to update service status."));
                            return RedirectToAction("UpdatesError");
                        }
                    }
                    else if (action == "cancel")
                    {
                        GenericResponse genericResponse = await dSITEdit2IService.CancelServiceUpdates(serviceDraftTokenDto.ServiceDraft);
                        if (genericResponse.Success)
                        {
                            //Token will be removed when service draft is deleted                          
                            return RedirectToAction("ServiceChangesCancelled");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Failed to cancel service updates."));
                            return RedirectToAction("UpdatesError");
                        }
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Invalid action provided."));
                        return RedirectToAction("UpdatesError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: Unauthorised attempt with invalid token."));
                    await dSITEdit2IService.RemoveServiceDraftToken(tokenDetails.Token, tokenDetails.TokenId);
                    return RedirectToAction("UpdatesError");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveServiceChanges: No token provided for service changes."));
                return RedirectToAction("UpdatesError");
            }

        }


        [HttpGet("service-changes/approved")]
        public IActionResult ServiceChangesApproved()
        {
            return View();
        }


        [HttpGet("service-changes/cancelled")]
        public IActionResult ServiceChangesCancelled()
        {
            return View();
        }



        [HttpGet("changes/already-approved")]
        public IActionResult UpdatesAlreadyApproved()
        {
            _logger.LogInformation("UpdatesAlreadyApproved: A request was made for already approved updates.");
            return View();
        }

        [HttpGet("changes/error")]
        public IActionResult UpdatesError()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("UpdatesError: An error occurred during the provider/service update process."));
            return View();
        }
        [HttpGet("changes/url-expired")]
        public IActionResult URLExpiredError()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("URLExpiredError: A request was made using an expired or invalid URL."));
            return View();
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
