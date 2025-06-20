using DVSRegister.BusinessLogic.Models;
using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models.CAB.Service;

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("cab-service/submit-service")]
    public class TrustFramework0_4Controller(ITrustFrameworkService trustFrameworkService, ICabService cabService, IUserService userService, IMapper mapper, ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;        
        private readonly ITrustFrameworkService trustFrameworkService = trustFrameworkService;
        private readonly ICabService cabService = cabService;
        private readonly IUserService userService = userService;
        private readonly IMapper mapper = mapper;
       

        [HttpGet("select-cab")]
        public async Task<IActionResult> SelectCabOfUnderpinningService()
        {
            var allCabs = await trustFrameworkService.GetCabs();
            var AllCabsViewModel = new AllCabsViewModel
            {
                Cabs = allCabs
            };

            return View(AllCabsViewModel);
        }


        [HttpGet("select-service-type")]
        public IActionResult SelectSelectServiceType()
        {
            //Figma 604
            // ServiceTypeEnum to pupulate radios

            return View();
        }

        [HttpPost("select-service-type")]
        public async Task<IActionResult> SaveSelectServiceType(string action)
        {
            //to do - set data
            bool fromSummaryPage = false;
            bool fromDetailsPage = false;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                //Todo:
                string nextPage = string.Empty;
                string controller = string.Empty;
                //nextPage = GPG45 in cabservice controller for  underpinning or neither
                //nextpage = StatusOfUnderpinningService in this controller for whitelabelled
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, nextPage, controller);
            }
            else
            {
                return View("SelectSelectServiceType");
            }
        }



        [HttpGet("status-of-underpinning-service")]
        public IActionResult StatusOfUnderpinningService()
            => View();

        [HttpGet("select-underpinning-or-white-labelled")]
        public IActionResult UnderpinningChoice()
             => View();


        private async Task<IActionResult> HandleActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, string nextPage, string controller = "TrustFramework0_4")
        {
            switch (action)
            {
                case "continue":
                    return fromSummaryPage ? RedirectToAction("ServiceSummary")
                        : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                        : RedirectToAction(nextPage, controller);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> SaveAsDraftAndRedirect(ServiceSummaryViewModel serviceSummary)
        {
            GenericResponse genericResponse = new();
            serviceSummary.ServiceStatus = ServiceStatusEnum.SavedAsDraft;
            ServiceDto serviceDto = mapper.Map<ServiceDto>(serviceSummary);
            if (!IsValidCabId(serviceSummary.CabId))
                return HandleInvalidCabId(serviceSummary.CabId);

            if (serviceDto.CabUserId < 0) throw new InvalidDataException("Invalid CabUserId");

            if (serviceSummary.IsResubmission)
            {
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail);
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            }

            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ServiceSummary");
                return RedirectToAction("ProviderServiceDetails", "Cab", new { serviceKey = genericResponse.InstanceId });
            }
            else
            {
                throw new InvalidOperationException("SaveAsDraftAndRedirect: Failed to save draft");
            }

        }

        [HttpGet("tf-version")]
        public async Task<IActionResult> SelectVersionOfTrustFrameWork(bool fromSummaryPage, int providerProfileId, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;

            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            await HandleInvalidProfileAndCab(providerProfileId, cabUserDto);

            var referralUrl = GetRefererURL();

            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            TFVersionViewModel TFVersionViewModel = new()
            {
                SelectedTFVersionId = summaryViewModel?.TFVersionViewModel?.SelectedTFVersionId,
                AvailableVersions = await trustFrameworkService.GetTrustFrameworkVersions(),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = referralUrl
            };

            summaryViewModel.ProviderProfileId = providerProfileId;
            summaryViewModel.RefererURL = referralUrl;
            HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

            return View(TFVersionViewModel);
        }

        [HttpPost("tf-version")]
        public async Task<IActionResult> SaveTFVersion(TFVersionViewModel TFVersionViewModel, string action)
        {
            bool fromSummaryPage = TFVersionViewModel.FromSummaryPage;
            bool fromDetailsPage = TFVersionViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<TrustFrameworkVersionDto> availableVersion = await trustFrameworkService.GetTrustFrameworkVersions();
            TFVersionViewModel.AvailableVersions = availableVersion;
            TFVersionViewModel.SelectedTFVersionId = TFVersionViewModel.SelectedTFVersionId ?? null;
            TFVersionViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (TFVersionViewModel.SelectedTFVersionId != null)
            {
                summaryViewModel.TFVersionViewModel.SelectedTFVersion = availableVersion
                    .FirstOrDefault(c => c.Id == TFVersionViewModel.SelectedTFVersionId);
            }

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "ServiceName", "CabService");
            }
            else
            {
                return View("SelectVersionOfTrustFrameWork", TFVersionViewModel);
            }
        }
        
        [HttpGet("unregistered-underpinning-service")]
        public IActionResult UnregisteredUnderpinningService()
            => View();
        
        [HttpGet("confirmed-unregistered-underpinning-service")]
        public IActionResult ConfirmedUnregisteredUnderpinningService()
            => View();
        
        [HttpGet("edit-underpinning-service-details")]
        public IActionResult EditUnderpinningServiceDetails()
            => View();

        [HttpGet("select-underpinning-service")]
        public async Task<IActionResult> SelectUnderpinningService()
        {
            List<String> allServiceAndProviderNames = await trustFrameworkService.GetPSNames();
            ViewBag.ServiceAndProviderNames = allServiceAndProviderNames;
            return View();
        }


        #region GPG45

        [HttpGet("scheme/gpg45")]
        public async Task<IActionResult> SchemeGPG45(bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

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
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "SchemeGPG44Input", "TrustFramework0_4", new { schemeId = identityProfileViewModel.SchemeId });
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
            summaryViewModel.RefererURL = GetRefererURL();
            summaryViewModel.SchemeId = schemeId;
            summaryViewModel.SchemeName = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Where(scheme => scheme.Id == schemeId)
            .Select(scheme => scheme.SchemeName).FirstOrDefault() ?? string.Empty;
            return View(summaryViewModel);
        }

        [HttpPost("scheme/gpg44-input")]
        public async Task<IActionResult> SaveSchemeGPG44Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.IsAmendment = summaryViewModel.IsAmendment;
            viewModel.FromDetailsPage = false;
            viewModel.FromSummaryPage = false;
            if (ModelState["SchemeHasGPG44"].Errors.Count == 0)
            {
                SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel = new();
                schemeQualityLevelMappingViewModel.HasGpg44 = viewModel.SchemeHasGPG44;
                schemeQualityLevelMappingViewModel.SchemeId = viewModel.SchemeId;

                var existingMapping = summaryViewModel?.SchemeQualityLevelMapping?.FirstOrDefault(x => x.SchemeId == viewModel.SchemeId);
                if (existingMapping != null)
                {
                    int index = summaryViewModel.SchemeQualityLevelMapping.IndexOf(existingMapping);
                    summaryViewModel.SchemeQualityLevelMapping[index] = schemeQualityLevelMappingViewModel;
                }
                else
                {
                    summaryViewModel?.SchemeQualityLevelMapping?.Add(schemeQualityLevelMappingViewModel);
                }

                summaryViewModel.SchemeHasGPG44 = viewModel.SchemeHasGPG44;
                summaryViewModel.RefererURL = viewModel.RefererURL;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleGpg44Actions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, viewModel.SchemeId);
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
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = summaryViewModel.RefererURL
            };
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
            if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Count > 0)
            {
                qualityLevelViewModel.SelectedQualityofAuthenticators = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Contains(c.Id)).ToList();
                qualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();
                var mapping = summaryViewModel.SchemeQualityLevelMapping?.FirstOrDefault(x => x.SchemeId == qualityLevelViewModel.SchemeId);
                mapping.QualityLevel = qualityLevelViewModel;

            }


            qualityLevelViewModel.AvailableLevelOfProtections = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds = qualityLevelViewModel.SelectedLevelOfProtectionIds ?? [];
            if (qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();


            summaryViewModel.QualityLevelViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                bool hasRemainingSchemes = HasRemainingSchemes(qualityLevelViewModel.SchemeId);
                var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
                if (hasRemainingSchemes)
                {
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "SchemeGPG45", "TrustFramework0_4", new { schemeId = selectedSchemeIds[0] });
                }
                else
                {
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "CertificateUploadPage", "CabService");
                }

            }
            else
            {
                return View("GPG44", qualityLevelViewModel);
            }
        }
        #endregion


        #region Private methods
        private async Task<IActionResult> HandleActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, string nextPage, string controller = "TrustFramework0_4", object routeValues = null!)
        {
            switch (action)
            {
                case "continue":
                    return fromSummaryPage ? RedirectToAction("ServiceSummary")
                        : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                        : routeValues == null ? RedirectToAction(nextPage, controller) : RedirectToAction(nextPage, controller, routeValues);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandleGpg44Actions(string action, ServiceSummaryViewModel summaryViewModel, bool fromSummaryPage, bool fromDetailsPage, int schemeId)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(summaryViewModel.SchemeHasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SchemeGPG44", new { fromSummaryPage = fromSummaryPage, fromDetailsPage = fromDetailsPage, schemeId = schemeId });
                    }
                    else
                    {
                        bool hasRemainingSchemes = HasRemainingSchemes(schemeId);
                        var selectedSchemeIds = HttpContext?.Session.Get<List<int>>("SelectedSchemeIds");
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : hasRemainingSchemes ? RedirectToAction("SchemeGPG45", new { schemeId = selectedSchemeIds[0] }) : RedirectToAction("CerificateUploadPage", "CabService");
                    }

                case "draft":
                    if (!Convert.ToBoolean(summaryViewModel.SchemeHasGPG44))
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                    }
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await SaveAsDraftAndRedirect(summaryViewModel);

                case "amend":

                    if (Convert.ToBoolean(summaryViewModel.SchemeHasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("GPG44");
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
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
      
        #endregion



        private async Task HandleInvalidProfileAndCab(int providerProfileId, CabUserDto cabUserDto)
        {
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (!isValid)
                throw new InvalidOperationException("Invalid provider profile ID for Cab ID");
        }

    }
}