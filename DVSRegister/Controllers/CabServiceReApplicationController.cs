using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/re-application")]
    public class CabServiceReApplicationController : Controller
    { 
        [HttpGet("resume-submission")]
        public async Task<IActionResult> ResumeSubmission(int serviceId)
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
        public IActionResult BeforeNewCertificate(int providerProfileId)
        {
            ViewBag.ProviderProfileId = providerProfileId;
            
            return View("BeforeYouSubmitNewCertificate");
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
