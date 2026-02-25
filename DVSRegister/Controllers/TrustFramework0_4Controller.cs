using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Models.CabTrustFramework;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("cab-service/submit-service")]
    public class TrustFramework0_4Controller(ITrustFrameworkService trustFrameworkService, ICabService cabService, IUserService userService
        ,IBucketService bucketService, IOptions<S3Configuration> config, IMapper mapper, ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;        
        private readonly ITrustFrameworkService trustFrameworkService = trustFrameworkService;
        private readonly ICabService cabService = cabService;
        private readonly IUserService userService = userService;
        private readonly IBucketService bucketService = bucketService;
        private readonly IMapper mapper = mapper;
        private readonly S3Configuration config = config.Value;

        [HttpGet("tf-version/{providerProfileId}/{fromSummaryPage?}/{fromDetailsPage?}")]
        public async Task<IActionResult> SelectVersionOfTrustFrameWork(int providerProfileId, bool fromSummaryPage=false,  bool fromDetailsPage=false)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            await HandleInvalidProfileAndCab(providerProfileId, cabUserDto);
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            TFVersionViewModel TFVersionViewModel = new()
            {
                SelectedTFVersionId = summaryViewModel?.TFVersionViewModel?.SelectedTFVersion?.Id,
                AvailableVersions = await trustFrameworkService.GetTrustFrameworkVersions(),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() :
                summaryViewModel.IsTFVersionChanged.GetValueOrDefault() && summaryViewModel.IsAmendment ? "/cab-service/amend/service-amendments?serviceId=" + summaryViewModel.ServiceId :
                summaryViewModel.IsTFVersionChanged.GetValueOrDefault() && !summaryViewModel.IsAmendment && summaryViewModel.TFVersionViewModel.FromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel.ServiceKey :
                summaryViewModel.IsTFVersionChanged.GetValueOrDefault() && !summaryViewModel.IsAmendment && summaryViewModel.TFVersionViewModel.FromSummaryPage ? "/cab-service/submit-service/check-your-answers" :
                "/cab-service/submit-service/before-you-start/" + summaryViewModel.ProviderProfileId
            }; 

            return View(TFVersionViewModel);
        }

        [HttpPost("tf-version")]
        public async Task<IActionResult> SaveTFVersion(TFVersionViewModel model, string action)
        {
            var summary = GetServiceSummary() ?? new ServiceSummaryViewModel();
            List<TrustFrameworkVersionDto> availableVersions = await trustFrameworkService.GetTrustFrameworkVersions();
            model.AvailableVersions = availableVersions;
            summary.TFVersionViewModel ??= new TFVersionViewModel();
            model.IsAmendment = summary.IsAmendment;

            if (!ModelState.IsValid)
            {
                return View("SelectVersionOfTrustFrameWork", model);
            }

            bool fromSummaryPage = model.FromSummaryPage;
            bool fromDetailsPage = model.FromDetailsPage;

            var prevVersion = summary.TFVersionViewModel?.SelectedTFVersion?.Version;
            var selected = availableVersions.FirstOrDefault(v => v.Id == model.SelectedTFVersionId);

            summary.TFVersionViewModel = new TFVersionViewModel
            {
                SelectedTFVersion = selected,
                FromDetailsPage = fromDetailsPage,
                FromSummaryPage = fromSummaryPage
            };

            var newVersion = selected?.Version; 
            bool isExistingFlow = fromDetailsPage || fromSummaryPage || model.IsAmendment;

            // If not coming from intermediate flow, go back to the full flow start
            if (!isExistingFlow)
            {
                return RedirectToAction("ServiceName", "CabService");
            }

            // 0) No change → persist and go back to previous page they came from
            // service name page as saftey net
            if (prevVersion == newVersion)
            {
                HttpContext?.Session.Set("ServiceSummary", summary);
                return await HandleActions(action, summary, fromSummaryPage, fromDetailsPage, false, "ServiceName", "CabService");
            }

            // 1) Downgrade to 0.3 (from 0.4 or 1.0)
            if (newVersion == Constants.TFVersion0_3)
            {
                // fields that only exist for >= 0.4
                ViewModelHelper.ClearTFVersion0_4Fields(summary);
                ClearSchemesAndGpg(summary);
                await KeepOnlyRolesSupportedByAsync(summary, newVersion);

                HttpContext?.Session.Set("ServiceSummary", summary);
                return await HandleActions(action, summary, false, fromDetailsPage, false, "ProviderRoles", "CabService");
            }

            // 2) Upgrade from 0.3 to either 0.4 or 1.0
            if (prevVersion == Constants.TFVersion0_3 && newVersion != Constants.TFVersion0_3)
            {
                ClearSchemesAndGpg(summary);
                await KeepOnlyRolesSupportedByAsync(summary, newVersion);

                HttpContext?.Session.Set("ServiceSummary", summary);

                if (newVersion == Constants.TFVersion0_4)
                {
                    return await HandleActions(action, summary, false, fromDetailsPage, false, "ProviderRoles", "CabService");
                }

                return await HandleActions(action, summary, false, fromDetailsPage, false, "TermsOfUseUpload");
            }

            // 3) Upgrade from 0.4 to 1.0
            if (prevVersion == Constants.TFVersion0_4 && newVersion == Constants.TFVersion1_0)
            {
                HttpContext?.Session.Set("ServiceSummary", summary);
                return await HandleActions(action, summary, false, fromDetailsPage, false, "TermsOfUseUpload");
            }

            // 4) Downgrade from 1.0 to 0.4
            if (prevVersion == Constants.TFVersion1_0 && newVersion == Constants.TFVersion0_4)
            {
                summary.TOUFileLink = null;
                summary.TOUFileName = null;
                summary.TOUFileSizeInKb = null;

                HttpContext?.Session.Set("ServiceSummary", summary);
                return await HandleActions(action, summary, fromSummaryPage, fromDetailsPage, false, "TermsOfUseUpload");
            }

            // 5) Fallback: unknown transition go back to normal full flow
            HttpContext?.Session.Set("ServiceSummary", summary);
            return await HandleActions(action, summary, fromSummaryPage, fromDetailsPage, false, "ServiceName", "CabService");
        }

        #region Terms Of Use
        [HttpGet("terms-of-use-upload")]
        public async Task<IActionResult> TermsOfUseUpload(bool fromSummaryPage, bool remove, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            ViewBag.IsTFVersionChanged = summaryViewModel.IsTFVersionChanged;
            var lastScheme = summaryViewModel?.SchemeQualityLevelMapping?.LastOrDefault() ?? null;
            TOUFileViewModel TOUFileViewModel = new()
            {
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/company-address",
                IsAmendment = summaryViewModel.IsAmendment
            };

            if (remove)
            {
                summaryViewModel.TOUFileLink = null;
                summaryViewModel.TOUFileName = null;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                TOUFileViewModel.FileRemoved = true;
                TOUFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
                return View(TOUFileViewModel);
            }
            if (!string.IsNullOrEmpty(summaryViewModel.TOUFileName) && !string.IsNullOrEmpty(summaryViewModel.TOUFileLink))
            {
                TOUFileViewModel.FileName = summaryViewModel.TOUFileName;
                TOUFileViewModel.FileUrl = summaryViewModel.TOUFileLink;
                var fileContent = await bucketService.DownloadFileAsync(summaryViewModel.TOUFileLink, config.TOUBucketName);
                var stream = new MemoryStream(fileContent);
                IFormFile formFile = new FormFile(stream, 0, fileContent.Length, "File", summaryViewModel.TOUFileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };
                TOUFileViewModel.FileUploadedSuccessfully = true;
                TOUFileViewModel.File = formFile;
                TOUFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            }
            return View(TOUFileViewModel);
        }

        [HttpPost("terms-of-use-upload")]
        public async Task<IActionResult> SaveTermsOfUse(TOUFileViewModel TOUFileViewModel, string action)
        {
            bool fromSummaryPage = TOUFileViewModel.FromSummaryPage;
            bool fromDetailsPage = TOUFileViewModel.FromDetailsPage;
            TOUFileViewModel.FromSummaryPage = false;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            TOUFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (Convert.ToBoolean(TOUFileViewModel.FileUploadedSuccessfully) == false)
            {
                if (ModelState["File"].Errors.Count == 0)
                {
                    using var memoryStream = new MemoryStream();
                    await TOUFileViewModel.File.CopyToAsync(memoryStream);
                    if (!ValidationHelper.ValidatePdfSignature(memoryStream))
                    {
                        ModelState.AddModelError("File", "The uploaded file does not appear to be a valid PDF.");
                        return View("TermsOfUseUpload", TOUFileViewModel);
                    }
                    GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, TOUFileViewModel.File.FileName, config.TOUBucketName);

                    if (genericResponse.Success)
                    {
                        summaryViewModel.TOUFileName = TOUFileViewModel.File.FileName;
                        summaryViewModel.TOUFileSizeInKb = Math.Round((decimal)TOUFileViewModel.File.Length / 1024, 1);
                        summaryViewModel.TOUFileLink = genericResponse.Data;
                        TOUFileViewModel.FileUploadedSuccessfully = true;
                        TOUFileViewModel.FileName = TOUFileViewModel.File.FileName;
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);


                        if (action == "continue" || action == "amend")
                        {
                            return View("TermsOfUseUpload", TOUFileViewModel);
                        }
                        else if (action == "draft")
                        {
                            return await SaveAsDraftAndRedirect(summaryViewModel);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid action parameter");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Unable to upload the file provided");
                        return View("TermsOfUseUpload", TOUFileViewModel);
                    }
                }
                else
                {
                    return View("TermsOfUseUpload", TOUFileViewModel);
                }
            }
            else
            {
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, "ProviderRoles", "CabService");
            }
        }

        #endregion

        #region Select service type     


        [HttpGet("select-service-type")]
        public IActionResult SelectServiceType(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/provider-roles";
            return View(serviceSummaryViewModel);
        }

        [HttpPost("select-service-type")]
        public async Task<IActionResult> SaveServiceType(ServiceSummaryViewModel viewModel, string action)
        {
            if (ModelState["ServiceType"].Errors.Count == 0)
            {
                ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
                ServiceTypeEnum? previousServiceType = summaryViewModel.ServiceType;
                summaryViewModel.ServiceType = viewModel.ServiceType;
                bool fromSummaryPage = viewModel.FromSummaryPage;
                bool fromDetailsPage = viewModel.FromDetailsPage;
                viewModel.FromSummaryPage = false;
                viewModel.FromDetailsPage = false;
                viewModel.IsAmendment = summaryViewModel.IsAmendment;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);              

                if (viewModel.ServiceType == ServiceTypeEnum.UnderPinning || viewModel.ServiceType == ServiceTypeEnum.Neither)
                {
                    if(previousServiceType == ServiceTypeEnum.WhiteLabelled)
                    {
                        ViewModelHelper.ClearUnderPinningServiceFields(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    }
                    if (fromSummaryPage && summaryViewModel.HasSupplementarySchemes == null)
                    {
                        return await HandleActions(action, summaryViewModel, false, fromDetailsPage, false, "HasSupplementarySchemesInput", "CabService");
                    }
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, "ServiceGPG45Input");
                }
                else if (viewModel.ServiceType == ServiceTypeEnum.WhiteLabelled)
                {
                    if (previousServiceType == ServiceTypeEnum.UnderPinning || previousServiceType == ServiceTypeEnum.Neither)
                    {
                        summaryViewModel.FromSummaryPage = fromSummaryPage;
                        fromSummaryPage = false;                        
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);// need to eneter underpinning service details
                    }
                    return await HandleAmendActions(action, summaryViewModel, false, fromDetailsPage, false, false, "StatusOfUnderpinningService");
                }
                else
                    throw new InvalidDataException("Invalid service type");               

            }
            else
            {
                return View("SelectServiceType", viewModel);
            }
        }
        #endregion

        #region GPG45 input

        [HttpGet("service/gpg45-input")]
        public IActionResult ServiceGPG45Input(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers"
                : fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey
                : summaryViewModel.ServiceType == ServiceTypeEnum.WhiteLabelled ? "/cab-service/submit-service/select-underpinning-service"
                : "/cab-service/submit-service/select-service-type";
            return View(summaryViewModel);
        }

        [HttpPost("service/gpg45-input")]
        public async Task<IActionResult> SaveServiceGPG45Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.FromSummaryPage = false;
            viewModel.FromDetailsPage = false;
            viewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (ModelState["HasGPG45"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG45 = viewModel.HasGPG45;
                summaryViewModel.RefererURL = viewModel.RefererURL;
                return await HandleGpg45Actions(action, summaryViewModel, fromSummaryPage, fromDetailsPage);
            }
            else
            {
                return View("ServiceGPG45Input", viewModel);
            }
        }
        #endregion

        #region select GPG45

        [HttpGet("service/gpg45")]
        public async Task<IActionResult> ServiceGPG45(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            IdentityProfileViewModel identityProfileViewModel = new()
            {
                IsAmendment = summaryViewModel.IsAmendment,
                SelectedIdentityProfileIds = summaryViewModel?.IdentityProfileViewModel?.SelectedIdentityProfiles?.Select(c => c.Id).ToList(),
                AvailableIdentityProfiles = await cabService.GetIdentityProfiles(),
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/service/gpg45-input"
            };
            return View(identityProfileViewModel);
        }

        /// <summary>
        /// Save selected values to session
        /// </summary>
        /// <param name="identityProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost("service/gpg45")]
        public async Task<IActionResult> SaveServiceGPG45(IdentityProfileViewModel identityProfileViewModel, string action)
        {
            bool fromSummaryPage = identityProfileViewModel.FromSummaryPage;
            bool fromDetailsPage = identityProfileViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            List<IdentityProfileDto> availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds = identityProfileViewModel.SelectedIdentityProfileIds ?? new List<int>();
            identityProfileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (identityProfileViewModel.SelectedIdentityProfileIds.Count > 0)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();
            summaryViewModel.IdentityProfileViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage,false, "ServiceGPG44Input");
            }
            else
            {
                return View("ServiceGPG45", identityProfileViewModel);
            }
        }
        #endregion
    
        #region GPG44 - input

        [HttpGet("service/gpg44-input")]
        public IActionResult ServiceGPG44Input(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers" 
                : fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey 
                : summaryViewModel.HasGPG45 == true ? "/cab-service/submit-service/service/gpg45"
                : "/cab-service/submit-service/service/gpg45-input";
            return View(summaryViewModel);
        }

        [HttpPost("service/gpg44-input")]
        public async Task<IActionResult> SaveServiceGPG44Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.IsAmendment = summaryViewModel.IsAmendment;
            viewModel.FromDetailsPage = false;
            viewModel.FromSummaryPage = false;
            if (ModelState["HasGPG44"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG44 = viewModel.HasGPG44;
                summaryViewModel.RefererURL = viewModel.RefererURL;
                return await HandleGpg44Actions(action, summaryViewModel, fromSummaryPage, fromDetailsPage);
            }
            else
            {
                return View("ServiceGPG44Input", viewModel);
            }
        }

        #endregion

        #region select GPG44
        [HttpGet("service/gpg44")]
        public async Task<IActionResult> ServiceGPG44(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            QualityLevelViewModel qualityLevelViewModel = new()
            {
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/service/gpg44-input"
            };
            var qualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = qualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();
            qualityLevelViewModel.SelectedQualityofAuthenticatorIds = summaryViewModel?.QualityLevelViewModel?.SelectedQualityofAuthenticators?.Select(c => c.Id).ToList();

            qualityLevelViewModel.AvailableLevelOfProtections = qualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds = summaryViewModel?.QualityLevelViewModel?.SelectedLevelOfProtections?.Select(c => c.Id).ToList();

            return View(qualityLevelViewModel);
        }

        /// <summary>
        /// Save selected values to session
        /// </summary>
        /// <param name="qualityLevelViewModel"></param>
        /// <returns></returns>
        [HttpPost("service/gpg44")]
        public async Task<IActionResult> SaveServiceGPG44(QualityLevelViewModel qualityLevelViewModel, string action)
        {

            bool fromSummaryPage = qualityLevelViewModel.FromSummaryPage;
            bool fromDetailsPage = qualityLevelViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            List<QualityLevelDto> availableQualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();
            qualityLevelViewModel.SelectedQualityofAuthenticatorIds = qualityLevelViewModel.SelectedQualityofAuthenticatorIds ?? [];
            qualityLevelViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Contains(c.Id)).ToList();

            qualityLevelViewModel.AvailableLevelOfProtections = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();

            qualityLevelViewModel.SelectedLevelOfProtectionIds = qualityLevelViewModel.SelectedLevelOfProtectionIds ?? [];
            if (qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();


            summaryViewModel.QualityLevelViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage,false, "HasSupplementarySchemesInput","CabService");
            }
            else
            {
                return View("ServiceGPG44", qualityLevelViewModel);
            }
        }
        #endregion

      

        [HttpGet("status-of-underpinning-service")]
        public IActionResult StatusOfUnderpinningService(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/select-service-type";
            return View(summaryViewModel);
        }

        [HttpPost("status-of-underpinning-service")]
        public async Task<IActionResult> StatusOfUnderpinningService(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummary.FromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummary.FromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;

            if (ModelState["IsUnderpinningServicePublished"].Errors.Count == 0)
            {
                serviceSummary.IsUnderpinningServicePublished = serviceSummaryViewModel.IsUnderpinningServicePublished;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleAmendActions(action, serviceSummary, false, false, false, false, "SelectUnderpinningService");
            }
            else
            {
                return View("StatusOfUnderpinningService", serviceSummaryViewModel);
            }
        }

        /// <summary>
        /// Search for services with status published or certificate review 
        /// passed based on selection in StatusOfUnderpinningService
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="SearchAction"></param>
        /// <returns></returns>
        [HttpGet("select-underpinning-service")]
        public async Task<IActionResult> SelectUnderpinningService(string SearchText, string SearchAction)
        {            
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            ViewBag.fromSummaryPage = summaryViewModel.FromSummaryPage;
            ViewBag.fromDetailsPage = summaryViewModel.FromDetailsPage;

            var published = (bool)summaryViewModel.IsUnderpinningServicePublished;

            var services = new List<ServiceDto>();
            var manualServices = new List<ServiceDto>();
            if (SearchAction == "clearSearch")
            {
                ModelState.Clear();
                SearchText = string.Empty;
            }
            else if( SearchAction == "search" && string.IsNullOrEmpty(SearchText))
            {
                SearchText = "All";
            }


            if(SearchText != null)
            {
                if(published)
                {
                    // Fetch only services of type underpinning and status published
                    //IsUnderpinningServicePublished // true
                    //SelectedUnderPinningServiceId // not null
                   // SelectedManualUnderPinningServiceId //null
                    services = await trustFrameworkService.GetPublishedUnderpinningServices(SearchText);
                }
                else
                {
                    //Fetch only manually saved underpinning services with certificate review passed status
                    // Fetch only services of type underpinning and status published
                    //IsUnderpinningServicePublished // false
                    //SelectedUnderPinningServiceId //  null
                    // SelectedManualUnderPinningServiceId //not null
                    manualServices = await trustFrameworkService.GetServicesWithManualUnderinningService(SearchText);
                }
                
            }
            else
            {
                SearchText = string.Empty;

            }
            UnderpinningServiceViewModel underpinningServiceViewModel = new()
            {
                IsPublished = published,
                SearchText = SearchText,
                UnderpinningServices = services,
                ManualUnderpinningServices = manualServices,
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = "/cab-service/submit-service/status-of-underpinning-service"
            };
            return View(underpinningServiceViewModel);
        }


        [HttpGet("selected-underpinning-service")]
      
        public async Task<IActionResult> SelectedUnderpinningService(int serviceId, bool published, bool fromSummaryPage, bool fromDetailsPage)
        {
            //service details with manual service details
            var service = await trustFrameworkService.GetServiceDetails(serviceId);
            ViewBag.published = published;
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            return View(service);
        }



        [HttpGet("confirm-underpinning-service")]     
        public async Task<IActionResult> ConfirmUnderpinningService(int serviceId, bool published, bool fromSummaryPage, bool fromDetailsPage)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.ServiceId = serviceId;
            var service = await trustFrameworkService.GetServiceDetails(serviceId);  //service details with manual service details
            ServiceSummaryViewModel serviceSummary;           
            if (published)
            {
                 serviceSummary = new()
                {
                    SelectedUnderPinningServiceId = service.Id,
                    IsUnderpinningServicePublished = published,
                    UnderPinningProviderName = service.Provider.RegisteredName,
                    UnderPinningServiceName = service.ServiceName,
                    UnderPinningServiceExpiryDate = service.ConformityExpiryDate,
                    SelectCabViewModel = new SelectCabViewModel { SelectedCabId = service?.CabUser?.CabId, SelectedCabName = service?.CabUser?.Cab?.CabName},
                    IsAmendment = summaryViewModel.IsAmendment

                 };

            }
            else
            {
                serviceSummary = new()
                {
                    SelectedManualUnderPinningServiceId = service.ManualUnderPinningService.Id,
                    IsUnderpinningServicePublished = published,
                    UnderPinningProviderName = service.ManualUnderPinningService.ProviderName,
                    UnderPinningServiceName = service.ManualUnderPinningService.ServiceName,
                    UnderPinningServiceExpiryDate = service.ManualUnderPinningService.CertificateExpiryDate,
                    SelectCabViewModel = new SelectCabViewModel { SelectedCabId = service?.ManualUnderPinningService?.CabId, SelectedCabName = service?.ManualUnderPinningService?.Cab?.CabName},
                    IsAmendment = summaryViewModel.IsAmendment
                };

            }
            serviceSummary.RefererURL =  summaryViewModel.IsAmendment == true ? "/cab-service/amend/service-amendments?serviceId=" + summaryViewModel?.ServiceId
            : GetRefererURL();
            return View(serviceSummary);
        }

        [HttpPost("confirm-underpinning-service")]
        public async Task<IActionResult> SaveUnderpinningService(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;

            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;

            serviceSummary.SelectedManualUnderPinningServiceId = serviceSummaryViewModel.SelectedManualUnderPinningServiceId;
            serviceSummary.SelectedUnderPinningServiceId = serviceSummaryViewModel.SelectedUnderPinningServiceId;
            serviceSummary.UnderPinningServiceName = serviceSummaryViewModel.UnderPinningServiceName;
            serviceSummary.UnderPinningProviderName = serviceSummaryViewModel.UnderPinningProviderName;
            serviceSummary.UnderPinningServiceExpiryDate = serviceSummaryViewModel.UnderPinningServiceExpiryDate;
            serviceSummary.SelectCabViewModel = new SelectCabViewModel { SelectedCabId = serviceSummaryViewModel?.SelectCabViewModel?.SelectedCabId,
                SelectedCabName = serviceSummaryViewModel?.SelectCabViewModel?.SelectedCabName};
            serviceSummary.IsManualServiceLinkedToMultipleServices = true;

            if(ModelState["SelectedUnderPinningServiceId"].Errors.Count == 0 && ModelState["UnderPinningServiceName"].Errors.Count == 0)
            {
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                if (fromSummaryPage && serviceSummary.HasSupplementarySchemes == null)
                {
                    return await HandleActions(action, serviceSummary, false, fromDetailsPage, false, "HasSupplementarySchemesInput", "CabService");
                }
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, false, "ServiceGPG45Input");
            }
            else
            {
                return View("ConfirmUnderpinningService", serviceSummaryViewModel);
            }
        }
       


        #region Service Name
        [HttpGet("underpinning-service-name")]
        public IActionResult UnderPinningServiceName(bool fromSummaryPage, bool fromDetailsPage, bool singleChange, bool fromUnderPinningServiceSummaryPage, bool manualEntryFirstTimeLoad)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.singleChange = singleChange;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            if (manualEntryFirstTimeLoad)
            {
                ViewModelHelper.ClearUnderPinningServiceFieldsBeforeManualEntry(serviceSummaryViewModel);
                HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);            
            }
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/select-underpinning-service";

            return View(serviceSummaryViewModel);
        }

        [HttpPost("underpinning-service-name")]
        public async Task<IActionResult> SaveUnderPinningServiceName(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            bool singleChange = serviceSummaryViewModel.SingleChange;
            bool fromUnderPinningServiceSummaryPage = serviceSummaryViewModel.FromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["UnderPinningServiceName"].Errors.Count == 0)
            {
                serviceSummary.UnderPinningServiceName = serviceSummaryViewModel.UnderPinningServiceName;
                serviceSummary.IsManualServiceLinkedToMultipleServices = false;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleAmendActions(action, serviceSummary, false, fromDetailsPage, fromUnderPinningServiceSummaryPage, singleChange, "UnderPinningProviderName");
            }
            else
            {
                return View("UnderPinningServiceName", serviceSummaryViewModel);
            }
        }
        #endregion

        #region underpinning provider name

        [HttpGet("underpinning-provider-name")]
        public IActionResult UnderPinningProviderName(bool fromSummaryPage, bool fromDetailsPage, bool singleChange, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.singleChange = singleChange;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/underpinning-service-name?manualEntryFirstTimeLoad=True";
            return View(serviceSummaryViewModel);
        }

        [HttpPost("underpinning-provider-name")]
        public async Task<IActionResult> SaveUnderPinningProviderName(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            bool singleChange = serviceSummaryViewModel.SingleChange;
            bool fromUnderPinningServiceSummaryPage = serviceSummaryViewModel.FromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["UnderPinningProviderName"].Errors.Count == 0)
            {
                serviceSummary.UnderPinningProviderName = serviceSummaryViewModel.UnderPinningProviderName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleAmendActions(action, serviceSummary, false, fromDetailsPage, fromUnderPinningServiceSummaryPage, singleChange, "SelectCabOfUnderpinningService");
            }
            else
            {
                return View("UnderPinningProviderName", serviceSummaryViewModel);
            }
        }
        #endregion


        #region Select-cab

        [HttpGet("select-cab")]
        public async Task<IActionResult> SelectCabOfUnderpinningService(bool fromSummaryPage, bool fromDetailsPage, bool singleChange, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.singleChange = singleChange;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            var allCabs = await trustFrameworkService.GetCabs();
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            var selectCabViewModel = serviceSummaryViewModel?.SelectCabViewModel ?? new SelectCabViewModel();
            selectCabViewModel.Cabs = allCabs;
            selectCabViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/select-underpinning-service";
            selectCabViewModel.IsAmendment = serviceSummaryViewModel.IsAmendment;
            return View(selectCabViewModel);
        }

        [HttpPost("select-cab")]
        public async Task<IActionResult> SaveSelectedCab(SelectCabViewModel cabsViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = cabsViewModel.FromSummaryPage;
            bool fromDetailsPage = cabsViewModel.FromDetailsPage;
            bool singleChange = cabsViewModel.SingleChange;
            bool fromUnderPinningServiceSummaryPage = cabsViewModel.FromUnderPinningServiceSummaryPage;
            cabsViewModel.FromSummaryPage = false;
            cabsViewModel.FromDetailsPage = false;
            cabsViewModel.IsAmendment = serviceSummary.IsAmendment;


            if (cabsViewModel.Cabs == null)
                cabsViewModel.Cabs = await trustFrameworkService.GetCabs();
            if (ModelState.IsValid)
            {
                string cabName = cabsViewModel.Cabs.Where(x=>x.Id == cabsViewModel.SelectedCabId).Select(x=>x.CabName).First();
                serviceSummary.SelectCabViewModel = new SelectCabViewModel { SelectedCabId = cabsViewModel.SelectedCabId ,
                SelectedCabName = cabName
                };
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleAmendActions(action, serviceSummary, false, fromDetailsPage, fromUnderPinningServiceSummaryPage, singleChange, "UnderPinningServiceExpiryDate");
            }

            return View("SelectCabOfUnderpinningService", cabsViewModel);
        }
        #endregion

        [HttpGet("under-pinning-service-expiry-date")]
        public IActionResult UnderPinningServiceExpiryDate(bool fromSummaryPage, bool fromDetailsPage, bool singleChange, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.singleChange = singleChange;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            DateViewModel dateViewModel = new()
            {
                PropertyName = "UnderPinningServiceExpiryDate"
            };

            if (summaryViewModel.UnderPinningServiceExpiryDate != null)
            {
                dateViewModel = ViewModelHelper.GetDayMonthYear(summaryViewModel.UnderPinningServiceExpiryDate);
            }
            dateViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/underpinning-provider-name";
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            return View(dateViewModel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateViewModel"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost("under-pinning-service-expiry-date")]
        public async Task<IActionResult> SaveUnderPinningServiceExpiryDate(DateViewModel dateViewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            bool fromDetailsPage = dateViewModel.FromDetailsPage;
            bool singleChange = dateViewModel.SingleChange;
            bool fromUnderPinningServiceSummaryPage = dateViewModel.FromUnderPinningServiceSummaryPage;
            dateViewModel.FromDetailsPage = false;
            dateViewModel.FromSummaryPage = false;
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            dateViewModel.PropertyName = "UnderPinningServiceExpiryDate";            

            DateTime? underPinningServiceExpiryDate = ValidationHelper.ValidateCustomExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate), ModelState);
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (ModelState.IsValid)
            {
                summaryViewModel.UnderPinningServiceExpiryDate = underPinningServiceExpiryDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleAmendActions(action, summaryViewModel, false, fromDetailsPage, fromUnderPinningServiceSummaryPage, singleChange, "UnderpinningServiceDetailsSummary");
            }
            else
            {
                return View("UnderPinningServiceExpiryDate", dateViewModel);
            }
        }


        [HttpGet("underpinning-service-details-summary")]
        public IActionResult UnderpinningServiceDetailsSummary()
        {        
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = "/cab-service/submit-service/under-pinning-service-expiry-date";
            return View(summaryViewModel);
        }

        [HttpPost("underpinning-service-details-summary")]
        public async Task<IActionResult> UnderpinningServiceDetailsSummary(string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            if (summaryViewModel.FromSummaryPage && summaryViewModel.HasSupplementarySchemes == null)
            {
                return await HandleActions(action, summaryViewModel, false, summaryViewModel.FromDetailsPage, false, "HasSupplementarySchemesInput", "CabService");
            }
            return await HandleActions(action, summaryViewModel, summaryViewModel.FromSummaryPage, summaryViewModel.FromDetailsPage, false, "ServiceGPG45Input", "TrustFramework0_4");
        }




        [HttpGet("download-certificate")]
        public async Task<IActionResult> DownloadCertificate(string key, string filename)
        {
            try
            {
                byte[]? fileContent = await bucketService.DownloadFileAsync(key, config.BucketName);

                if (fileContent == null || fileContent.Length == 0)
                    throw new InvalidOperationException($"Failed to download certificate: Empty or null content for key.");

                string contentType = "application/octet-stream";
                return File(fileContent, contentType, filename);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception occurred while downloading certificate with key.", ex);
            }
        }


        #region GPG45

        [HttpGet("scheme/gpg45")]
        public async Task<IActionResult> SchemeGPG45(bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            int previousSchemeId = GetPreviousScheme(schemeId);
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers"
                : fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey
                : previousSchemeId == 0 ? "/cab-service/submit-service/supplementary-schemes" 
                : "/cab-service/submit-service/scheme/gpg44?schemeId=" + previousSchemeId;
            var identityProfile = summaryViewModel?.SchemeIdentityProfileMapping?.Where(scheme => scheme.SchemeId == schemeId)?.FirstOrDefault()?.IdentityProfile;
            IdentityProfileViewModel identityProfileViewModel = new()
            {
                SchemeId = schemeId,
                SchemeName = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Where(scheme => scheme.Id == schemeId)
                .Select(scheme => scheme.SchemeName).FirstOrDefault() ?? string.Empty,
                IsAmendment = summaryViewModel.IsAmendment,
                SelectedIdentityProfileIds = identityProfile?.SelectedIdentityProfiles?.Select(c => c.Id)?.ToList() ?? [],
                AvailableIdentityProfiles = await cabService.GetIdentityProfiles(),
                RefererURL = summaryViewModel.RefererURL
            };

            return View(identityProfileViewModel);


        }

        /// <summary>
        /// Save selected values to session
        /// </summary>
        /// <param name="identityProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost("scheme/gpg45")]
        public async Task<IActionResult> SaveSchemeGPG45(IdentityProfileViewModel identityProfileViewModel, string action)
        {
            bool fromSummaryPage = identityProfileViewModel.FromSummaryPage;
            bool fromDetailsPage = identityProfileViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            identityProfileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            List<IdentityProfileDto> availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds = identityProfileViewModel.SelectedIdentityProfileIds ?? new List<int>();

            if (identityProfileViewModel.SelectedIdentityProfileIds.Count > 0)
            {
                var selectedServiceGpg45 = summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles.Select(x => x.Id).ToList();
                if (identityProfileViewModel.SelectedIdentityProfileIds.Except(selectedServiceGpg45).Any())
                {
                    ModelState.AddModelError("SelectedIdentityProfileIds", Constants.NotGpg45SubsetError);
                }

                identityProfileViewModel.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();
                SchemeIdentityProfileMappingViewModel schemeIdentityProfileMappingViewModel = new();
                schemeIdentityProfileMappingViewModel.SchemeId = identityProfileViewModel.SchemeId;
                schemeIdentityProfileMappingViewModel.IdentityProfile = identityProfileViewModel;
                schemeIdentityProfileMappingViewModel.IdentityProfile.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();

                var existingMapping = summaryViewModel?.SchemeIdentityProfileMapping?.FirstOrDefault(x => x.SchemeId == schemeIdentityProfileMappingViewModel.SchemeId);
                if (existingMapping != null)
                {
                    int index = summaryViewModel.SchemeIdentityProfileMapping.IndexOf(existingMapping);
                    summaryViewModel.SchemeIdentityProfileMapping[index] = schemeIdentityProfileMappingViewModel;
                }
                else
                {                   
                    summaryViewModel?.SchemeIdentityProfileMapping?.Add(schemeIdentityProfileMappingViewModel);
                }
            }

            if (ModelState.IsValid)
            {
                if(summaryViewModel.HasGPG44 == false)
                {
                    SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel = new();
                    schemeQualityLevelMappingViewModel.HasGPG44 = false;
                    schemeQualityLevelMappingViewModel.SchemeId = identityProfileViewModel.SchemeId;
                    schemeQualityLevelMappingViewModel.RefererURL = identityProfileViewModel.RefererURL;
                    summaryViewModel?.SchemeQualityLevelMapping?.Add(schemeQualityLevelMappingViewModel);
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    bool hasRemainingSchemes = HasRemainingSchemes(identityProfileViewModel.SchemeId);
                    var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
                    if (hasRemainingSchemes)
                    {
                        return await HandleAmendActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, false, "SchemeGPG45", "TrustFramework0_4", new { schemeId = selectedSchemeIds[0] });
                    }
                    else
                    {
                        return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, summaryViewModel.IsSchemeEditedFromSummary ? "ServiceSummary" : "CertificateUploadPage", "CabService");
                    }
                }
                else
                {
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await HandleAmendActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, false, "SchemeGPG44Input", "TrustFramework0_4",
                    new { schemeId = identityProfileViewModel.SchemeId });
                }
            }
            else
            {
                return View("SchemeGPG45", identityProfileViewModel);
            }
        }
        #endregion

        #region GPG44 - input

        [HttpGet("scheme/gpg44-input")]
        public IActionResult SchemeGPG44Input(bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();         

            SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel = summaryViewModel?.SchemeQualityLevelMapping?.Where(scheme => scheme.SchemeId == schemeId).
            FirstOrDefault() ?? new();
            schemeQualityLevelMappingViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers" :
                fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey : 
                "/cab-service/submit-service/scheme/gpg45?schemeId=" + schemeId;
            schemeQualityLevelMappingViewModel.SchemeId = schemeId;
            schemeQualityLevelMappingViewModel.SchemeName = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Where(scheme => scheme.Id == schemeId)
            .Select(scheme => scheme.SchemeName).FirstOrDefault() ?? string.Empty;
            schemeQualityLevelMappingViewModel.IsAmendment = summaryViewModel.IsAmendment;

            return View(schemeQualityLevelMappingViewModel);
        }

        [HttpPost("scheme/gpg44-input")]
        public async Task<IActionResult> SaveSchemeGPG44Input(SchemeQualityLevelMappingViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.IsAmendment = summaryViewModel.IsAmendment;
            viewModel.FromDetailsPage = false;
            viewModel.FromSummaryPage = false;

            if (summaryViewModel.HasGPG44 == false && viewModel.HasGPG44 == true)
            {
                ModelState.AddModelError("HasGPG44", Constants.ServiceGpg44SelectedNo);
            }
            if (ModelState["HasGPG44"].Errors.Count == 0)
            {
                SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel = new();

                var existingMapping = summaryViewModel?.SchemeQualityLevelMapping?.FirstOrDefault(x => x.SchemeId == viewModel.SchemeId);
                if (existingMapping != null)
                {
                    int index = summaryViewModel.SchemeQualityLevelMapping.IndexOf(existingMapping);
                    schemeQualityLevelMappingViewModel = viewModel;
                    summaryViewModel.SchemeQualityLevelMapping[index].HasGPG44 = viewModel.HasGPG44;
                }
                else
                {                  
                    schemeQualityLevelMappingViewModel.HasGPG44 = viewModel.HasGPG44;
                    schemeQualityLevelMappingViewModel.SchemeId = viewModel.SchemeId;
                    schemeQualityLevelMappingViewModel.RefererURL = viewModel.RefererURL;
                    summaryViewModel?.SchemeQualityLevelMapping?.Add(schemeQualityLevelMappingViewModel);
                }               
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleSchemeGpg44Actions(action, summaryViewModel, schemeQualityLevelMappingViewModel, fromSummaryPage, fromDetailsPage, viewModel.SchemeId);
            }
            else
            {
                return View("SchemeGPG44Input", viewModel);
            }
        }

        #endregion

        #region select GPG44
        [HttpGet("scheme/gpg44")]
        public async Task<IActionResult> SchemeGPG44(bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {


            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            QualityLevelViewModel qualityLevelViewModel = new()
            {
                SchemeId = schemeId,
                SchemeName = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Where(scheme => scheme.Id == schemeId)
                .Select(scheme => scheme.SchemeName).FirstOrDefault() ?? string.Empty,
                IsAmendment = summaryViewModel.IsAmendment
            
            };

            qualityLevelViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/scheme/gpg44-input?schemeId=" + schemeId;
            var qualityLevel = summaryViewModel?.SchemeQualityLevelMapping?.Where(scheme => scheme.SchemeId == schemeId)?.FirstOrDefault()?.QualityLevel;

            var qualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = qualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();
            qualityLevelViewModel.SelectedQualityofAuthenticatorIds = qualityLevel?.SelectedQualityofAuthenticators?.Select(c => c.Id)?.ToList() ?? [];

            qualityLevelViewModel.AvailableLevelOfProtections = qualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds = qualityLevel?.SelectedLevelOfProtections?.Select(c => c.Id)?.ToList() ?? [];

            return View(qualityLevelViewModel);
        }

        /// <summary>
        /// Save selected values to session
        /// </summary>
        /// <param name="qualityLevelViewModel"></param>
        /// <returns></returns>
        [HttpPost("scheme/gpg44")]
        public async Task<IActionResult> SaveSchemeGPG44(QualityLevelViewModel qualityLevelViewModel, string action)
        {

            bool fromSummaryPage = qualityLevelViewModel.FromSummaryPage;
            bool fromDetailsPage = qualityLevelViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            qualityLevelViewModel.IsAmendment = summaryViewModel.IsAmendment;
            List<QualityLevelDto> availableQualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();
            qualityLevelViewModel.SelectedQualityofAuthenticatorIds = qualityLevelViewModel.SelectedQualityofAuthenticatorIds ?? [];
            qualityLevelViewModel.AvailableLevelOfProtections = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds = qualityLevelViewModel.SelectedLevelOfProtectionIds ?? [];
            if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Count > 0 && qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
            {
                var selectedServiceGpg44Authenticator = summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators.Select(x => x.Id).ToList();

                var selectedServiceGpg44Protection = summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections.Select(x => x.Id).ToList();
                if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Except(selectedServiceGpg44Authenticator).Any())
                {
                    ModelState.AddModelError("SelectedQualityofAuthenticatorIds", Constants.NotGpg44SubsetError);
                }
                if (qualityLevelViewModel.SelectedLevelOfProtectionIds.Except(selectedServiceGpg44Protection).Any())
                {
                    ModelState.AddModelError("SelectedLevelOfProtectionIds", Constants.NotGpg44SubsetError);
                }

                qualityLevelViewModel.SelectedQualityofAuthenticators = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Contains(c.Id)).ToList();
                qualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();
                var mapping = summaryViewModel.SchemeQualityLevelMapping?.Where(x => x.SchemeId == qualityLevelViewModel.SchemeId).FirstOrDefault()??new SchemeQualityLevelMappingViewModel();                
                mapping.QualityLevel = qualityLevelViewModel;

            }
            summaryViewModel.QualityLevelViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                bool hasRemainingSchemes = HasRemainingSchemes(qualityLevelViewModel.SchemeId);
                var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");              
                if (hasRemainingSchemes)
                {
                    return await HandleAmendActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, false, "SchemeGPG45", "TrustFramework0_4", new { schemeId = selectedSchemeIds[0] });
                }
                else
                {                   
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage,false, summaryViewModel.IsSchemeEditedFromSummary ? "ServiceSummary": "CertificateUploadPage", "CabService");
                }
            }
            else
            {
                return View("SchemeGPG44", qualityLevelViewModel);
            }
        }
        #endregion

        #region Private methods
        
        private async Task<IActionResult> HandleGpg45Actions(string action, ServiceSummaryViewModel summaryViewModel, bool fromSummaryPage, bool fromDetailsPage)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(summaryViewModel.HasGPG45))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceGPG45", new {fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary","CabService") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : RedirectToAction("ServiceGPG44Input");
                    }

                case "draft":
                    if (!Convert.ToBoolean(summaryViewModel.HasGPG45))
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                    }
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await SaveAsDraftAndRedirect(summaryViewModel);

                case "amend":
                    if (Convert.ToBoolean(summaryViewModel.HasGPG45))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceGPG45", new { fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("ServiceGPG44Input") : RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> HandleGpg44Actions(string action, ServiceSummaryViewModel summaryViewModel, bool fromSummaryPage, bool fromDetailsPage)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(summaryViewModel.HasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceGPG44", new { fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary", "CabService") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : RedirectToAction("HasSupplementarySchemesInput","CabService");
                    }

                case "draft":
                    if (!Convert.ToBoolean(summaryViewModel.HasGPG44))
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                    }
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await SaveAsDraftAndRedirect(summaryViewModel);

                case "amend":

                    if (Convert.ToBoolean(summaryViewModel.HasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceGPG44");
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("HasSupplementarySchemesInput", "CabService") :  RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> HandleSchemeGpg44Actions(string action, ServiceSummaryViewModel summaryViewModel,SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel,
            bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(schemeQualityLevelMappingViewModel.HasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SchemeGPG44", new {  fromSummaryPage, fromDetailsPage, schemeId });
                    }
                    else
                    {
                        bool hasRemainingSchemes = HasRemainingSchemes(schemeId);
                        var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
                        ViewModelHelper.ClearSchemeGpg44(summaryViewModel,schemeId);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary","CabService") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : hasRemainingSchemes ? RedirectToAction("SchemeGPG45", new { schemeId = selectedSchemeIds[0] }) :
                         summaryViewModel.IsSchemeEditedFromSummary? RedirectToAction("ServiceSummary", "CabService"): RedirectToAction("CertificateUploadPage", "CabService");
                    }

                case "draft":
                    if (!Convert.ToBoolean(schemeQualityLevelMappingViewModel.HasGPG44))
                    {
                        ViewModelHelper.ClearSchemeGpg44(summaryViewModel, schemeId);
                    }
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await SaveAsDraftAndRedirect(summaryViewModel);

                case "amend":

                    if (Convert.ToBoolean(schemeQualityLevelMappingViewModel.HasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SchemeGPG44", new {fromSummaryPage, fromDetailsPage, schemeId });
                    }
                    else
                    {
                        bool hasRemainingSchemes = HasRemainingSchemes(schemeId);
                        var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
                        ViewModelHelper.ClearSchemeGpg44(summaryViewModel, schemeId);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return hasRemainingSchemes ? RedirectToAction("SchemeGPG45", new { schemeId = selectedSchemeIds[0] }) : summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("CertificateUploadPage", "CabService"):
                            RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> HandleActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage, string nextPage,
            string controller = "TrustFramework0_4", object routeValues = null!)
        {
            switch (action)
            {
                case "continue":
                    return   fromUnderPinningServiceSummaryPage ? RedirectToAction("UnderpinningServiceDetailsSummary", "TrustFramework0_4")
                           : fromSummaryPage ? RedirectToAction("ServiceSummary", "CabService")
                           : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                           : routeValues == null ? RedirectToAction(nextPage, controller) 
                           : RedirectToAction(nextPage, controller, routeValues);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return serviceSummary.IsTFVersionChanged.GetValueOrDefault() ? routeValues == null ? RedirectToAction(nextPage, controller) 
                   : RedirectToAction(nextPage, controller, routeValues) : RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        
        private async Task<IActionResult> HandleAmendActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage,
            bool singleChange, string nextPage, string controller = "TrustFramework0_4", object routeValues = null!)
        {
            switch (action)
            {
                case "continue":
                    return singleChange ? RedirectToAction("ServiceSummary", "CabService")
                           : fromUnderPinningServiceSummaryPage ? RedirectToAction("UnderpinningServiceDetailsSummary", "TrustFramework0_4")
                           : fromSummaryPage ? RedirectToAction("ServiceSummary", "CabService")
                           : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                           : routeValues == null ? RedirectToAction(nextPage, controller)
                           : RedirectToAction(nextPage, controller, routeValues);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return fromSummaryPage || singleChange ? RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment")
                        : routeValues == null ? RedirectToAction(nextPage, controller)
                        : RedirectToAction(nextPage, controller, routeValues);

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> SaveAsDraftAndRedirect(ServiceSummaryViewModel serviceSummary)
        {
            GenericResponse genericResponse = new();
            serviceSummary.ServiceStatus = ServiceStatusEnum.SavedAsDraft;
            ServiceDto serviceDto = mapper.Map<ServiceDto>(serviceSummary);
            ViewModelHelper.MapTFVersion0_4Fields(serviceSummary, serviceDto);
            if (!IsValidCabId(serviceSummary.CabId))
                return HandleInvalidCabId(serviceSummary.CabId);

            if (serviceDto.CabUserId < 0) throw new InvalidDataException("Invalid CabUserId");

            if (serviceSummary.IsResubmission)
            {
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail, serviceSummary.IsReupload.GetValueOrDefault(),null);
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            }

            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ServiceSummary");
                return RedirectToAction("ServiceDraftDetails", "CabServiceReApplication", new { serviceId = genericResponse.InstanceId });
            }
            else
            {
                throw new InvalidOperationException("SaveAsDraftAndRedirect: Failed to save draft");
            }

        }

        private bool HasRemainingSchemes(int schemeId)
        {

            var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
            if (selectedSchemeIds != null && selectedSchemeIds.Count > 0)
            {
                selectedSchemeIds.Remove(schemeId);
                HttpContext?.Session.Set("SelectedSchemeIds", selectedSchemeIds);
                return selectedSchemeIds.Count > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        private int GetPreviousScheme(int schemeId)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            var selectedSchemeIds = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(x => x.Id).ToList();

            if (selectedSchemeIds != null && selectedSchemeIds.Count > 0)
            {
                int currentIndex = selectedSchemeIds.IndexOf(schemeId);               
                if (currentIndex > 0)
                {
                    return selectedSchemeIds[currentIndex - 1]; 
                }
            }

            return 0; 
        }
        private async Task HandleInvalidProfileAndCab(int providerProfileId, CabUserDto cabUserDto)
        {
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (!isValid)
                throw new InvalidOperationException("Invalid provider profile ID for Cab ID");
        }

        /// <summary>
        /// Restrict selected roles to those supported by the given TF version.
        /// </summary>
        private async Task KeepOnlyRolesSupportedByAsync(ServiceSummaryViewModel summary, decimal? version)
        {
            if (version is null) return;

            var availableRoles = await cabService.GetRoles(version.Value);
            var selectedRoles = summary.RoleViewModel?.SelectedRoles ?? new List<RoleDto>();

            summary.RoleViewModel ??= new RoleViewModel();
            summary.RoleViewModel.SelectedRoles = selectedRoles.Where(sr => availableRoles.Any(ar => ar.Id == sr.Id)).ToList();
        }

        /// <summary>
        /// Clear schemes and GPG-related fields when a TF version transition requires resetting state.
        /// </summary>
        private void ClearSchemesAndGpg(ServiceSummaryViewModel summary)
        {
            ViewModelHelper.ClearSchemes(summary);
            ViewModelHelper.ClearGpg44(summary);
            ViewModelHelper.ClearGpg45(summary);

            summary.HasSupplementarySchemes = null;
            summary.HasGPG45 = null;
            summary.HasGPG44 = null;
            summary.IsTFVersionChanged = true;
        }

        #endregion
    }
}