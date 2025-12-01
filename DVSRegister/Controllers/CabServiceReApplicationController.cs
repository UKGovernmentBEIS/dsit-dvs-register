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
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/re-application")]
    public class CabServiceReApplicationController(ICabService cabService, IUserService userService, ILogger<CabServiceReApplicationController> logger) : BaseController(logger)
    {
        private readonly ICabService cabService = cabService;
        private readonly IUserService userService = userService;
        private readonly ILogger<CabServiceReApplicationController> _logger = logger;

        [HttpGet("resume-submission")]
        public IActionResult ResumeSubmission()
        {
            if(!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
            return RedirectToNextEmptyField(serviceSummary);
            
        }
        
        [HttpGet("before-new-certificate")]
        public async Task<IActionResult> BeforeYouSubmitNewCertificate(int serviceKey, int providerProfileId, int currentServiceId, bool isReupload)
        {
            ViewBag.serviceKey = serviceKey;    
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            
            if(!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
        
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (isValid)
            {

                ServiceDto currentServiceVersion = await cabService.GetServiceDetails(currentServiceId, CabId);              
                SetServiceDataToSession(CabId, currentServiceVersion);
                ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
                serviceSummary.IsResubmission = true;
                serviceSummary.IsReupload = isReupload;
                serviceSummary.CabId = cabUserDto.CabId;
                serviceSummary.CabUserId = cabUserDto.Id;
                serviceSummary.ServiceKey = serviceKey;
                serviceSummary.ProviderProfileId = providerProfileId;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return View();
            }
            else
            {
                throw new ArgumentException("Invalid providerProfileId.");
            }
        }
        [HttpPost("before-new-certificate")]
        public async Task<IActionResult> ContinueSubmission()
        {
            ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
        
            var serviceList = await cabService.GetServiceList(serviceSummary.ServiceKey, CabId);
            if(!serviceSummary.IsReupload.GetValueOrDefault())
            {
                  InProgressApplicationParameters inProgressApplicationParameters =   ViewModelHelper.GetInProgressApplicationParameters(serviceList);
          
                if (inProgressApplicationParameters.HasInProgressApplication || inProgressApplicationParameters.HasActiveReassignmentRequest
                 || inProgressApplicationParameters.HasActiveRemovalRequest || inProgressApplicationParameters.InProgressAndUpdateRequested)
                {
                    return RedirectToAction("StartInProgressApplicationRemoval");

                }
                else
                {
                    return RedirectToAction("SelectVersionOfTrustFrameWork", "TrustFramework0_4", new { providerProfileId = serviceSummary.ProviderProfileId });
                }
            }
            else
            {
                return RedirectToAction("SelectVersionOfTrustFrameWork", "TrustFramework0_4", new { providerProfileId = serviceSummary.ProviderProfileId });
            }
            
          
        }


        [HttpGet("in-progress-application-removal-start")]
        public IActionResult StartInProgressApplicationRemoval()
        {
            ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
            ViewBag.ServiceKey = serviceSummary.ServiceKey;
            ViewBag.ProviderProfileId = serviceSummary.ProviderProfileId;
            ViewBag.ServiceId = serviceSummary.ServiceId;
            return View();
        }

   


        #region Private method
        private IActionResult RedirectToNextEmptyField(ServiceSummaryViewModel serviceSummary)
        {
            if (serviceSummary.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4)
            {
                bool isPublished = serviceSummary.IsUnderpinningServicePublished != null && serviceSummary.IsUnderpinningServicePublished == true;
                bool isSelected = serviceSummary.SelectedUnderPinningServiceId != null;

                bool hasIdentityProfileSchemeMappings = serviceSummary.HasSupplementarySchemes == true
                && serviceSummary?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Count > 0
                && serviceSummary.SchemeIdentityProfileMapping?.Count > 0;

                List<int> schemeIdsToCheck = serviceSummary?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(scheme => scheme.Id).ToList() ?? new List<int>();

                bool allSchemeIdentityProfilesPresent = schemeIdsToCheck.All(id => serviceSummary.SchemeIdentityProfileMapping.Any(mapping => mapping.SchemeId == id));

                bool allSchemeQualityLevelsPresent = schemeIdsToCheck.All(id => serviceSummary.SchemeQualityLevelMapping.Any(mapping => mapping.SchemeId == id &&
                     (mapping.HasGPG44.GetValueOrDefault() ? mapping?.QualityLevel?.SelectedLevelOfProtections?.Count > 0 : true)));

                if (string.IsNullOrEmpty(serviceSummary.ServiceName))
                {
                    return RedirectToAction("ServiceName", "CabService", new { providerProfileId = serviceSummary.ProviderProfileId });
                }
                else if (string.IsNullOrEmpty(serviceSummary.ServiceURL))
                {
                    return RedirectToAction("ServiceURL", "CabService", new { providerProfileId = serviceSummary.ProviderProfileId });
                }
                else if (string.IsNullOrEmpty(serviceSummary.CompanyAddress))
                {
                    return RedirectToAction("CompanyAddress", "CabService");
                }
                else if (serviceSummary.RoleViewModel.SelectedRoles == null || serviceSummary.RoleViewModel.SelectedRoles.Count == 0)
                {
                    return RedirectToAction("ProviderRoles", "CabService");
                }
                else if (serviceSummary.ServiceType == 0)
                {
                    return RedirectToAction("SelectServiceType", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && serviceSummary.IsUnderpinningServicePublished == null)
                {
                    return RedirectToAction("StatusOfUnderpinningService", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && isPublished && !isSelected)
                {
                    return RedirectToAction("SelectUnderpinningService", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && !isPublished && string.IsNullOrEmpty(serviceSummary.UnderPinningServiceName))
                {
                    return RedirectToAction("SelectUnderpinningService", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && !isPublished && string.IsNullOrEmpty(serviceSummary.UnderPinningProviderName))
                {
                    return RedirectToAction("UnderPinningProviderName", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && !isPublished && serviceSummary.SelectCabViewModel.SelectedCabId == null)
                {
                    return RedirectToAction("SelectCabOfUnderpinningService", "TrustFramework0_4");
                }
                else if (serviceSummary.ServiceType == ServiceTypeEnum.WhiteLabelled && !isPublished && serviceSummary.UnderPinningServiceExpiryDate == null)
                {
                    return RedirectToAction("UnderPinningServiceExpiryDate", "TrustFramework0_4");
                }

                else if (serviceSummary.HasGPG45 == null)
                {
                    return RedirectToAction("ServiceGPG45Input", "TrustFramework0_4");
                }
                else if (serviceSummary.HasGPG45 == true && serviceSummary.IdentityProfileViewModel.SelectedIdentityProfiles.Count == 0)
                {
                    return RedirectToAction("ServiceGPG45", "TrustFramework0_4");
                }

                else if (serviceSummary.HasGPG44 == null)
                {
                    return RedirectToAction("ServiceGPG44Input", "TrustFramework0_4");
                }
                else if (serviceSummary.HasGPG44 == true && (serviceSummary.QualityLevelViewModel.SelectedQualityofAuthenticators.Count == 0 ||
                    serviceSummary.QualityLevelViewModel.SelectedLevelOfProtections.Count == 0))
                {
                    return RedirectToAction("ServiceGPG44", "TrustFramework0_4");
                }
                else if (serviceSummary.HasSupplementarySchemes == null)
                {
                    return RedirectToAction("HasSupplementarySchemesInput", "CabService");
                }
                else if (serviceSummary.HasSupplementarySchemes == true && serviceSummary.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Count == 0)
                {
                    return RedirectToAction("SupplementarySchemes", "CabService");
                }
                else if ((!hasIdentityProfileSchemeMappings || !allSchemeIdentityProfilesPresent || !allSchemeQualityLevelsPresent) && serviceSummary.HasSupplementarySchemes == true)
                {
                    var selectedSchemeIds = serviceSummary?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(scheme => scheme.Id).ToList();
                    foreach (var scheme in serviceSummary?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes)
                    {
                        selectedSchemeIds.Remove(scheme.Id);
                        HttpContext?.Session.Set("SelectedSchemeIds", selectedSchemeIds);

                        var identityProfile = serviceSummary.SchemeIdentityProfileMapping.Where(s => s.SchemeId == scheme.Id).FirstOrDefault();
                        var qualityLevel = serviceSummary.SchemeQualityLevelMapping.Where(s => s.SchemeId == scheme.Id).FirstOrDefault();

                        if (identityProfile == null)
                        {
                            return RedirectToAction("SchemeGPG45", "TrustFramework0_4", new { schemeId = scheme.Id });
                        }
                        else if (qualityLevel == null)
                        {
                            return RedirectToAction("SchemeGPG44Input", "TrustFramework0_4", new { schemeId = scheme.Id });
                        }
                        else if (qualityLevel != null && qualityLevel.HasGPG44 == true && 
                            (qualityLevel?.QualityLevel?.SelectedLevelOfProtections?.Count == 0 || qualityLevel?.QualityLevel?.SelectedQualityofAuthenticators?.Count == 0))
                        {
                            return RedirectToAction("SchemeGPG44", "TrustFramework0_4", new { schemeId = scheme.Id });
                        }
                    }
                    return RedirectToAction("SchemeGPG45", "TrustFramework0_4", new { schemeId = schemeIdsToCheck[0] });
                }
                else if (serviceSummary.FileName == null)
                {
                    return RedirectToAction("CertificateUploadPage", "CabService");
                }
                else if (serviceSummary.ConformityIssueDate == null)
                {
                    return RedirectToAction("ConfirmityIssueDate", "CabService");
                }
                else if (serviceSummary.ConformityExpiryDate == null)
                {
                    return RedirectToAction("ConfirmityExpiryDate", "CabService");
                }
                else
                {
                    return RedirectToAction("ServiceSummary", "CabService");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(serviceSummary.ServiceName))
                {
                    return RedirectToAction("ServiceName", "CabService", new { providerProfileId = serviceSummary.ProviderProfileId });
                }
                else if (string.IsNullOrEmpty(serviceSummary.ServiceURL))
                {
                    return RedirectToAction("ServiceURL", "CabService", new { providerProfileId = serviceSummary.ProviderProfileId });
                }
                else if (string.IsNullOrEmpty(serviceSummary.CompanyAddress))
                {
                    return RedirectToAction("CompanyAddress", "CabService");
                }
                else if (serviceSummary.RoleViewModel.SelectedRoles == null || serviceSummary.RoleViewModel.SelectedRoles.Count == 0)
                {
                    return RedirectToAction("ProviderRoles", "CabService");
                }
                else if (serviceSummary.HasGPG44 == null)
                {
                    return RedirectToAction("GPG44Input", "CabService");
                }
                else if (serviceSummary.HasGPG44 == true && (serviceSummary.QualityLevelViewModel.SelectedQualityofAuthenticators.Count == 0 ||
                    serviceSummary.QualityLevelViewModel.SelectedLevelOfProtections.Count == 0))
                {
                    return RedirectToAction("GPG44", "CabService");
                }
                else if (serviceSummary.HasGPG45 == null)
                {
                    return RedirectToAction("GPG45Input", "CabService");
                }
                else if (serviceSummary.HasGPG45 == true && serviceSummary.IdentityProfileViewModel.SelectedIdentityProfiles.Count == 0)
                {
                    return RedirectToAction("GPG45", "CabService");
                }
                else if (serviceSummary.HasSupplementarySchemes == null)
                {
                    return RedirectToAction("HasSupplementarySchemesInput", "CabService");
                }
                else if (serviceSummary.HasSupplementarySchemes == true && serviceSummary.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Count == 0)
                {
                    return RedirectToAction("SupplementarySchemes", "CabService");
                }
                else if (serviceSummary.FileName == null)
                {
                    return RedirectToAction("CertificateUploadPage", "CabService");
                }
                else if (serviceSummary.ConformityIssueDate == null)
                {
                    return RedirectToAction("ConfirmityIssueDate", "CabService");
                }
                else if (serviceSummary.ConformityExpiryDate == null)
                {
                    return RedirectToAction("ConfirmityExpiryDate", "CabService");
                }
                else
                {
                    return RedirectToAction("ServiceSummary", "CabService");
                }
            }
        }
        #endregion
    }
}