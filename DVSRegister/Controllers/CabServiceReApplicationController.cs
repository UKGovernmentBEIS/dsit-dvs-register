using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/re-application")]
    public class CabServiceReApplicationController : Controller
    {
        private readonly ICabService cabService;
        private readonly IUserService userService;
        private string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        public CabServiceReApplicationController(ICabService cabService, IUserService userService)
        {
            this.cabService = cabService;
            this.userService = userService;
        }

        [HttpGet("resume-submission")]
        public  IActionResult ResumeSubmission()
        {

            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0)
            {
                ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
                return RedirectToNextEmptyField(serviceSummary);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
        }
        
        [HttpGet("before-new-certificate")]
        public async Task<IActionResult> BeforeYouSubmitNewCertificate(int serviceKey, int providerProfileId)
        {

            ViewBag.ServiceKey = serviceKey;
            ViewBag.ProviderProfileId = providerProfileId;
            string email = HttpContext?.Session.Get<string>("Email") ?? string.Empty;
            CabUserDto cabUserDto = await userService.GetUser(email);
            if (cabUserDto.Id > 0)
            {
                // to prevent another cab changing the providerProfileId from url
                bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
                if (isValid)
                {
                    ServiceSummaryViewModel serviceSummary = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel();
                    serviceSummary.IsResubmission = true;                 
                    serviceSummary.CabId = cabUserDto.CabId;
                    serviceSummary.CabUserId = cabUserDto.Id;
                    if(!serviceSummary.IsDraft)
                        serviceSummary.ResetInpuData(); // clear current input data from session for resubmission if it is not a draft version
                    HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                    return View();
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return RedirectToAction("HandleException", "Error");
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
