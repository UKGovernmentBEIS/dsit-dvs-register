using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;


namespace DVSRegister.Controllers
{
    [Route("cab-service/submit-service")]
    [ValidCognitoToken]
    public class CabServiceController : Controller
    {

        private readonly ICabService cabService;
        private readonly IBucketService bucketService;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;
        private string UserEmail => HttpContext.Session.Get<string>("Email")??string.Empty;
        public CabServiceController(ICabService cabService, IBucketService bucketService, IUserService userService, IEmailSender emailSender)
        {
            this.cabService = cabService;
            this.bucketService = bucketService;
            this.userService=userService;
            this.emailSender=emailSender;
        }

        [HttpGet("before-you-start")]
        public async Task<IActionResult> BeforeYouStart(int providerProfileId)
        {
            ViewBag.ProviderProfileId = providerProfileId;
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            CabUserDto cabUserDto = await userService.GetUser(email);
            if(cabUserDto.Id>0 )
            {
                // to prevent another cab changing the providerProfileId from url
                bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
                if(isValid)
                {
                    ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
                    serviceSummaryViewModel.CabId = cabUserDto.CabId;
                    serviceSummaryViewModel.CabUserId = cabUserDto.Id;
                    HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
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
        

        #region Service Name
        [HttpGet("name-of-service")]
        public async Task<IActionResult> ServiceName(bool fromSummaryPage, int providerProfileId)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            CabUserDto cabUserDto = await userService.GetUser(email);
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (providerProfileId > 0 && isValid) 
            {
                serviceSummaryViewModel.ProviderProfileId = providerProfileId;
                HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
                return View(serviceSummaryViewModel);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
          

          
        }

        [HttpPost("name-of-service")]
        public async Task<IActionResult> SaveServiceName(ServiceSummaryViewModel serviceSummaryViewModel, string action )
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ServiceName"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.ServiceName = serviceSummaryViewModel.ServiceName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                if(action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ServiceURL");
                }                
                else if(action == "draft")
                {
                    return await SaveAsDraftAndRedirect(serviceSummary);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("ServiceName", serviceSummaryViewModel);
            }
        }
        #endregion

