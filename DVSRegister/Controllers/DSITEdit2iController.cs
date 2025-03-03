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

        public DSITEdit2iController(IJwtService jwtService, IDSITEdit2iService dSITEdit2IService, IBucketService bucketService)
        {
            this.jwtService = jwtService;
            this.dSITEdit2IService = dSITEdit2IService;
            this.bucketService = bucketService;
        }

        [HttpGet("provider-changes")]
        public async Task<IActionResult> ProviderChanges(string token)
        {
            ProviderDraftTokenDto providerDraftTokenDto = new();
            ProviderReviewViewModel providerReviewViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

                    if (providerDraftTokenDto == null || providerDraftTokenDto.ProviderProfileDraft == null || providerDraftTokenDto.ProviderProfileDraft.Provider == null ||
                    (providerDraftTokenDto.ProviderProfileDraft.Provider != null && providerDraftTokenDto.ProviderProfileDraft.Provider.ProviderStatus != ProviderStatusEnum.UpdatesRequested))
                    {
                        return RedirectToAction("ProviderChangesError");
                    }

                    providerReviewViewModel.token = tokenDetails.Token;
                    var (previous, current) = dSITEdit2IService.GetProviderKeyValue(providerDraftTokenDto.ProviderProfileDraft, providerDraftTokenDto.ProviderProfileDraft.Provider);
                    providerReviewViewModel.PreviousDataKeyValuePair = previous;
                    providerReviewViewModel.CurrentDataKeyValuePair = current;
                }
                else
                {
                    return RedirectToAction("ProviderChangesAlreadyApproved");
                }
            }
            else
            {
                return RedirectToAction("ProviderChangesError");
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
                            //Token  removed whhen draft is deleted after update
                            return RedirectToAction("ProviderChangesApproved");
                        }
                        else
                        {
                            return RedirectToAction("ProviderChangesError");
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
                            return RedirectToAction("ProviderChangesError");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ProviderChangesError");
                    }
                }
                else
                {
                    await dSITEdit2IService.RemoveProviderDraftToken(tokenDetails.Token, tokenDetails.TokenId);
                    return RedirectToAction("ProviderChangesError");
                }
            }
            else
            {
                return RedirectToAction("ProviderChangesError");
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

        [HttpGet("provider-changes/already-approved")]
        public IActionResult ProviderChangesAlreadyApproved()
        {
            return View();
        }

        [HttpGet("provider-changes/error")]
        public IActionResult ProviderChangesError()
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
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    serviceDraftTokenDto = await dSITEdit2IService.GetServiceChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

                    if (serviceDraftTokenDto == null || serviceDraftTokenDto.ServiceDraft == null || serviceDraftTokenDto.ServiceDraft.Service == null ||
                    (serviceDraftTokenDto.ServiceDraft.Service != null && serviceDraftTokenDto.ServiceDraft.Service.ServiceStatus != ServiceStatusEnum.UpdatesRequested))
                    {
                        return RedirectToAction("ServiceChangesError");
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
                    return RedirectToAction("ServiceChangesAlreadyApproved");
                }
            }
            else
            {
                return RedirectToAction("ServiceChangesError");
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
                        GenericResponse genericResponse = await dSITEdit2IService.UpdateServiceStatusAndData(serviceDraftTokenDto.ServiceDraft.ServiceId, serviceDraftTokenDto.ServiceDraftId);
                        if (genericResponse.Success)
                        {
                           //Token will be removed whhen draft is deleted after update
                            return RedirectToAction("ServiceChangesApproved");
                        }
                        else
                        {
                            return RedirectToAction("ServiceChangesError");
                        }
                    }
                    else if (action == "cancel")
                    {
                        GenericResponse genericResponse = await dSITEdit2IService.CancelServiceUpdates(serviceDraftTokenDto.ServiceDraft.ServiceId, serviceDraftTokenDto.ServiceDraftId);
                        if (genericResponse.Success)
                        {
                            //Token will be removed when service draft is deleted                          
                            return RedirectToAction("ServiceChangesCancelled");
                        }
                        else
                        {
                            return RedirectToAction("ServiceChangesError");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ServiceChangesError");
                    }
                }
                else
                {
                    await dSITEdit2IService.RemoveServiceDraftToken(tokenDetails.Token, tokenDetails.TokenId);
                    return RedirectToAction("ServiceChangesError");
                }
            }
            else
            {
                return RedirectToAction("ServiceChangesError");
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

        [HttpGet("service-changes/already-approved")]
        public IActionResult ServiceChangesAlreadyApproved()
        {
            return View();
        }

        [HttpGet("service-changes/error")]
        public IActionResult ServiceChangesError()
        {
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
                    return RedirectToAction("ServiceChangesError");
                }
                string contentType = "application/octet-stream";
                return File(fileContent, contentType, filename);
            }
            catch (Exception)
            {
                return RedirectToAction("ServiceChangesError");
            }
        }
    }
}
