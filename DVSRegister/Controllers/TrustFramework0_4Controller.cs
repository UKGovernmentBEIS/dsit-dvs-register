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

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("cab-service/submit-service")]
    public class TrustFramework0_4Controller(ITrustFrameworkService trustFrameworkService, ICabService cabService, IUserService userService,IBucketService bucketService, IMapper mapper, ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;        
        private readonly ITrustFrameworkService trustFrameworkService = trustFrameworkService;
        private readonly ICabService cabService = cabService;
        private readonly IUserService userService = userService;
        private readonly IBucketService bucketService = bucketService;
        private readonly IMapper mapper = mapper;     

        [HttpGet("tf-version")]
        public async Task<IActionResult> SelectVersionOfTrustFrameWork(int providerProfileId, bool fromSummaryPage,  bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;

            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            await HandleInvalidProfileAndCab(providerProfileId, cabUserDto);

            var referralUrl = GetRefererURL();

            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            TFVersionViewModel TFVersionViewModel = new()
            {
                SelectedTFVersionId = summaryViewModel?.TFVersionViewModel?.SelectedTFVersion?.Id,
                AvailableVersions = await trustFrameworkService.GetTrustFrameworkVersions(),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = referralUrl,
                
            }; 

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

            TrustFrameworkVersionDto previousSelection = new();
            if (TFVersionViewModel.SelectedTFVersionId != null)
            {
                previousSelection = summaryViewModel.TFVersionViewModel.SelectedTFVersion;
                summaryViewModel.TFVersionViewModel = new();              
                summaryViewModel.TFVersionViewModel.SelectedTFVersion = availableVersion.FirstOrDefault(c => c.Id == TFVersionViewModel.SelectedTFVersionId);// current selection
            }
            if (ModelState.IsValid)
            {
                if (previousSelection?.Version == Constants.TFVersion0_4 && summaryViewModel?.TFVersionViewModel?.SelectedTFVersion?.Version == Constants.TFVersion0_3)
                {
                    ViewModelHelper.ClearTFVersion0_4Fields(summaryViewModel); // clear extra fields value changed from 0.4 to 0.3
                }
                 HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, false, "ServiceName", "CabService");
            }
            else
            {
                return View("SelectVersionOfTrustFrameWork", TFVersionViewModel);
            }
        }

        #region Select service type

      


        [HttpGet("select-service-type")]
        public IActionResult SelectServiceType(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = GetRefererURL();
            return View(serviceSummaryViewModel);
        }

        [HttpPost("select-service-type")]
        public async Task<IActionResult> SaveServiceType(ServiceSummaryViewModel viewModel, string action)
        {
            if (ModelState["ServiceType"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
                ServiceTypeEnum? previousServiceType = serviceSummaryViewModel.ServiceType;
                serviceSummaryViewModel.ServiceType = viewModel.ServiceType;
                bool fromSummaryPage = viewModel.FromSummaryPage;
                bool fromDetailsPage = viewModel.FromDetailsPage;
                HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
                string nextPage = string.Empty;

                if (viewModel.ServiceType == ServiceTypeEnum.UnderPinning || viewModel.ServiceType == ServiceTypeEnum.Neither)
                {
                    if(previousServiceType == ServiceTypeEnum.WhiteLabelled)
                    {
                        ViewModelHelper.ClearUnderPinningServiceFields(serviceSummaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
                    }
                    nextPage = "ServiceGPG45Input";
                }
                else if (viewModel.ServiceType == ServiceTypeEnum.WhiteLabelled)
                {
                    if (previousServiceType == ServiceTypeEnum.UnderPinning || previousServiceType == ServiceTypeEnum.Neither)
                    {
                        fromSummaryPage = false; // need to eneter underpinning service details
                        
                    }
                    nextPage = "StatusOfUnderpinningService";

                }
                else
                    throw new InvalidDataException("Invalid service type");

                return await HandleActions(action, serviceSummaryViewModel, fromSummaryPage, fromDetailsPage, false, nextPage);

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
            summaryViewModel.RefererURL = GetRefererURL();
            return View(summaryViewModel);
        }

        [HttpPost("service/gpg45-input")]
        public async Task<IActionResult> SaveServiceGPG45Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            viewModel.IsAmendment = summaryViewModel.IsAmendment;
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.FromSummaryPage = false;
            viewModel.FromDetailsPage = false;
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
                RefererURL = GetRefererURL()
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
            identityProfileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            List<IdentityProfileDto> availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds = identityProfileViewModel.SelectedIdentityProfileIds ?? new List<int>();
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
            summaryViewModel.RefererURL = GetRefererURL();
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
                RefererURL = summaryViewModel.RefererURL
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
            qualityLevelViewModel.IsAmendment = summaryViewModel.IsAmendment;
            List<QualityLevelDto> availableQualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();

            qualityLevelViewModel.SelectedQualityofAuthenticatorIds = qualityLevelViewModel.SelectedQualityofAuthenticatorIds ?? [];
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
            summaryViewModel.RefererURL = GetRefererURL();
            return View(summaryViewModel);
        }

        [HttpPost("status-of-underpinning-service")]
        public async Task<IActionResult> StatusOfUnderpinningService(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;

            if (ModelState["IsUnderpinningServicePublished"].Errors.Count == 0)
            {
                serviceSummary.IsUnderpinningServicePublished = serviceSummaryViewModel.IsUnderpinningServicePublished;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, false, "SelectUnderpinningService");
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
        public async Task<IActionResult> SelectUnderpinningService(string SearchText, string SearchAction, bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
        
            var published = (bool)summaryViewModel.IsUnderpinningServicePublished;

            var services = new List<ServiceDto>();
            var manualServices = new List<ServiceDto>();
            if (SearchAction == "clearSearch")
            {
                ModelState.Clear();
                SearchText = string.Empty;
            }

            if(SearchText != null)
            {
                if(published)
                {
                    services = await trustFrameworkService.GetPublishedUnderpinningServices(SearchText);
                }
                else
                {
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
                ManualUnderpinningServices = manualServices
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
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
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
                    SelectCabViewModel = new SelectCabViewModel { SelectedCabId = service?.CabUser?.CabId, SelectedCabName = service?.CabUser?.Cab?.CabName }

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
                    SelectCabViewModel = new SelectCabViewModel { SelectedCabId = service?.ManualUnderPinningService?.CabId, SelectedCabName = service?.ManualUnderPinningService?.Cab?.CabName }

                };

            }
           
            return View(serviceSummary);
        }

        [HttpPost("confirm-underpinning-service")]
        public async Task<IActionResult> SaveUnderpinningService(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;

            serviceSummary.SelectedManualUnderPinningServiceId = serviceSummaryViewModel.SelectedManualUnderPinningServiceId;
            serviceSummary.SelectedUnderPinningServiceId = serviceSummaryViewModel.SelectedUnderPinningServiceId;
            serviceSummary.UnderPinningServiceName = serviceSummaryViewModel.UnderPinningServiceName;
            serviceSummary.UnderPinningProviderName = serviceSummaryViewModel.UnderPinningProviderName;
            serviceSummary.UnderPinningServiceExpiryDate = serviceSummaryViewModel.UnderPinningServiceExpiryDate;
            serviceSummary.SelectCabViewModel = new SelectCabViewModel { SelectedCabId = serviceSummaryViewModel?.SelectCabViewModel?.SelectedCabId,
                SelectedCabName = serviceSummaryViewModel?.SelectCabViewModel?.SelectedCabName};

            if(ModelState["SelectedUnderPinningServiceId"].Errors.Count == 0 && ModelState["UnderPinningServiceName"].Errors.Count == 0)
            {
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, false, "ServiceGPG45Input");
            }
            else
            {
                return View("ConfirmUnderpinningService", serviceSummaryViewModel);
            }
        }
       


        #region Service Name
        [HttpGet("underpinning-service-name")]
        public IActionResult UnderPinningServiceName(bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage, bool manualEntryFirstTimeLoad)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            if (manualEntryFirstTimeLoad)
            {
                ViewModelHelper.ClearUnderPinningServiceFieldsBeforeManualEntry(serviceSummaryViewModel);
                HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
            }
            serviceSummaryViewModel.RefererURL = GetRefererURL();
            return View(serviceSummaryViewModel);
        }

        [HttpPost("underpinning-service-name")]
        public async Task<IActionResult> SaveUnderPinningServiceName(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            bool fromUnderPinningServiceSummaryPage = serviceSummaryViewModel.FromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["UnderPinningServiceName"].Errors.Count == 0)
            {
                serviceSummary.UnderPinningServiceName = serviceSummaryViewModel.UnderPinningServiceName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, fromUnderPinningServiceSummaryPage, "UnderPinningProviderName", "TrustFramework0_4");
            }
            else
            {
                return View("UnderPinningServiceName", serviceSummaryViewModel);
            }
        }
        #endregion

        #region underpinning provider name

        [HttpGet("underpinning-provider-name")]
        public IActionResult UnderPinningProviderName(bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = GetRefererURL();
            return View(serviceSummaryViewModel);
        }

        [HttpPost("underpinning-provider-name")]
        public async Task<IActionResult> SaveUnderPinningProviderName(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            bool fromUnderPinningServiceSummaryPage = serviceSummaryViewModel.FromUnderPinningServiceSummaryPage;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["UnderPinningProviderName"].Errors.Count == 0)
            {
                serviceSummary.UnderPinningProviderName = serviceSummaryViewModel.UnderPinningProviderName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, fromUnderPinningServiceSummaryPage, "SelectCabOfUnderpinningService", "TrustFramework0_4");
            }
            else
            {
                return View("UnderPinningProviderName", serviceSummaryViewModel);
            }
        }
        #endregion


        #region Select-cab

        [HttpGet("select-cab")]
        public async Task<IActionResult> SelectCabOfUnderpinningService(bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ViewBag.fromUnderPinningServiceSummaryPage = fromUnderPinningServiceSummaryPage;
            var allCabs = await trustFrameworkService.GetCabs();
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            var selectCabViewModel = serviceSummaryViewModel?.SelectCabViewModel ?? new SelectCabViewModel();
            selectCabViewModel.Cabs = allCabs;
            selectCabViewModel.RefererURL = GetRefererURL();
            return View(selectCabViewModel);
        }

        [HttpPost("select-cab")]
        public async Task<IActionResult> SaveSelectedCab(SelectCabViewModel cabsViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = cabsViewModel.FromSummaryPage;
            bool fromDetailsPage = cabsViewModel.FromDetailsPage;
            bool fromUnderPinningServiceSummaryPage = cabsViewModel.FromUnderPinningServiceSummaryPage;
            if (cabsViewModel.Cabs == null)
                cabsViewModel.Cabs = await trustFrameworkService.GetCabs();
            if (ModelState.IsValid)
            {
                string cabName = cabsViewModel.Cabs.Where(x=>x.Id == cabsViewModel.SelectedCabId).Select(x=>x.CabName).First();
                serviceSummary.SelectCabViewModel = new SelectCabViewModel { SelectedCabId = Convert.ToInt32(cabsViewModel.SelectedCabId),
                SelectedCabName = cabName
                };
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, fromUnderPinningServiceSummaryPage, "UnderPinningServiceExpiryDate", "TrustFramework0_4");
            }

            return View("SelectCabOfUnderpinningService", cabsViewModel);
        }
        #endregion

        [HttpGet("under-pinning-service-expiry-date")]
        public IActionResult UnderPinningServiceExpiryDate(bool fromSummaryPage, bool fromDetailsPage, bool fromUnderPinningServiceSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
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
            dateViewModel.RefererURL = GetRefererURL();
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
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            bool fromDetailsPage = dateViewModel.FromDetailsPage;
            bool fromUnderPinningServiceSummaryPage = dateViewModel.FromUnderPinningServiceSummaryPage;
            dateViewModel.FromDetailsPage = false;
            dateViewModel.PropertyName = "UnderPinningServiceExpiryDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            DateTime? underPinningServiceExpiryDate = ValidationHelper.ValidateCustomExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate), ModelState);
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (ModelState.IsValid)
            {
                summaryViewModel.UnderPinningServiceExpiryDate = underPinningServiceExpiryDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, fromUnderPinningServiceSummaryPage, "UnderpinningServiceDetailsSummary", "TrustFramework0_4");
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
            return View(summaryViewModel);
        }

        [HttpPost("underpinning-service-details-summary")]
        public async Task<IActionResult> UnderpinningServiceDetailsSummary(string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return await HandleActions(action, summaryViewModel, false, false, false, "ServiceGPG45Input", "TrustFramework0_4");
        }




        [HttpGet("download-certificate")]
        public async Task<IActionResult> DownloadCertificate(string key, string filename)
        {
            try
            {
                byte[]? fileContent = await bucketService.DownloadFileAsync(key);

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
            summaryViewModel.RefererURL = GetRefererURL();
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
                   
                    summaryViewModel.SchemeIdentityProfileMapping.Add(schemeIdentityProfileMappingViewModel);
                }

            }

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);               
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage,false, "SchemeGPG44Input", "TrustFramework0_4", 
                new { schemeId = identityProfileViewModel.SchemeId });
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
            schemeQualityLevelMappingViewModel.RefererURL = GetRefererURL();
            schemeQualityLevelMappingViewModel.SchemeId = schemeId;
            schemeQualityLevelMappingViewModel.SchemeName = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Where(scheme => scheme.Id == schemeId)
            .Select(scheme => scheme.SchemeName).FirstOrDefault() ?? string.Empty;

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
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = GetRefererURL()
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
            qualityLevelViewModel.AvailableLevelOfProtections = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds = qualityLevelViewModel.SelectedLevelOfProtectionIds ?? [];
            if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Count > 0 && qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
            {
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
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage,false, "SchemeGPG45", "TrustFramework0_4", new { schemeId = selectedSchemeIds[0] });
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
                        return RedirectToAction("ServiceGPG45", new { fromSummaryPage = fromSummaryPage, fromDetailsPage = fromDetailsPage });
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
                        return RedirectToAction("ServiceGPG45");
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
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
                        return RedirectToAction("ServiceGPG44", new { fromSummaryPage = fromSummaryPage, fromDetailsPage = fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
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
                        return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
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
                        return RedirectToAction("SchemeGPG44", new { fromSummaryPage = fromSummaryPage, fromDetailsPage = fromDetailsPage, schemeId = schemeId });
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
                        return RedirectToAction("SchemeGPG44");
                    }
                    else
                    {
                        ViewModelHelper.ClearSchemeGpg44(summaryViewModel, schemeId);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
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
        private async Task HandleInvalidProfileAndCab(int providerProfileId, CabUserDto cabUserDto)
        {
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (!isValid)
                throw new InvalidOperationException("Invalid provider profile ID for Cab ID");
        }

        #endregion





    }
}