        #region Service URL
        [HttpGet("service-url")]
        public IActionResult ServiceURL(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            return View("ServiceURL", serviceSummaryViewModel);
        }
        [HttpPost("service-url")]
        public async Task<IActionResult> SaveServiceURL(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ServiceURL"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.ServiceURL = serviceSummaryViewModel.ServiceURL;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                if (action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("CompanyAddress");
                }
                else if (action == "draft")
                {
                    return await SaveAsDraftAndRedirect(serviceSummary);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }
            }
            {
                return View("ServiceURL", serviceSummaryViewModel);
            }
        }

     
        #endregion

        #region Company Address
        [HttpGet("company-address")]
        public IActionResult CompanyAddress(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            return View("CompanyAddress", serviceSummaryViewModel);
        }
        [HttpPost("company-address")]
        public async Task <IActionResult> SaveCompanyAddress(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["CompanyAddress"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.CompanyAddress = serviceSummaryViewModel.CompanyAddress;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);

                if(action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ProviderRoles");
                }
                else if(action == "draft")
                {
                    return await SaveAsDraftAndRedirect(serviceSummary);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("CompanyAddress", serviceSummaryViewModel);
            }
        }
        #endregion

        #region Roles

        [HttpGet("provider-roles")]
        public async Task<IActionResult> ProviderRoles(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            RoleViewModel roleViewModel = new RoleViewModel();
            roleViewModel.SelectedRoleIds = summaryViewModel?.RoleViewModel?.SelectedRoles?.Select(c => c.Id).ToList();
            roleViewModel.AvailableRoles = await cabService.GetRoles();
            return View(roleViewModel);
        }


        /// <summary>
        /// Save selected roles to session
        /// </summary>
        /// <param name="roleViewModel"></param>
        /// <returns></returns>
        [HttpPost("provider-roles")]
        public async Task<IActionResult> SaveRoles(RoleViewModel roleViewModel, string action)
        {
            bool fromSummaryPage = roleViewModel.FromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<RoleDto> availableRoles = await cabService.GetRoles();          
            roleViewModel.AvailableRoles = availableRoles;
            roleViewModel.SelectedRoleIds =  roleViewModel.SelectedRoleIds??new List<int>();
            if (roleViewModel.SelectedRoleIds.Count > 0)
                summaryViewModel.RoleViewModel.SelectedRoles = availableRoles.Where(c => roleViewModel.SelectedRoleIds.Contains(c.Id)).ToList();
            summaryViewModel.RoleViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                if(action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("GPG44Input");
                }
                else if(action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("ProviderRoles", roleViewModel);
            }
        }
        #endregion

        #region GPG44 - input

        [HttpGet("gpg44-input")]
        public IActionResult GPG44Input(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return View(summaryViewModel);
        }

        [HttpPost("gpg44-input")]
        public async Task<IActionResult> SaveGPG44Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasGPG44"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG44 = viewModel.HasGPG44;    
                
                if(action == "continue")
                {
                    if (Convert.ToBoolean(summaryViewModel.HasGPG44))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("GPG44", new { fromSummaryPage = fromSummaryPage });
                    }
                    else
                    {
                        // clear selections if the value is changed from yes to no
                        summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators = new List<QualityLevelDto>();
                        summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = new List<QualityLevelDto>();
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("GPG45Input");
                    }
                }
                else if (action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("GPG44Input", viewModel);
            }
        }
        #endregion

        #region select GPG44
        [HttpGet("gpg44")]
        public async Task<IActionResult> GPG44(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            QualityLevelViewModel qualityLevelViewModel = new QualityLevelViewModel();
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
        [HttpPost("gpg44")]
        public async Task<IActionResult> SaveGPG44(QualityLevelViewModel qualityLevelViewModel, string action)
        {
            bool fromSummaryPage = qualityLevelViewModel.FromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<QualityLevelDto> availableQualityLevels = await cabService.GetQualitylevels();
            qualityLevelViewModel.AvailableQualityOfAuthenticators = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList();
            qualityLevelViewModel.SelectedQualityofAuthenticatorIds =  qualityLevelViewModel.SelectedQualityofAuthenticatorIds??new List<int>();
            if (qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedQualityofAuthenticatorIds.Contains(c.Id)).ToList();

            qualityLevelViewModel.AvailableLevelOfProtections = availableQualityLevels.Where(x => x.QualityType == QualityTypeEnum.Protection).ToList();
            qualityLevelViewModel.SelectedLevelOfProtectionIds =  qualityLevelViewModel.SelectedLevelOfProtectionIds??new List<int>();
            if (qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();


            summaryViewModel.QualityLevelViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                if(action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("GPG45Input");
                }
                else if (action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("GPG44", qualityLevelViewModel);
            }
        }
        #endregion

        #region GPG45 input

        [HttpGet("gpg45-input")]
        public IActionResult GPG45Input(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return View(summaryViewModel);
        }

        [HttpPost("gpg45-input")]
        public async Task<IActionResult> SaveGPG45Input(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasGPG45"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG45 = viewModel.HasGPG45;

                if (action == "continue")
                {
                    if (Convert.ToBoolean(summaryViewModel.HasGPG45))
                    {
                        Debug.WriteLine($"pressed yes {summaryViewModel.HasGPG45}");
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("GPG45", new { fromSummaryPage = fromSummaryPage });
                    }
                    else
                    {
                        Debug.WriteLine($"pressed no {summaryViewModel.HasGPG45}");
                        summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = new List<IdentityProfileDto>();
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("HasSupplementarySchemesInput");
                    } 
                }
                else if (action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("GPG45Input", viewModel);
            }
        }
        #endregion

        #region select GPG45

        [HttpGet("gpg45")]
        public async Task<IActionResult> GPG45(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            IdentityProfileViewModel identityProfileViewModel = new IdentityProfileViewModel();
            identityProfileViewModel.SelectedIdentityProfileIds = summaryViewModel?.IdentityProfileViewModel?.SelectedIdentityProfiles?.Select(c => c.Id).ToList();
            identityProfileViewModel.AvailableIdentityProfiles = await cabService.GetIdentityProfiles();
            return View(identityProfileViewModel);
        }

        /// <summary>
        /// Save selected values to session
        /// </summary>
        /// <param name="identityProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost("gpg45")]
        public async Task<IActionResult> SaveGPG45(IdentityProfileViewModel identityProfileViewModel, string action)
        {
            bool fromSummaryPage = identityProfileViewModel.FromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<IdentityProfileDto> availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds =  identityProfileViewModel.SelectedIdentityProfileIds??new List<int>();
            if (identityProfileViewModel.SelectedIdentityProfileIds.Count > 0)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();
            summaryViewModel.IdentityProfileViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                if(action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("HasSupplementarySchemesInput");
                }
                else if (action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }
            }
            else
            {
                return View("GPG45", identityProfileViewModel);
            }
        }
        #endregion


        #region Supplemetary schemes

        [HttpGet("supplementary-schemes-input")]
        public IActionResult HasSupplementarySchemesInput(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return View(summaryViewModel);
        }

        [HttpPost("supplementary-schemes-input")]
        public async Task<IActionResult> SaveHasSupplementarySchemesInput(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasSupplementarySchemes"].Errors.Count == 0)
            {
                summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;

                if (action == "continue")
                {
                    if (Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SupplementarySchemes", new { fromSummaryPage = fromSummaryPage });
                    }
                    else
                    {
                        summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = new List<SupplementarySchemeDto>();
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("CertificateUploadPage");
                    } 
                }
                else if (action == "draft")
                {

                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }

            }
            else
            {
                return View("HasSupplementarySchemesInput", viewModel);
            }
        }

        [HttpGet("supplementary-schemes")]
        public async Task<IActionResult> SupplementarySchemes(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            SupplementarySchemeViewModel supplementarySchemeViewModel = new SupplementarySchemeViewModel();
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(c => c.Id).ToList();
            supplementarySchemeViewModel.AvailableSchemes = await cabService.GetSupplementarySchemes();
            return View(supplementarySchemeViewModel);
        }

        [HttpPost("supplementary-schemes")]
        public async Task<IActionResult> SaveSupplementarySchemes(SupplementarySchemeViewModel supplementarySchemeViewModel, string action)
        {
            bool fromSummaryPage = supplementarySchemeViewModel.FromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<SupplementarySchemeDto> availableSupplementarySchemes = await cabService.GetSupplementarySchemes();
            supplementarySchemeViewModel.AvailableSchemes = availableSupplementarySchemes;
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds =  supplementarySchemeViewModel.SelectedSupplementarySchemeIds??new List<int>();
            if (supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Count > 0)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = availableSupplementarySchemes.Where(c => supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
            summaryViewModel.SupplementarySchemeViewModel.FromSummaryPage = false;

            if (action == "continue")
            {
                if (ModelState.IsValid)
                {
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("CertificateUploadPage");
                }
                else
                {
                    return View("SupplementarySchemes", supplementarySchemeViewModel);
                } 
            }
            else if (action == "draft")
            {

                return await SaveAsDraftAndRedirect(summaryViewModel);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
        }
        #endregion

        #region File upload/download

        [HttpGet("certificate-upload")]
        public async Task<IActionResult> CertificateUploadPage(bool fromSummaryPage, bool remove)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            CertificateFileViewModel certificateFileViewModel = new();
            certificateFileViewModel.HasSupplementarySchemes = summaryViewModel.HasSupplementarySchemes;
            if (remove)
            {
                summaryViewModel.FileLink = null;
                summaryViewModel.FileName = null;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return View(certificateFileViewModel);
            }
            if (!string.IsNullOrEmpty(summaryViewModel.FileName) && !string.IsNullOrEmpty(summaryViewModel.FileLink))
            {
                certificateFileViewModel.FileName = summaryViewModel.FileName;
                certificateFileViewModel.FileUrl = summaryViewModel.FileLink;                
                var fileContent = await bucketService.DownloadFileAsync(summaryViewModel.FileLink);
                var stream = new MemoryStream(fileContent);
                IFormFile formFile = new FormFile(stream, 0, fileContent.Length, "File", summaryViewModel.FileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };
                certificateFileViewModel.FileUploadedSuccessfully = true;
                certificateFileViewModel.File = formFile;
            }
            return View(certificateFileViewModel);
        }

        /// <summary>
        /// upload to s3
        /// </summary>
        /// <param name="certificateFileViewModel"></param>
        /// <returns></returns>

        [HttpPost("certificate-upload")]
        public async Task<IActionResult> SaveCertificate(CertificateFileViewModel certificateFileViewModel, string action)
        {

            bool fromSummaryPage = certificateFileViewModel.FromSummaryPage;

            certificateFileViewModel.FromSummaryPage = false;          
            if  (ModelState["File"].Errors.Count == 0)
            {
                Debug.WriteLine("File not uploaded successfully and no model state errors.");

                using (var memoryStream = new MemoryStream())
                {
                    Debug.WriteLine("Copying file to memory stream.");
                    await certificateFileViewModel.File.CopyToAsync(memoryStream);
                    Debug.WriteLine("File copied to memory stream.");

                    GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, certificateFileViewModel.File.FileName);
                    Debug.WriteLine($"S3 upload response: Success = {genericResponse.Success}");

                    if (genericResponse.Success)
                    {
                        ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
                        summaryViewModel.FileName = certificateFileViewModel.File.FileName;
                        summaryViewModel.FileSizeInKb = Math.Round((decimal)certificateFileViewModel.File.Length / 1024, 1);
                        summaryViewModel.FileLink = genericResponse.Data;
                        certificateFileViewModel.FileUploadedSuccessfully = true;
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                        Debug.WriteLine($"File uploaded successfully: {summaryViewModel.FileName}, Size: {summaryViewModel.FileSizeInKb} KB");
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Unable to upload the file provided");
                        Debug.WriteLine("Error uploading file: Unable to upload the file provided.");
                        return View("CertificateUploadPage", certificateFileViewModel);
                    }
                }
            }

            if (action == "continue")
            {
                Debug.WriteLine("Action is continue.");
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model state is valid. Redirecting...");
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ConfirmityIssueDate");
                }
            }
            else if (action == "draft")
            {
                Debug.WriteLine("Action is draft.");
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model state is valid. Saving as draft and redirecting...");
                    return await SaveAsDraftAndRedirect(GetServiceSummary());
                }
            }

            Debug.WriteLine("Returning to CertificateUploadPage.");
            return View("CertificateUploadPage", certificateFileViewModel);
        }




        /// <summary>
        /// download from s3
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
                    return RedirectToAction(Constants.CabRegistrationErrorPath);
                }
                string contentType = "application/octet-stream";
                return File(fileContent, contentType, filename);
            }
            catch (Exception)
            {
                return RedirectToAction(Constants.CabRegistrationErrorPath);
            }
        }


        #endregion

        #region Conformity issue/expiry dates

        [HttpGet("enter-issue-date")]
        public IActionResult ConfirmityIssueDate(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateViewModel dateViewModel = new DateViewModel();
            dateViewModel.PropertyName = "ConfirmityIssueDate";
            if (summaryViewModel.ConformityIssueDate != null)
            {
                dateViewModel = GetDayMonthYear(summaryViewModel.ConformityIssueDate);
            }

            return View(dateViewModel);
        }



        /// <summary>
        /// Updates confirmity issue date variable in session 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("enter-issue-date")]
        public async Task<IActionResult> SaveConfirmityIssueDate(DateViewModel dateViewModel, string action)
        {
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            dateViewModel.FromSummaryPage = false;
            dateViewModel.PropertyName = "ConfirmityIssueDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityIssueDate = ValidateIssueDate(dateViewModel, summaryViewModel.ConformityExpiryDate, fromSummaryPage);
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate = conformityIssueDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                if (action == "continue")
                {
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ConfirmityExpiryDate");
                }
                else if (action == "draft")
                {
                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);
            }
        }



        [HttpGet("enter-expiry-date")]
        public IActionResult ConfirmityExpiryDate()
        {
            Debug.WriteLine("Entering ConfirmityExpiryDate action.");

            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            Debug.WriteLine("Retrieved ServiceSummaryViewModel.");

            DateViewModel dateViewModel = new DateViewModel();
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            Debug.WriteLine($"Initial DateViewModel PropertyName: {dateViewModel.PropertyName}");

            if (summaryViewModel.ConformityExpiryDate != null)
            {
                Debug.WriteLine($"ConformityExpiryDate found: {summaryViewModel.ConformityExpiryDate}");
                dateViewModel = GetDayMonthYear(summaryViewModel.ConformityExpiryDate);
                Debug.WriteLine($"DateViewModel updated with day, month, year: {dateViewModel.Day}, {dateViewModel.Month}, {dateViewModel.Year}");
            }
            else
            {
                Debug.WriteLine("ConformityExpiryDate is null.");
            }

            return View(dateViewModel);
        }


        /// <summary>
        /// Updates confirmity issue expiry date variable in session 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("enter-expiry-date")]
        public async Task<IActionResult> SaveConfirmityExpiryDate(DateViewModel dateViewModel, string action)
        {
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            DateTime? conformityExpiryDate = ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate));

            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityExpiryDate = conformityExpiryDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                if (action == "continue")
                {
                    return RedirectToAction("ServiceSummary");
                }
                else if (action == "draft")
                {
                    return await SaveAsDraftAndRedirect(summaryViewModel);
                }
                else
                {
                    return RedirectToAction("HandleException", "Error");
                }
            }
            else
            {
                return View("ConfirmityExpiryDate", dateViewModel);
            }
        }

        #endregion

        #region Summary and save to database
        [HttpGet("check-your-answers")]
        public IActionResult ServiceSummary()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return View(summaryViewModel);
        }

        [HttpPost("check-your-answers")]
        public async Task<IActionResult> SaveServiceSummary()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();           
            ServiceDto serviceDto = MapViewModelToDto(summaryViewModel, ServiceStatusEnum.Submitted);
            if(serviceDto!=null)
            {
                GenericResponse genericResponse = await cabService.SaveService(serviceDto, UserEmail);
                if (genericResponse.Success)
                {
                    return RedirectToAction("InformationSubmitted");
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

        /// <summary>
        ///Final page if save success
        /// </summary>       
        /// <returns></returns>
        [HttpGet("service-submitted")]
        public async Task <IActionResult> InformationSubmitted()
        {
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            HttpContext?.Session.Remove("ServiceSummary");
            ViewBag.Email = email;
            await emailSender.SendEmailCabInformationSubmitted(email, email);
            await emailSender.SendCertificateInfoSubmittedToDSIT();
            return View();

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
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

                    foreach(var item in protectionLevels)
                    {
                        qualityLevelViewModel.SelectedLevelOfProtections.Add(item);
                    }

                    foreach (var item in authenticatorLevels)
                    {
                        qualityLevelViewModel.SelectedQualityofAuthenticators.Add(item);
                    }
                   

                }
                if(serviceDto.ServiceIdentityProfileMapping!= null && serviceDto.ServiceIdentityProfileMapping.Count > 0)
                {
                    identityProfileViewModel.SelectedIdentityProfiles = serviceDto.ServiceIdentityProfileMapping.Select(mapping => mapping.IdentityProfile).ToList();
                }
                if(serviceDto.ServiceSupSchemeMapping != null && serviceDto.ServiceSupSchemeMapping.Count>0)
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
                    SupplementarySchemeViewModel= supplementarySchemeViewModel,
                    FileLink = serviceDto.FileLink,
                    FileName = serviceDto.FileName,
                    FileSizeInKb = serviceDto.FileSizeInKb,
                    ConformityIssueDate = serviceDto.ConformityIssueDate,
                    ConformityExpiryDate = serviceDto.ConformityExpiryDate,
                    ServiceId = serviceDto.Id,
                    ProviderProfileId = serviceDto.ProviderProfileId,
                    CabId = cabId,
                    CabUserId = serviceDto.CabUserId
                };
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);

                DateTime minDate = new DateTime(1900, 1, 1);
                if (string.IsNullOrEmpty(serviceSummary.ServiceName))
                {
                    return RedirectToAction("ServiceName", new { providerProfileId = serviceDto.ProviderProfileId}); 
                }
                else if (string.IsNullOrEmpty(serviceSummary.ServiceURL))
                {
                    return RedirectToAction("ServiceURL", new { providerProfileId = serviceDto.ProviderProfileId }); 
                }
                 else if (string.IsNullOrEmpty(serviceSummary.CompanyAddress))
                {
                    return RedirectToAction("CompanyAddress");
                }
                else if (serviceSummary.RoleViewModel.SelectedRoles == null || serviceSummary.RoleViewModel.SelectedRoles.Count == 0)
                {
                    return RedirectToAction("ProviderRoles");
                }
                else if (serviceSummary.HasGPG44 == null)
                {
                    return RedirectToAction("GPG44Input");
                }
                else if (serviceSummary.HasGPG44 == true && (serviceSummary.QualityLevelViewModel.SelectedQualityofAuthenticators.Count == 0 || 
                    serviceSummary.QualityLevelViewModel.SelectedLevelOfProtections.Count == 0))
                {
                    return RedirectToAction("GPG44");
                }
                else if (serviceSummary.HasGPG45 == null)
                {
                    return RedirectToAction("GPG45Input");
                }
                else if (serviceSummary.HasGPG45 == true && serviceSummary.IdentityProfileViewModel.SelectedIdentityProfiles.Count == 0)
                {
                    return RedirectToAction("GPG45");
                }
                else if (serviceSummary.HasSupplementarySchemes == null)
                {
                    return RedirectToAction("HasSupplementarySchemesInput");
                }
                else if (serviceSummary.HasSupplementarySchemes == true && serviceSummary.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Count == 0)
                {
                    return RedirectToAction("SupplementarySchemes");
                }
                else if (serviceSummary.FileName == null)
                {
                    return RedirectToAction("CertificateUploadPage");
                }
                else if (serviceSummary.ConformityIssueDate < minDate)
                {
                    return RedirectToAction("ConfirmityIssueDate"); 
                }
                else if (serviceSummary.ConformityExpiryDate < minDate)
                {
                    return RedirectToAction("ConfirmityExpiryDate");
                }
                else
                {
                    return RedirectToAction("ServiceName", new { providerProfileId = serviceDto.ProviderProfileId });
                }
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }




        }

        #endregion


        #region Private Methods


        private async Task<IActionResult> SaveAsDraftAndRedirect(ServiceSummaryViewModel serviceSummary)
        {
            ServiceDto serviceDto = MapViewModelToDto(serviceSummary, ServiceStatusEnum.SavedAsDraft);
            GenericResponse genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ServiceSummary");
                return RedirectToAction("ProviderServiceDetails", "Cab", new { serviceId = genericResponse.InstanceId });
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }

        }

        private ServiceSummaryViewModel GetServiceSummary()
        {
            

            ServiceSummaryViewModel model = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel
            {
                QualityLevelViewModel = new QualityLevelViewModel { SelectedLevelOfProtections = new List<QualityLevelDto>(), SelectedQualityofAuthenticators = new List<QualityLevelDto>() },
                RoleViewModel = new RoleViewModel { SelectedRoles = new List<RoleDto>() },
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = new List<IdentityProfileDto>() },
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } }
            };
            return model;
        }

        private DateViewModel GetDayMonthYear(DateTime? dateTime)
        {
            DateViewModel dateViewModel = new DateViewModel();
            DateTime conformityIssueDate = Convert.ToDateTime(dateTime);
            dateViewModel.Day = conformityIssueDate.Day;
            dateViewModel.Month = conformityIssueDate.Month;
            dateViewModel.Year = conformityIssueDate.Year;
            return dateViewModel;
        }

        private DateTime? ValidateIssueDate(DateViewModel dateViewModel, DateTime? expiryDate, bool fromSummaryPage)
        {
            DateTime? date = null;
            DateTime minDate = new DateTime(1900, 1, 1);
            DateTime minIssueDate;
           
        
            try
            {
                if (dateViewModel.Day == null || dateViewModel.Month == null ||dateViewModel.Year == null)
                {
                    if (dateViewModel.Day == null)
                    {
                        ModelState.AddModelError("Day", Constants.ConformityIssueDayError);
                    }
                    if (dateViewModel.Month == null)
                    {
                        ModelState.AddModelError("Month", Constants.ConformityIssueMonthError);
                    }
                    if (dateViewModel.Year == null)
                    {
                        ModelState.AddModelError("Year", Constants.ConformityIssueYearError);
                    }
                }
                else
                {
                    date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                    if (date>DateTime.Today)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssuePastDateError);
                    }
                    if (date<minDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);
                    }

                    if (expiryDate.HasValue && fromSummaryPage)
                    {
                        minIssueDate  = expiryDate.Value.AddYears(-2).AddDays(-60);
                        if (date < minIssueDate)
                        {
                            ModelState.AddModelError("ValidDate", Constants.ConformityMaxExpiryDateError);
                        }
                    }
                  
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);

            }
            return date;
        }

        private DateTime? ValidateExpiryDate(DateViewModel dateViewModel, DateTime issueDate)
        {
            DateTime? date = null;     
          
            try
            {
                if (dateViewModel.Day == null || dateViewModel.Month == null ||dateViewModel.Year == null)
                {
                    if (dateViewModel.Day == null)
                    {
                        ModelState.AddModelError("Day", Constants.ConformityExpiryDayError);
                    }
                    if (dateViewModel.Month == null)
                    {
                        ModelState.AddModelError("Month", Constants.ConformityExpiryMonthError);
                    }
                    if (dateViewModel.Year == null)
                    {
                        ModelState.AddModelError("Year", Constants.ConformityExpiryYearError);
                    }
                }
                else
                {
                    date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                    var maxExpiryDate = issueDate.AddYears(2).AddDays(60);
                    if (date <= DateTime.Today)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityExpiryPastDateError);
                    }
                    else if (date <= issueDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateExpiryDateError);
                    }
                    else if (date > maxExpiryDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityMaxExpiryDateError);
                    }
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("ValidDate", Constants.ConformityExpiryDateInvalidError);

            }
            return date;
        }

        private ServiceDto MapViewModelToDto(ServiceSummaryViewModel model, ServiceStatusEnum serviceStatus)
        {


            ServiceDto serviceDto = null;
            if (model!= null   && model.CabUserId>0)
            {
                serviceDto = new ();
                ICollection<ServiceQualityLevelMappingDto> serviceQualityLevelMappings = new List<ServiceQualityLevelMappingDto>();
                ICollection<ServiceRoleMappingDto> serviceRoleMappings = new List<ServiceRoleMappingDto>();
                ICollection<ServiceIdentityProfileMappingDto> serviceIdentityProfileMappings = new List<ServiceIdentityProfileMappingDto>();
                ICollection<ServiceSupSchemeMappingDto> serviceSupSchemeMappings = new List<ServiceSupSchemeMappingDto>();

                foreach (var item in model.QualityLevelViewModel.SelectedQualityofAuthenticators)
                {                   
                        serviceQualityLevelMappings.Add(new ServiceQualityLevelMappingDto { QualityLevelId = item.Id });
                }
                foreach (var item in model.QualityLevelViewModel.SelectedLevelOfProtections)
                {                  
                        serviceQualityLevelMappings.Add(new ServiceQualityLevelMappingDto { QualityLevelId = item.Id });
                }
                foreach (var item in model.RoleViewModel.SelectedRoles)
                {                
                        serviceRoleMappings.Add(new ServiceRoleMappingDto { RoleId = item.Id });
                }
                foreach (var item in model.IdentityProfileViewModel.SelectedIdentityProfiles)
                {                   
                        serviceIdentityProfileMappings.Add(new ServiceIdentityProfileMappingDto { IdentityProfileId = item.Id });
                }
                foreach (var item in model.SupplementarySchemeViewModel.SelectedSupplementarySchemes)
                {               
                        serviceSupSchemeMappings.Add(new ServiceSupSchemeMappingDto { SupplementarySchemeId = item.Id });
                }

                serviceDto.ProviderProfileId = model.ProviderProfileId;
                serviceDto.ServiceName = model.ServiceName;
                serviceDto.WebSiteAddress = model.ServiceURL;
                serviceDto.CompanyAddress = model.CompanyAddress;
                serviceDto.ServiceRoleMapping = serviceRoleMappings;
                serviceDto.ServiceIdentityProfileMapping= serviceIdentityProfileMappings;
                serviceDto.ServiceQualityLevelMapping = serviceQualityLevelMappings;
                serviceDto.HasSupplementarySchemes =  model.HasSupplementarySchemes;
                serviceDto.HasGPG44 = model.HasGPG44;
                serviceDto.HasGPG45 = model.HasGPG45;
                serviceDto.ServiceSupSchemeMapping = serviceSupSchemeMappings;
                serviceDto.FileLink = model.FileLink;
                serviceDto.FileName = model.FileName;
                serviceDto.FileSizeInKb = model.FileSizeInKb??0;
                serviceDto.ConformityIssueDate= Convert.ToDateTime(model.ConformityIssueDate);
                serviceDto.ConformityExpiryDate = Convert.ToDateTime(model.ConformityExpiryDate);
                serviceDto.CabUserId = model.CabUserId;
                serviceDto.ServiceStatus =serviceStatus;
                serviceDto.Id = model.ServiceId;
            }
            return serviceDto;


        }
        #endregion
    }
}