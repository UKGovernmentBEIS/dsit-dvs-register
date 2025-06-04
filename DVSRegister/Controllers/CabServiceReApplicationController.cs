using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Extensions;
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
        public  IActionResult ResumeSubmission()
        {
            if(!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
            return RedirectToNextEmptyField(serviceSummary);
            
        }
        
        [HttpGet("before-new-certificate")]
        public async Task<IActionResult> BeforeYouSubmitNewCertificate(int serviceKey, int providerProfileId)
        {

            ViewBag.ServiceKey = serviceKey;
            ViewBag.ProviderProfileId = providerProfileId;        
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            
            if(!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
        
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (isValid)
            {
                ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
                serviceSummary.IsResubmission = true;                 
                serviceSummary.CabId = cabUserDto.CabId;
                serviceSummary.CabUserId = cabUserDto.Id;
                serviceSummary.ServiceKey = serviceKey;
                serviceSummary.ProviderProfileId = providerProfileId;
                if (!serviceSummary.IsDraft)
                    serviceSummary.ResetInpuData(); // clear current input data from session for resubmission if it is not a draft version
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return View();
            }
            else
            {
                throw new ArgumentException("Invalid providerProfileId.");
            }
        }


        #region Private method
        private IActionResult RedirectToNextEmptyField( ServiceSummaryViewModel serviceSummary)
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