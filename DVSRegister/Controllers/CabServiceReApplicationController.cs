using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/re-application")]
    public class CabServiceReApplicationController : Controller
    {

        private readonly ICabService cabService;

        public CabServiceReApplicationController(ICabService cabService)
        {
            this.cabService = cabService;            
        }

        [HttpGet("resume-submission")]
        public async Task<IActionResult> ResumeSubmission(int serviceId)
        {

            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0)
            {
                ServiceDto serviceDto = await cabService.GetServiceDetails(serviceId, cabId);
                RoleViewModel roleViewModel = new()
                {
                    SelectedRoles = new List<RoleDto>()
                };
                QualityLevelViewModel qualityLevelViewModel = new()
                {
                    SelectedLevelOfProtections = new List<QualityLevelDto>(),
                    SelectedQualityofAuthenticators = new List<QualityLevelDto>()
                };

                IdentityProfileViewModel identityProfileViewModel = new()
                {
                    SelectedIdentityProfiles = new List<IdentityProfileDto>()
                };

                SupplementarySchemeViewModel supplementarySchemeViewModel = new()
                {
                    SelectedSupplementarySchemes = new List<SupplementarySchemeDto>()
                };


                if (serviceDto.ServiceRoleMapping != null && serviceDto.ServiceRoleMapping.Count > 0)
                {
                    roleViewModel.SelectedRoles = serviceDto.ServiceRoleMapping.Select(mapping => mapping.Role).ToList();
                }

                if (serviceDto.ServiceQualityLevelMapping != null && serviceDto.ServiceQualityLevelMapping.Count > 0)
                {
                    var protectionLevels = serviceDto.ServiceQualityLevelMapping
                    .Where(item => item.QualityLevel.QualityType == QualityTypeEnum.Protection)
                    .Select(item => item.QualityLevel);

                    var authenticatorLevels = serviceDto.ServiceQualityLevelMapping
                    .Where(item => item.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel);

                    foreach (var item in protectionLevels)
                    {
                        qualityLevelViewModel.SelectedLevelOfProtections.Add(item);
                    }

                    foreach (var item in authenticatorLevels)
                    {
                        qualityLevelViewModel.SelectedQualityofAuthenticators.Add(item);
                    }


                }
                if (serviceDto.ServiceIdentityProfileMapping != null && serviceDto.ServiceIdentityProfileMapping.Count > 0)
                {
                    identityProfileViewModel.SelectedIdentityProfiles = serviceDto.ServiceIdentityProfileMapping.Select(mapping => mapping.IdentityProfile).ToList();
                }
                if (serviceDto.ServiceSupSchemeMapping != null && serviceDto.ServiceSupSchemeMapping.Count > 0)
                {
                    supplementarySchemeViewModel.SelectedSupplementarySchemes = serviceDto.ServiceSupSchemeMapping.Select(mapping => mapping.SupplementaryScheme).ToList();
                }


                ServiceSummaryViewModel serviceSummary = new ServiceSummaryViewModel
                {
                    ServiceName = serviceDto.ServiceName,
                    ServiceURL = serviceDto.WebSiteAddress,
                    CompanyAddress = serviceDto.CompanyAddress,
                    RoleViewModel = roleViewModel,
                    IdentityProfileViewModel = identityProfileViewModel,
                    QualityLevelViewModel = qualityLevelViewModel,
                    HasSupplementarySchemes = serviceDto.HasSupplementarySchemes,
                    HasGPG44 = serviceDto.HasGPG44,
                    HasGPG45 = serviceDto.HasGPG45,
                    SupplementarySchemeViewModel = supplementarySchemeViewModel,
                    FileLink = serviceDto.FileLink,
                    FileName = serviceDto.FileName,
                    FileSizeInKb = serviceDto.FileSizeInKb,
                    ConformityIssueDate = serviceDto.ConformityIssueDate == DateTime.MinValue ? null : serviceDto.ConformityIssueDate,
                    ConformityExpiryDate = serviceDto.ConformityExpiryDate == DateTime.MinValue ? null : serviceDto.ConformityExpiryDate,
                    ServiceId = serviceDto.Id,
                    ProviderProfileId = serviceDto.ProviderProfileId,
                    CabId = cabId,
                    CabUserId = serviceDto.CabUserId
                };
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return RedirectToNextEmptyField(serviceDto, serviceSummary);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
        }


        #region Private method
        private IActionResult RedirectToNextEmptyField(ServiceDto serviceDto, ServiceSummaryViewModel serviceSummary)
        {
            if (string.IsNullOrEmpty(serviceSummary.ServiceName))
            {
                return RedirectToAction("ServiceName", "CabService", new { providerProfileId = serviceDto.ProviderProfileId });
            }
            else if (string.IsNullOrEmpty(serviceSummary.ServiceURL))
            {
                return RedirectToAction("ServiceURL", "CabService", new { providerProfileId = serviceDto.ProviderProfileId });
            }
            else if (string.IsNullOrEmpty(serviceSummary.CompanyAddress))
            {
                return RedirectToAction("CompanyAddress","CabService");
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
        #endregion
    }
}
