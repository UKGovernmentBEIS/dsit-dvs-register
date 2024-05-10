using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility;
using Newtonsoft.Json;
using DVSRegister.Models;


namespace DVSRegister.Controllers
{
    [Route("cab-registration")]
    public class CabController : Controller
    {
    
        private readonly ICabService cabService;
        private readonly IBucketService bucketService;
        private readonly IAVService avService;

        public CabController(ICabService cabService, IAVService aVService, IBucketService bucketService)
        {           
            this.cabService = cabService;
            this.bucketService = bucketService;
            this.avService = aVService;
        }

        [HttpGet("")]
        [HttpGet("landing-page")]
        public IActionResult LandingPage()
        {
            return View();
        }


        [HttpGet("validate-urn-for-application")]
        public IActionResult ValidateURNForApplication()
        {
            return View();
        }

        [HttpPost("validate-urn-for-application-post")]
        public async Task<IActionResult> URNValidationForApplication(string urnNumber)
        {
            DVSRegister.Data.Entities.PreRegistration URNDetails = await cabService.GetURNDetails(urnNumber);

            if(URNDetails != null)
            {
                URNViewModel viewModel = new URNViewModel();
                viewModel.RegisteredName = URNDetails.RegisteredCompanyName;
                viewModel.TradingName = URNDetails.TradingName;
                viewModel.URN = URNDetails.URN;
                HttpContext?.Session.Set("URNViewModel", viewModel);
                return View("URNData", viewModel);
            }
            else
            {
                return View("ValidateURNForApplication");
            }
        }

        [HttpGet("check-information")]
        public IActionResult CheckInformation()
        {
            return View("InformationCheckStart");
        }

        [HttpGet("registered-name")]
        public IActionResult RegisteredName()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("RegisteredName", certificateInfoSummaryViewModel);
        }

        [HttpGet("urn-data")]
        public IActionResult URNData()
        {
            URNViewModel viewModel = HttpContext?.Session.Get<URNViewModel>("URNViewModel");
            return View("URNData", viewModel);
        }

