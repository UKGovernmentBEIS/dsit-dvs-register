﻿using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility;
using Newtonsoft.Json;


namespace DVSRegister.Controllers
{
    [Route("cab-service")]
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
            if (!string.IsNullOrEmpty(urnViewModel.URN))
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


        #region Submit certificate info
        [HttpGet("submit-certificate-information")]
        public IActionResult CertificateInformationStartPage()
        {
            return View();
        }


        [HttpGet("submit-certificate-information/enter-unique-reference-number")]
        public IActionResult ValidateURNForApplication()
        {
            return View();
        }

        [HttpPost("submit-certificate-information/enter-unique-reference-number")]
        public async Task<IActionResult> URNValidationForApplication(URNViewModel urnViewModel)
        {
            if (string.IsNullOrEmpty(urnViewModel.URN))
            {
                ModelState.AddModelError("URN", Constants.URNRequiredError);
                return View("ValidateURNForApplication");
            }
            else
            {
                DVSRegister.Data.Entities.PreRegistration URNDetails = await cabService.GetURNDetails(urnViewModel.URN);
                CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
                if(URNDetails == null)
                {
                    ModelState.AddModelError("URN", Constants.URNErrorMessage);
                    return View("ValidateURNForApplication");
                }
                else
                {
                    certificateInfoSummaryViewModel.PreRegistrationId = URNDetails.Id;
                    bool isValidURN = await cabService.CheckURNValidatedByCab(urnViewModel.URN);
                    HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoSummaryViewModel);
                    if (!isValidURN)
                        ModelState.AddModelError("URN", Constants.URNErrorMessage);
                    if (ModelState.IsValid)
                    {
                        URNViewModel viewModel = new URNViewModel();
                        viewModel.RegisteredName = URNDetails.RegisteredCompanyName;
                        viewModel.TradingName = URNDetails.TradingName;
                        viewModel.URN = URNDetails.URN;
                        HttpContext?.Session.Set("URNViewModel", viewModel);
                        return RedirectToAction("URNData");
                    }
                    else
                    {
                        return View("ValidateURNForApplication");
                    }
                }
                   
                
            }
           
        }
        [HttpGet("submit-certificate-information/data-associated-with-service-provider")]
        public IActionResult URNData()
        {
            URNViewModel viewModel = HttpContext?.Session.Get<URNViewModel>("URNViewModel");
            return View("URNData", viewModel);
        }

        [HttpGet("submit-certificate-information/check-that-you-have-all-information")]
        public IActionResult CheckInformation()
        {
            return View("InformationCheckStart");
        }

        [HttpGet("submit-certificate-information/service-providers-registered-name")]
        public IActionResult RegisteredName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("RegisteredName", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-registered-name")]
        public IActionResult RegisteredNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;           
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState.ContainsKey("RegisteredName") && ModelState["RegisteredName"].Errors.Count == 0)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.RegisteredName = certificateInfoSummaryViewModel.RegisteredName;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("TradingName");
            }
            else
            {               
                return View("RegisteredName", certificateInfoSummaryViewModel);
            }
        }



        [HttpGet("submit-certificate-information/service-providers-trading-name")]
        public IActionResult TradingName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("TradingName", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-trading-name")]
        public IActionResult TradingNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;           
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState.ContainsKey("TradingName") && ModelState["TradingName"].Errors.Count == 0)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.TradingName = certificateInfoSummaryViewModel.TradingName;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("PublicContactEmail");

            }
            else
            {
                return View("TradingName", certificateInfoSummaryViewModel);
            }
        }

        [HttpGet("submit-certificate-information/service-providers-public-contact-email")]
        public IActionResult PublicContactEmail(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("PublicContactEmail", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-public-contact-email")]
        public IActionResult PublicContactEmailValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;            
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState["PublicContactEmail"].Errors.Count == 0 && certificateInfoSummaryViewModel.PublicContactEmail != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.PublicContactEmail = certificateInfoSummaryViewModel.PublicContactEmail;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("TelephoneNumber");
            }
            else
            {                
                return View("PublicContactEmail", certificateInfoSummaryViewModel);
            }

        }

        [HttpGet("submit-certificate-information/service-providers-telephone-number")]
        public IActionResult TelephoneNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("TelephoneNumber", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-telephone-number")]
        public IActionResult TelephoneNumberValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState["TelephoneNumber"].Errors.Count ==0 && certificateInfoSummaryViewModel.TelephoneNumber != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.TelephoneNumber = certificateInfoSummaryViewModel.TelephoneNumber;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("WebsiteAddress");
            }
            else
            {               
                return View("TelephoneNumber", certificateInfoSummaryViewModel);
            }
        }

        [HttpGet("submit-certificate-information/service-providers-website-address")]
        public IActionResult WebsiteAddress(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("WebsiteAddress", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-website-address")]
        public IActionResult WebsiteAddressValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;
            certificateInfoSummaryViewModel.FromSummaryPage = false;           
            if (ModelState["WebsiteAddress"].Errors.Count == 0 && certificateInfoSummaryViewModel.WebsiteAddress != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.WebsiteAddress = certificateInfoSummaryViewModel.WebsiteAddress;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("CompanyAddress");
            }
            else
            {
                return View("WebsiteAddress", certificateInfoSummaryViewModel);
            }
        }

        [HttpGet("submit-certificate-information/service-providers-company-address")]
        public IActionResult CompanyAddress(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("CompanyAddress", certificateInfoSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-company-address")]
        public IActionResult CompanyAddressValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState["Address"].Errors.Count == 0 && certificateInfoSummaryViewModel.Address != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.Address = certificateInfoSummaryViewModel.Address;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("ServiceName");
            }
            else
            {
                return View("CompanyAddress", certificateInfoSummaryViewModel);
            }
        }

        [HttpGet("submit-certificate-information/service-providers-service-name")]
        public IActionResult ServiceName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel certificateInfoSummaryViewModel = GetCertificateInfoSummary();
            return View("ServiceName", certificateInfoSummaryViewModel);
        }       

        [HttpPost("submit-certificate-information/service-providers-service-name")]
        public IActionResult ServiceNameValidation(CertificateInfoSummaryViewModel certificateInfoSummaryViewModel)
        {
            bool fromSummaryPage = certificateInfoSummaryViewModel.FromSummaryPage;
            certificateInfoSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ServiceName"].Errors.Count == 0 && certificateInfoSummaryViewModel.ServiceName != null)
            {
                CertificateInfoSummaryViewModel certificateInfoModelSoFar = GetCertificateInfoSummary();
                certificateInfoModelSoFar.ServiceName = certificateInfoSummaryViewModel.ServiceName;
                HttpContext?.Session.Set("CertificateInfoSummary", certificateInfoModelSoFar);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("SelectRoles");               
            }
            else
            {
                return View("ServiceName", certificateInfoSummaryViewModel);
            }
        } 

        [HttpGet("submit-certificate-information/service-providers-roles")]
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
        [HttpPost("submit-certificate-information/service-providers-roles")]
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

        [HttpGet("submit-certificate-information/service-providers-identity-profiles")]
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
        [HttpPost("submit-certificate-information/service-providers-identity-profiles")]
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

        [HttpGet("submit-certificate-information/service-providers-supplementary-schemes")]
        public IActionResult HasSupplementaryScheme(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        /// Updates HasSupplementarySchemes variable in session
        /// and redirect based on HasSupplementarySchemes value
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("submit-certificate-information/service-providers-supplementary-schemes")]
        public IActionResult SaveHasSupplementaryScheme(CertificateInfoSummaryViewModel viewModel)
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            //if (viewModel.HasSupplementarySchemes == null)
            //    ModelState.AddModelError("HasSupplementarySchemes", Constants.SupplementarySchemeErrorMessage);
            if (ModelState["HasSupplementarySchemes"].Errors.Count == 0)
            {
                summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                if (Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                {                    
                    return RedirectToAction("SelectSupplementarySchemes", new { fromSummaryPage = fromSummaryPage });
                }
                else
                {
                    return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("CertificateUploadPage");                  
                }
            }
            else
            {
                return View("HasSupplementaryScheme", viewModel);
            }
            
        }


        /// <summary>
        /// select supplementary schemes
        /// </summary>
        /// <param name="fromSummaryPage"></param>
        /// <returns></returns>

        [HttpGet("submit-certificate-information/select-service-providers-supplementary-schemes")]
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
        [HttpPost("submit-certificate-information/select-service-providers-supplementary-schemes")]
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
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("CertificateUploadPage");
            }
            else
            {
                return View("SelectSupplementarySchemes", supplementarySchemeViewModel);
            }
        }


        [HttpGet("submit-certificate-information/upload-certificate-of-conformity")]
        public IActionResult CertificateUploadPage(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            ViewBag.HasSupplementarySchemes = summaryViewModel.HasSupplementarySchemes;
            return View();

        }

        [HttpPost("submit-certificate-information/upload-certificate-of-conformity")]
        public async Task<IActionResult> SaveCertificate(CertificateFileViewModel certificateFileViewModel)
        {
            // Virus Scan
            // Upload to S3
            bool fromSummaryPage = certificateFileViewModel.FromSummaryPage;
            certificateFileViewModel.FromSummaryPage = false;
            // Store the filename and link in Session
            if (Convert.ToBoolean(certificateFileViewModel.FileUploadedSuccessfully) == false)
            {
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
                                summaryViewModel.FileName = certificateFileViewModel.File.FileName;
                                summaryViewModel.FileLink = genericResponse.Data;

                                certificateFileViewModel.FileUploadedSuccessfully = true;                               
                                certificateFileViewModel.FileName = certificateFileViewModel.File.FileName;
                                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
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
            else
            {
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("ConfirmityIssueDate");
               
            }
        }

        [HttpGet("submit-certificate-information/certificate-of-confirmity-issue-date")]
        public IActionResult ConfirmityIssueDate(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
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
        [HttpPost("submit-certificate-information/certificate-of-confirmity-issue-date")]
        public IActionResult SaveConfirmityIssueDate(DateViewModel dateViewModel)
        {
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            dateViewModel.FromSummaryPage = false;
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            DateTime? conformityIssueDate = ValidateIssueDate(dateViewModel);
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate =conformityIssueDate;
                HttpContext?.Session.Set("CertificateInfoSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("CertificateInfoSummary") : RedirectToAction("ConfirmityExpiryDate");
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);
            }

        }

        [HttpGet("submit-certificate-information/certificate-of-confirmity-expiry-date")]
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
        [HttpPost("submit-certificate-information/certificate-of-confirmity-expiry-date")]
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
        [HttpGet("submit-certificate-information/check-your-answers")]
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

        [HttpPost("submit-certificate-information/check-your-answers")]
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
        [HttpGet("submit-certificate-information/information-submitted")]
        public IActionResult InformationSubmitted()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        ///Return to landing page
        /// </summary>       
        /// <returns></returns>
        [HttpPost("submit-certificate-information/return-to-service")]
        public IActionResult ReturnToService()
        {
            return RedirectToAction("LandingPage");
        }
        #endregion

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

        private DateTime? ValidateIssueDate(DateViewModel dateViewModel)
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
