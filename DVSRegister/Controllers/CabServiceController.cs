using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/submit-service")]
    [ValidCognitoToken]
    public class CabServiceController : Controller
    {

        private readonly ICabService cabService;
        private readonly IBucketService bucketService;
        private readonly IUserService userService;

        public CabServiceController(ICabService cabService, IBucketService bucketService, IUserService userService)
        {
            this.cabService = cabService;
            this.bucketService = bucketService;
            this.userService=userService;   
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
        public IActionResult ServiceName(bool fromSummaryPage, int providerProfileId)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            if(providerProfileId > 0) 
            {
                serviceSummaryViewModel.ProviderProfileId = providerProfileId;
                HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
            }
          
            return View(serviceSummaryViewModel);
        }

        [HttpPost("name-of-service")]
        public IActionResult SaveServiceName(ServiceSummaryViewModel serviceSummaryViewModel )
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ServiceName"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.ServiceName = serviceSummaryViewModel.ServiceName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ServiceURL");
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
        public IActionResult SaveServiceURL(ServiceSummaryViewModel serviceSummaryViewModel)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ServiceURL"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.ServiceURL = serviceSummaryViewModel.ServiceURL;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("CompanyAddress");
            }
            else
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
        public IActionResult SaveCompanyAddress(ServiceSummaryViewModel serviceSummaryViewModel)
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            if (ModelState["CompanyAddress"].Errors.Count == 0)
            {
                ServiceSummaryViewModel serviceSummary = GetServiceSummary();
                serviceSummary.CompanyAddress = serviceSummaryViewModel.CompanyAddress;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ProviderRoles");
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
        public async Task<IActionResult> SaveRoles(RoleViewModel roleViewModel)
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
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("GPG44Input");
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
        public IActionResult SaveGPG44Input(ServiceSummaryViewModel viewModel)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasGPG44"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG44 = viewModel.HasGPG44;               
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
        public async Task<IActionResult> SaveGPG44(QualityLevelViewModel qualityLevelViewModel)
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
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("GPG45Input");
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
        public IActionResult SaveGPG45Input(ServiceSummaryViewModel viewModel)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasGPG45"].Errors.Count == 0)
            {
                summaryViewModel.HasGPG45 = viewModel.HasGPG45;
               
                if (Convert.ToBoolean(summaryViewModel.HasGPG45))
                {
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return RedirectToAction("GPG45", new { fromSummaryPage = fromSummaryPage });
                }
                else
                {
                    summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = new List<IdentityProfileDto>();
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("HasSupplementarySchemesInput");
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
            identityProfileViewModel.HasGPG44 = summaryViewModel.HasGPG44;
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
        public async Task<IActionResult> SaveGPG45(IdentityProfileViewModel identityProfileViewModel)
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
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("HasSupplementarySchemesInput");
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
        public IActionResult SaveHasSupplementarySchemesInput(ServiceSummaryViewModel viewModel)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            if (ModelState["HasSupplementarySchemes"].Errors.Count == 0)
            {
                summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;               
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
        public async Task<IActionResult> SaveSupplementarySchemes(SupplementarySchemeViewModel supplementarySchemeViewModel)
        {
            bool fromSummaryPage = supplementarySchemeViewModel.FromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<SupplementarySchemeDto> availableSupplementarySchemes = await cabService.GetSupplementarySchemes();
            supplementarySchemeViewModel.AvailableSchemes = availableSupplementarySchemes;
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds =  supplementarySchemeViewModel.SelectedSupplementarySchemeIds??new List<int>();
            if (supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Count > 0)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = availableSupplementarySchemes.Where(c => supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
            summaryViewModel.SupplementarySchemeViewModel.FromSummaryPage = false;

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
        #endregion

        #region File upload/download

        [HttpGet("certificate-upload")]
        public async Task<IActionResult> CertificateUploadPage(bool fromSummaryPage, bool remove)
        {

            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            CertificateFileViewModel certificateFileViewModel = new CertificateFileViewModel();
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
        public async Task<IActionResult> SaveCertificate(CertificateFileViewModel certificateFileViewModel)
        {
            bool fromSummaryPage = certificateFileViewModel.FromSummaryPage;
            certificateFileViewModel.FromSummaryPage = false;
            if (Convert.ToBoolean(certificateFileViewModel.FileUploadedSuccessfully) == false)
            {
                if (ModelState["File"].Errors.Count == 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await certificateFileViewModel.File.CopyToAsync(memoryStream);
                        GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, certificateFileViewModel.File.FileName);
                        if (genericResponse.Success)
                        {
                            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
                            summaryViewModel.FileName = certificateFileViewModel.File.FileName;
                            summaryViewModel.FileLink = genericResponse.Data;
                            certificateFileViewModel.FileUploadedSuccessfully = true;
                            certificateFileViewModel.FileName = certificateFileViewModel.File.FileName;
                            HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                            return View("CertificateUploadPage", certificateFileViewModel);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "Unable to upload the file provided");
                            return View("CertificateUploadPage", certificateFileViewModel);
                        }
                    }
                }
                else
                {
                    return View("CertificateUploadPage", certificateFileViewModel);
                }
            }
            else
            {
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ConfirmityIssueDate");

            }
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
        public IActionResult SaveConfirmityIssueDate(DateViewModel dateViewModel)
        {
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            dateViewModel.FromSummaryPage = false;
            dateViewModel.PropertyName = "ConfirmityIssueDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityIssueDate = ValidateIssueDate(dateViewModel, summaryViewModel.ConformityExpiryDate, fromSummaryPage);
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate =conformityIssueDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ConfirmityExpiryDate");
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);
            }

        }

        [HttpGet("enter-expiry-date")]
        public IActionResult ConfirmityExpiryDate()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateViewModel dateViewModel = new DateViewModel();
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            if (summaryViewModel.ConformityExpiryDate != null)
            {
                dateViewModel = GetDayMonthYear(summaryViewModel.ConformityExpiryDate);
            }
            return View(dateViewModel);
        }

        /// <summary>
        /// Updates confirmity issue expiry date variable in session 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("enter-expiry-date")]
        public IActionResult SaveConfirmityExpiryDate(DateViewModel dateViewModel)
        {
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityExpiryDate = ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate));
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityExpiryDate = conformityExpiryDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return RedirectToAction("ServiceSummary"); 
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
            ServiceDto serviceDto = MapViewModelToDto(summaryViewModel);
            GenericResponse genericResponse = await cabService.SaveService(serviceDto);
            if (genericResponse.Success)
            {
                return RedirectToAction("InformationSubmitted");
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
        public IActionResult InformationSubmitted()
        {
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            HttpContext?.Session.Remove("ServiceSummary");
            ViewBag.Email = email;
            return View();

        }

        #endregion


        #region Private Methods

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
                        minIssueDate  = expiryDate.Value.AddYears(-2);
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
                    var maxExpiryDate = issueDate.AddYears(2);
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

        private ServiceDto MapViewModelToDto(ServiceSummaryViewModel model)
        {

            ServiceDto serviceDto = new ServiceDto();
            if (model!= null)
            {
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
                serviceDto.ServiceName = model.ServiceName??string.Empty;
                serviceDto.WebsiteAddress = model.ServiceURL??string.Empty;
                serviceDto.CompanyAddress = model.CompanyAddress??string.Empty;
                serviceDto.ServiceRoleMapping = serviceRoleMappings;
                serviceDto.ServiceIdentityProfileMapping= serviceIdentityProfileMappings;
                serviceDto.ServiceQualityLevelMapping = serviceQualityLevelMappings;
                serviceDto.HasSupplementarySchemes = model.HasSupplementarySchemes??false;
                serviceDto.HasGPG44 = model.HasGPG44??false;
                serviceDto.ServiceSupSchemeMapping = serviceSupSchemeMappings;
                serviceDto.FileLink = model.FileLink??string.Empty;
                serviceDto.FileName = model.FileName??string.Empty;
                serviceDto.FileSizeInKb = model.FileSizeInKb??0;
                serviceDto.ConformityIssueDate= Convert.ToDateTime(model.ConformityIssueDate);
                serviceDto.ConformityExpiryDate = Convert.ToDateTime(model.ConformityExpiryDate);
                serviceDto.CabUserId = model.CabUserId;
                serviceDto.ServiceStatus = ServiceStatusEnum.Submitted;
            }
            return serviceDto;


        }
        #endregion
    }
}