        [HttpGet("trading-name")]
        public IActionResult TradingName()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("TradingName", certificateInfoSummaryViewModel);
        }

        [HttpGet("public-contact-email")]
        public IActionResult PublicContactEmail()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("PublicContactEmail", certificateInfoSummaryViewModel);
        }

        [HttpGet("telephone-number")]
        public IActionResult TelephoneNumber()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("TelephoneNumber", certificateInfoSummaryViewModel);
        }

        [HttpGet("website-address")]
        public IActionResult WebsiteAddress()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("WebsiteAddress", certificateInfoSummaryViewModel);
        }

        [HttpGet("company-address")]
        public IActionResult CompanyAddress()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("CompanyAddress", certificateInfoSummaryViewModel);
        }

        [HttpGet("service-name")]
        public IActionResult ServiceName()
        {
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("ServiceName", certificateInfoSummaryViewModel);
        }

        [HttpPost("registered-name-validation")]
        public IActionResult RegisteredNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            
            ModelState.Remove(nameof(certificateInfoSummaryViewModel.TradingName));
            if (certificateInfoSummaryViewModel.RegisteredName != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.RegisteredName = certificateInfoSummaryViewModel.RegisteredName;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("TradingName");
            }
            else
            {
                ModelState.AddModelError("RegisteredName", "Error in the registered name");
                return View("RegisteredName", certificateInfoSummaryViewModel);
            }
        }

        [HttpPost("trading-name-validation")]
        public IActionResult TradingNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            ModelState.Remove(nameof(certificateInfoSummaryViewModel.PublicContactEmail));
            
            if(certificateInfoSummaryViewModel.TradingName != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.TradingName = certificateInfoSummaryViewModel.TradingName;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("PublicContactEmail");
            }
            else
            {
                ModelState.AddModelError("TradingName", "Error in the registered name");
                return View("TradingName", certificateInfoSummaryViewModel);
            }
        }

        [HttpPost("public-email-validation")]
        public IActionResult PublicContactEmailValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            ModelState.Remove(nameof(CertificateInfoSummaryViewModel.TelephoneNumber));
            if(ModelState["PublicContactEmail"].Errors.Count == 0 && certificateInfoSummaryViewModel.PublicContactEmail != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.PublicContactEmail = certificateInfoSummaryViewModel.PublicContactEmail;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("TelephoneNumber", certificateInfoSummaryViewModel);
            }
            else
            {
                ModelState.AddModelError("PublicEmail", "Error in the public contact email");
                return View("PublicContactEmail", certificateInfoSummaryViewModel);
            }
            
        }

        [HttpPost("telephone-number-validation")]
        public IActionResult TelephoneNumberValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            
            if (ModelState["TelephoneNumber"].Errors.Count ==0 && certificateInfoSummaryViewModel.TelephoneNumber != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.TelephoneNumber = certificateInfoSummaryViewModel.TelephoneNumber;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("WebsiteAddress", certificateInfoSummaryViewModel);
            }
            else
            {
                ModelState.AddModelError("TelephoneNumber", "Error in the telephone number validation");
                return View("TelephoneNumber", certificateInfoSummaryViewModel);
            }
        }

        [HttpPost("website-address-validation")]
        public IActionResult WebsiteAddressValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            ModelState.Remove(nameof(CertificateInfoSummaryViewModel.Address));
            if (ModelState["WebsiteAddress"].Errors.Count == 0 && certificateInfoSummaryViewModel.WebsiteAddress != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.WebsiteAddress = certificateInfoSummaryViewModel.WebsiteAddress;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("CompanyAddress", certificateInfoSummaryViewModel);
            }
            else
            {
                ModelState.AddModelError("WebsiteAddress", "Error in the validation");
                return View("WebsiteAddress", certificateInfoSummaryViewModel);
            }
        }

        [HttpPost("company-address-validation")]
        public IActionResult CompanyAddressValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {

            if (ModelState["Address"].Errors.Count == 0 && certificateInfoSummaryViewModel.Address != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.Address = certificateInfoSummaryViewModel.Address;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("ServiceName", certificateInfoSummaryViewModel);
            }
            else
            {
                ModelState.AddModelError("Address", "Error in the validation");
                return View("CompanyAddress", certificateInfoSummaryViewModel);
            }
        }

        [HttpPost("service-name-validation")]
        public IActionResult ServiceNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {

            if (ModelState["ServiceName"].Errors.Count == 0 && certificateInfoSummaryViewModel.ServiceName != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.Address = certificateInfoSummaryViewModel.Address;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return RedirectToAction("SelectRoles", certificateInfoSummaryViewModel);
            }
            else
            {
                ModelState.AddModelError("ServiceName", "Error in the validation");
                return View("ServiceName", certificateInfoSummaryViewModel);
            }
        }

        #region Validate URN
        /// <summary>
        /// Loaded on 
        /// Check Unique Number Link
        /// clicked on landing page
        /// </summary>
        /// <returns></returns>

        [HttpGet("check-urn-start")]
        public IActionResult CheckURNStartPage()
        {
            return View();
        }

        [HttpGet("check-urn")]
        public IActionResult CheckURN()
        {
            return View();
        }

        [HttpPost("check-urn")]
        public async Task<IActionResult> ValidateURN(URNViewModel urnViewModel)
        {
            if(!string.IsNullOrEmpty(urnViewModel.URN)) 
            {
                bool isValid = await cabService.ValidateURN(urnViewModel.URN);
                if (!isValid)
                    ModelState.AddModelError("URN", Constants.URNErrorMessage);
            }
           
            if (ModelState.IsValid)
            {
                TempData["URN"] = urnViewModel.URN;
                return RedirectToAction("ValidURNDetails");
            }
            else
            {
                return View("CheckURN", urnViewModel);
            }
        }

        [HttpGet("valid-urn")]
        public async Task<IActionResult> ValidURNDetails()
        {
            string URN = TempData["URN"] as string;
            PreRegistrationDto preRegistrationDto = await cabService.GetPreRegistrationDetails(URN);
            URNViewModel urnViewModel = MapDtoToViewModel(preRegistrationDto);
            return View(urnViewModel);
        }

        #endregion



        [HttpGet("certificate-information")]
        public IActionResult CertificateInformationStartPage()
        {
            return View();
        }


        [HttpGet("select-roles")]
        public async Task<IActionResult> SelectRoles(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
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
        [HttpPost("select-roles")]
        public async Task<IActionResult> SaveRoles(RoleViewModel roleViewModel)
        {
            bool fromSummaryPage = roleViewModel.FromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            List<RoleDto> availableRoles = JsonConvert.DeserializeObject<List<RoleDto>>(HttpContext.Request.Form["AvailableRoles"]);
            if (availableRoles == null  || availableRoles.Count == 0)
                availableRoles = await cabService.GetRoles();
            roleViewModel.AvailableRoles = availableRoles;
            roleViewModel.SelectedRoleIds =  roleViewModel.SelectedRoleIds??new List<int>();
            if (roleViewModel.SelectedRoleIds.Count > 0)
                summaryViewModel.RoleViewModel.SelectedRoles = availableRoles.Where(c => roleViewModel.SelectedRoleIds.Contains(c.Id)).ToList();
            summaryViewModel.RoleViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("SelectIdentityProfiles");
            }
            else
            {
                return View("SelectRoles", roleViewModel);
            }
        }


        /// <summary>
        ///  select-identity-profiles
        /// </summary>
        /// <param name="fromSummaryPage"></param>
        /// <returns></returns>

        [HttpGet("select-identity-profiles")]
        public async Task<IActionResult> SelectIdentityProfiles(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            IdentityProfileViewModel roleViewModel = new IdentityProfileViewModel();
            roleViewModel.SelectedIdentityProfileIds = summaryViewModel?.IdentityProfileViewModel?.SelectedIdentityProfiles?.Select(c => c.Id).ToList();
            roleViewModel.AvailableIdentityProfiles = await cabService.GetIdentityProfiles();
            return View(roleViewModel);
        }

        /// <summary>
        /// Save selected Identity Profiles to session
        /// </summary>
        /// <param name="identityProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost("select-identity-profiles")]
        public async Task<IActionResult> SaveIdentityProfiles(IdentityProfileViewModel identityProfileViewModel)
        {
            bool fromSummaryPage = identityProfileViewModel.FromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            List<IdentityProfileDto> availableIdentityProfiles = JsonConvert.DeserializeObject<List<IdentityProfileDto>>(HttpContext.Request.Form["AvailableIdentityProfiles"]);
            if (availableIdentityProfiles == null  || availableIdentityProfiles.Count == 0)
                availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds =  identityProfileViewModel.SelectedIdentityProfileIds??new List<int>();
            if (identityProfileViewModel.SelectedIdentityProfileIds.Count > 0)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();
            summaryViewModel.IdentityProfileViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("HasSupplementaryScheme");
            }
            else
            {
                return View("SelectIdentityProfiles", identityProfileViewModel);
            }
        }

        [HttpGet("has-supplementary-scheme")]
        public IActionResult HasSupplementaryScheme()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        /// Updates HasSupplementarySchemes variable in session
        /// and redirect based on HasSupplementarySchemes value
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("has-supplementary-scheme")]
        public IActionResult SaveHasSupplementaryScheme(CertificateInfoSummaryViewModel viewModel)
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();

            if (viewModel.HasSupplementarySchemes == null)
                ModelState.AddModelError("HasSupplementarySchemes", Constants.SupplementarySchemeErrorMessage);
            if (ModelState["HasSupplementarySchemes"].Errors.Count == 0)
            {
                summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                if (Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                {
                    //Provide Contact details
                    return RedirectToAction("SelectSupplementarySchemes");
                }
                else
                {

                    return RedirectToAction("CertificateUploadPage");
                }
            }
            return View("HasSupplementaryScheme", viewModel);
        }


        /// <summary>
        /// select supplementary schemes
        /// </summary>
        /// <param name="fromSummaryPage"></param>
        /// <returns></returns>

        [HttpGet("select-supplementary-schemes")]
        public async Task<IActionResult> SelectSupplementarySchemes(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            SupplementarySchemeViewModel supplementarySchemeViewModel = new SupplementarySchemeViewModel();
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(c => c.Id).ToList();
            supplementarySchemeViewModel.AvailableSchemes = await cabService.GetSupplementarySchemes();
            return View(supplementarySchemeViewModel);
        }

        /// <summary>
        /// Save selected supplementary schemes to session
        /// </summary>
        /// <param name="supplementarySchemeViewModel"></param>
        /// <returns></returns>
        [HttpPost("select-supplementary-schemes")]
        public async Task<IActionResult> SaveSupplementarySchemes(SupplementarySchemeViewModel supplementarySchemeViewModel)
        {
            bool fromSummaryPage = supplementarySchemeViewModel.FromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            List<SupplementarySchemeDto> availableSupplementarySchemes = JsonConvert.DeserializeObject<List<SupplementarySchemeDto>>(HttpContext.Request.Form["AvailableSchemes"]);
            if (availableSupplementarySchemes == null  || availableSupplementarySchemes.Count == 0)
                availableSupplementarySchemes = await cabService.GetSupplementarySchemes();
            supplementarySchemeViewModel.AvailableSchemes = availableSupplementarySchemes;
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds =  supplementarySchemeViewModel.SelectedSupplementarySchemeIds??new List<int>();
            if (supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Count > 0)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = availableSupplementarySchemes.Where(c => supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
            summaryViewModel.SupplementarySchemeViewModel.FromSummaryPage = false;

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("CertificateInfoSummary");
            }
            else
            {
                return View("SelectSupplementarySchemes", supplementarySchemeViewModel);
            }
        }


        [HttpGet("certificate-upload")]
        public IActionResult CertificateUploadPage()
        {
            return View();
        }

        [HttpPost("upload-certificate")]
        public async Task<IActionResult> SaveCertificate(CertificateFileViewModel certificateFileViewModel)
        {
            // Virus Scan
            // Upload to S3

            // Store the filename and link in Session
            if (ModelState["File"].Errors.Count == 0)
            {
                using (var memoryStream = new MemoryStream())
                {

                    await certificateFileViewModel.File.CopyToAsync(memoryStream);
                    //TODO:  uncomment service call once aws provisioned
                    GenericResponse avServiceResponse = new GenericResponse { Success = true };
                    // GenericResponse avServiceResponse = avService.ScanFileForVirus(memoryStream);

                    if (avServiceResponse.Success)
                    {
                        //TODO:  uncomment service call once aws provisioned
                        GenericResponse genericResponse = new GenericResponse { Success = true, Data = "YotiCertificate.pdf" };
                        // GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, certificateFileViewModel.File.FileName);
                        if (genericResponse.Success)
                        {
                            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
                            summaryViewModel.FileName = certificateFileViewModel.FileName;
                            summaryViewModel.FileLink = certificateFileViewModel.FileUrl;
                            
                            certificateFileViewModel.FileUploadedSuccessfully = true;
                            certificateFileViewModel.FileName = certificateFileViewModel.File.FileName;
                            return View("CertificateUploadPage", certificateFileViewModel);

                        }
                        else
                        {
                            ModelState.AddModelError("File", "Unable to upload the file provided");
                            return View("CertificateUploadPage", certificateFileViewModel);
                        }
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

        [HttpGet("confirmity-issue-date")]
        public IActionResult ConfirmityIssueDate()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            DateViewModel dateViewModel = new DateViewModel();
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
        [HttpPost("confirmity-issue-date")]
        public IActionResult SaveConfirmityIssueDate(DateViewModel dateViewModel)
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            DateTime? conformityIssueDate = ValidateDate(dateViewModel);
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate =conformityIssueDate;
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return RedirectToAction("ConfirmityExpiryDate");
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);

            }

        }

        [HttpGet("certificate-confirmity-expiry-date")]
        public IActionResult ConfirmityExpiryDate()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            DateViewModel dateViewModel = new DateViewModel();
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
        [HttpPost("confirmity-expiry-date")]
        public IActionResult SaveConfirmityExpiryDate(DateViewModel dateViewModel)
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            DateTime? conformityExpiryDate = ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate));
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityExpiryDate = conformityExpiryDate;
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return RedirectToAction("CertificateInfoSummary");
            }
            else
            {
                return View("ConfirmityExpiryDate", dateViewModel);

            }
        }



        /// <summary>
        /// Summary page displaying data saved in session
        /// </summary>      
        /// <returns></returns>
        [HttpGet("check-your-answers")]
        public IActionResult CertificateInfoSummary()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();          
            return View(summaryViewModel);
        }

        /// <summary>
        /// Get data from session, convert to dto and save
        /// to database on Confirm click
        /// </summary>
        /// <param name="summaryViewModel"></param>
        /// <returns></returns>

        [HttpPost("check-your-answers")]
        public async Task<IActionResult> SaveSummaryAndSubmit()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            CertificateInfoDto certificateInfoDto = MapViewModelToDto(summaryViewModel);
            GenericResponse genericResponse = await cabService.SaveCertificateInformation(certificateInfoDto);
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
        [HttpGet("information-submitted")]
        public IActionResult InformationSubmitted()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        ///Return to landing page
        /// </summary>       
        /// <returns></returns>
        [HttpPost("return-to-service")]
        public IActionResult ReturnToService()
        {
            return RedirectToAction("LandingPage");
        }


        #region Private methods
        private CertificateInfoSummaryViewModel GetCertificateInfoSummary()
        {
            CertificateInfoSummaryViewModel model = HttpContext?.Session.Get<CertificateInfoSummaryViewModel>("CertificateInfoSummary") ?? new CertificateInfoSummaryViewModel
            {
                TradingName = string.Empty,
                RoleViewModel = new RoleViewModel { SelectedRoles = new List<RoleDto>() },
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = new List<IdentityProfileDto>() },
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } }
            };
            return model;
        }


        private URNViewModel MapDtoToViewModel(PreRegistrationDto preRegistrationDto)
        {
            URNViewModel urnViewModel = new URNViewModel();
            urnViewModel.PreregistrationId = preRegistrationDto.Id;
            urnViewModel.TradingName = preRegistrationDto?.TradingName;
            urnViewModel.RegisteredName = preRegistrationDto?.RegisteredCompanyName;
            urnViewModel.URN = preRegistrationDto?.URN;
            return urnViewModel;

        }

        private CertificateInfoDto MapViewModelToDto(CertificateInfoSummaryViewModel model)
        {
            CertificateInfoDto certificateInfoDto = new CertificateInfoDto();
            ICollection<CertificateInfoRoleMappingDto> certificateInfoRoleMappings = new List<CertificateInfoRoleMappingDto>();
            ICollection<CertificateInfoIdentityProfileMappingDto> certificateInfoIdentityProfileMappings = new List<CertificateInfoIdentityProfileMappingDto>();
            ICollection<CertificateInfoSupSchemeMappingDto> certificateInfoSupSchemeMappings = new List<CertificateInfoSupSchemeMappingDto>();
            foreach (var item in model.RoleViewModel.SelectedRoles)
            {
                certificateInfoRoleMappings.Add(new CertificateInfoRoleMappingDto { RoleId = item.Id });
            }
            foreach (var item in model.IdentityProfileViewModel.SelectedIdentityProfiles)
            {
                certificateInfoIdentityProfileMappings.Add(new CertificateInfoIdentityProfileMappingDto { IdentityProfileId = item.Id });
            }
            foreach (var item in model.SupplementarySchemeViewModel.SelectedSupplementarySchemes)
            {
                certificateInfoSupSchemeMappings.Add(new CertificateInfoSupSchemeMappingDto { SupplementarySchemeId = item.Id });
            }
            certificateInfoDto.PreRegistrationId= Convert.ToInt32(model.PreRegistrationId);
            certificateInfoDto.RegisteredName = model.RegisteredName??string.Empty;
            certificateInfoDto.TradingName = model.TradingName??string.Empty;
            certificateInfoDto.PublicContactEmail = model.PublicContactEmail??string.Empty;
            certificateInfoDto.TelephoneNumber = model.TelephoneNumber??string.Empty;
            certificateInfoDto.WebsiteAddress = model.WebsiteAddress??string.Empty;
            certificateInfoDto.Address = model.Address??string.Empty;
            certificateInfoDto.ServiceName = model.ServiceName??string.Empty;
            certificateInfoDto.CertificateInfoRoleMappings = certificateInfoRoleMappings;
            certificateInfoDto.CertificateInfoIdentityProfileMappings = certificateInfoIdentityProfileMappings;
            certificateInfoDto.HasSupplementarySchemes = Convert.ToBoolean(model.HasSupplementarySchemes);
            certificateInfoDto.CertificateInfoSupSchemeMappings = certificateInfoSupSchemeMappings;
            certificateInfoDto.FileName = model.FileName??string.Empty;
            certificateInfoDto.FileLink = model.FileLink?? string.Empty;
            certificateInfoDto.ConformityIssueDate = Convert.ToDateTime(model.ConformityIssueDate);
            certificateInfoDto.ConformityExpiryDate = Convert.ToDateTime(model.ConformityExpiryDate);
            certificateInfoDto.CreatedDate = DateTime.UtcNow;
            certificateInfoDto.CertificateInfoStatus = CertificateInfoStatusEnum.Received;
            return certificateInfoDto;

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

        private DateTime? ValidateDate(DateViewModel dateViewModel)
        {
            DateTime? date = null;
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
        #endregion
    }
}
