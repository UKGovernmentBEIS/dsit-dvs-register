using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/submit-service")]
  
    public class CabServiceController(ICabService cabService, IBucketService bucketService, IUserService userService, CabEmailSender emailSender, ILogger<CabServiceController> logger, IMapper mapper) : BaseController(logger)
    {

        private readonly ICabService cabService = cabService;
        private readonly IBucketService bucketService = bucketService;
        private readonly IUserService userService = userService;
        private readonly CabEmailSender emailSender = emailSender;
        private readonly ILogger<CabServiceController> _logger = logger;
        private readonly IMapper _mapper = mapper;

        [HttpGet("before-you-start/{providerProfileId}")]
        public async Task<IActionResult> BeforeYouStart(int providerProfileId)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            ViewBag.ProviderProfileId = providerProfileId;
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);

            if (!IsValidCabId(cabUserDto.Id))
                return HandleInvalidCabId(cabUserDto.Id);

            await HandleInvalidProfileAndCab(providerProfileId, cabUserDto);
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.CabId = cabUserDto.CabId;
            serviceSummaryViewModel.CabUserId = cabUserDto.Id;
            serviceSummaryViewModel.ProviderProfileId = providerProfileId;
            HttpContext?.Session.Set("ServiceSummary", serviceSummaryViewModel);
            return View();            

        }       


        #region Service Name
        [HttpGet("name-of-service/{fromSummaryPage?}/{fromDetailsPage?}")]
        public IActionResult ServiceName(bool fromSummaryPage = false, bool fromDetailsPage = false)
        {           
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;          
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage 
           ? GetRefererURL() : "/cab-service/submit-service/tf-version/" + serviceSummaryViewModel.ProviderProfileId;
            return View(serviceSummaryViewModel);

        }

        [HttpPost("name-of-service")]
        public async Task<IActionResult> SaveServiceName(ServiceSummaryViewModel serviceSummaryViewModel, string action )
        {
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["ServiceName"]?.Errors.Count == 0)
            {               
                serviceSummary.ServiceName = serviceSummaryViewModel.ServiceName;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, "ServiceURL");  
            }
            else
            {
                return View("ServiceName", serviceSummaryViewModel);
            }
        }
        #endregion

        #region Service URL
        [HttpGet("service-url/{fromSummaryPage?}/{fromDetailsPage?}")]
        public IActionResult ServiceURL(bool fromSummaryPage=false, bool fromDetailsPage = false)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/name-of-service";
            return View("ServiceURL", serviceSummaryViewModel);
        }
        [HttpPost("service-url")]
        public async Task<IActionResult> SaveServiceURL(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {           
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["ServiceURL"]?.Errors.Count == 0)
            {
                serviceSummary.ServiceURL = serviceSummaryViewModel.ServiceURL;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, "CompanyAddress");               
            }
            else
            {
                return View("ServiceURL", serviceSummaryViewModel);
            }
        }

     
        #endregion

        #region Company Address
        [HttpGet("company-address")]
        public IActionResult CompanyAddress(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel serviceSummaryViewModel = GetServiceSummary();
            serviceSummaryViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/service-url";
            return View("CompanyAddress", serviceSummaryViewModel);
        }
        [HttpPost("company-address")]
        public async Task <IActionResult> SaveCompanyAddress(ServiceSummaryViewModel serviceSummaryViewModel, string action)
        {
            ServiceSummaryViewModel serviceSummary = GetServiceSummary();
            bool fromSummaryPage = serviceSummaryViewModel.FromSummaryPage;
            bool fromDetailsPage = serviceSummaryViewModel.FromDetailsPage;
            serviceSummaryViewModel.FromSummaryPage = false;
            serviceSummaryViewModel.FromDetailsPage = false;
            serviceSummaryViewModel.IsAmendment = serviceSummary.IsAmendment;
            if (ModelState["CompanyAddress"].Errors.Count == 0)
            {
               
                serviceSummary.CompanyAddress = serviceSummaryViewModel.CompanyAddress;
                HttpContext?.Session.Set("ServiceSummary", serviceSummary);
                return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, "ProviderRoles");      
            }
            else
            {
                return View("CompanyAddress", serviceSummaryViewModel);
            }
        }
        #endregion

        #region Roles

        [HttpGet("provider-roles")]
        public async Task<IActionResult> ProviderRoles(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            RoleViewModel roleViewModel = new()
            {
                SelectedRoleIds = summaryViewModel?.RoleViewModel?.SelectedRoles?.Select(c => c.Id).ToList(),
                AvailableRoles = await cabService.GetRoles(summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : 
                summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? "/cab-service/submit-service/tf-version?providerProfileId=" + summaryViewModel.ProviderProfileId :
                "/cab-service/submit-service/company-address"
            };
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
            bool fromDetailsPage = roleViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<RoleDto> availableRoles = await cabService.GetRoles(summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version);          
            roleViewModel.AvailableRoles = availableRoles;
            roleViewModel.SelectedRoleIds =  roleViewModel.SelectedRoleIds??[];
            roleViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (roleViewModel.SelectedRoleIds.Count > 0)
                summaryViewModel.RoleViewModel.SelectedRoles = availableRoles.Where(c => roleViewModel.SelectedRoleIds.Contains(c.Id)).ToList();
          
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                if (summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4)
                {
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "SelectServiceType", "TrustFramework0_4");
                }
                else
                {
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "GPG44Input");                
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
        public IActionResult GPG44Input(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers" :
                fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey :
                "/cab-service/submit-service/provider-roles";
            return View(summaryViewModel);
        }

        [HttpPost("gpg44-input")]
        public async Task<IActionResult> SaveGPG44Input(ServiceSummaryViewModel viewModel, string action)
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
                return await HandleGpg44Actions(action, summaryViewModel, fromSummaryPage, fromDetailsPage);
            }
            else
            {
                return View("GPG44Input", viewModel);
            }
        }

      #endregion

        #region select GPG44
        [HttpGet("gpg44")]
        public async Task<IActionResult> GPG44(bool fromSummaryPage, bool fromDetailsPage)
        {            
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            QualityLevelViewModel qualityLevelViewModel = new()
            {
                IsAmendment = summaryViewModel.IsAmendment,                
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/gpg44-input"
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
        [HttpPost("gpg44")]
        public async Task<IActionResult> SaveGPG44(QualityLevelViewModel qualityLevelViewModel, string action)
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

            qualityLevelViewModel.SelectedLevelOfProtectionIds =  qualityLevelViewModel.SelectedLevelOfProtectionIds??[];
            if (qualityLevelViewModel.SelectedLevelOfProtectionIds.Count > 0)
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = availableQualityLevels.Where(c => qualityLevelViewModel.SelectedLevelOfProtectionIds.Contains(c.Id)).ToList();


            summaryViewModel.QualityLevelViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "GPG45Input"); 
            }
            else
            {
                return View("GPG44", qualityLevelViewModel);
            }
        }
        #endregion

        #region GPG45 input

        [HttpGet("gpg45-input")]
        public IActionResult GPG45Input(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers" :
                fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey :
                summaryViewModel.HasGPG44.GetValueOrDefault() ? "/cab-service/submit-service/gpg44" :
                "/cab-service/submit-service/gpg44-input";
            return View(summaryViewModel);
        }

        [HttpPost("gpg45-input")]
        public async Task<IActionResult> SaveGPG45Input(ServiceSummaryViewModel viewModel, string action)
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
                return View("GPG45Input", viewModel);
            }
        }
        #endregion

        #region select GPG45

        [HttpGet("gpg45")]
        public async Task<IActionResult> GPG45(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            IdentityProfileViewModel identityProfileViewModel = new()
            {                
                IsAmendment = summaryViewModel.IsAmendment,
                SelectedIdentityProfileIds = summaryViewModel?.IdentityProfileViewModel?.SelectedIdentityProfiles?.Select(c => c.Id).ToList(),
                AvailableIdentityProfiles = await cabService.GetIdentityProfiles(),
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/gpg45-input"
            };
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
            bool fromDetailsPage = identityProfileViewModel.FromDetailsPage;     
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            identityProfileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            List<IdentityProfileDto> availableIdentityProfiles = await cabService.GetIdentityProfiles();
            identityProfileViewModel.AvailableIdentityProfiles = availableIdentityProfiles;
            identityProfileViewModel.SelectedIdentityProfileIds =  identityProfileViewModel.SelectedIdentityProfileIds??new List<int>();
            if (identityProfileViewModel.SelectedIdentityProfileIds.Count > 0)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = availableIdentityProfiles.Where(c => identityProfileViewModel.SelectedIdentityProfileIds.Contains(c.Id)).ToList();
            summaryViewModel.IdentityProfileViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "HasSupplementarySchemesInput");               
            }
            else
            {
                return View("GPG45", identityProfileViewModel);
            }
        }
        #endregion


        #region Supplemetary schemes

        [HttpGet("supplementary-schemes-input")]
        public IActionResult HasSupplementarySchemesInput(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.RefererURL = fromSummaryPage ? "/cab-service/submit-service/check-your-answers" 
                : fromDetailsPage ? "/cab-service/service-details?serviceKey=" + summaryViewModel?.ServiceKey
                : summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_3 ? summaryViewModel.HasGPG45 == true? "/cab-service/submit-service/gpg45" : "/cab-service/submit-service/gpg45-input"
                : summaryViewModel.HasGPG44 == true ?  "/cab-service/submit-service/service/gpg44" 
                : "/cab-service/submit-service/service/gpg44-input";
            return View(summaryViewModel);
        }

        [HttpPost("supplementary-schemes-input")]
        public async Task<IActionResult> SaveHasSupplementarySchemesInput(ServiceSummaryViewModel viewModel, string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            bool fromSummaryPage = viewModel.FromSummaryPage;
            bool fromDetailsPage = viewModel.FromDetailsPage;
            viewModel.IsAmendment = summaryViewModel.IsAmendment;           
            if (ModelState["HasSupplementarySchemes"].Errors.Count == 0)
            {
                summaryViewModel.IsSchemeEditedFromSummary = viewModel.FromSummaryPage;
                summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;
                summaryViewModel.RefererURL = viewModel.RefererURL;
                return await HandleSchemeActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage);     
            }
            else
            {
                return View("HasSupplementarySchemesInput", viewModel);
            }
        }

        [HttpGet("supplementary-schemes")]
        public async Task<IActionResult> SupplementarySchemes(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            SupplementarySchemeViewModel supplementarySchemeViewModel = new()
            {
                SelectedSupplementarySchemeIds = summaryViewModel?.SupplementarySchemeViewModel?.SelectedSupplementarySchemes?.Select(c => c.Id).ToList(),
                AvailableSchemes = await cabService.GetSupplementarySchemes(),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL()
                : "/cab-service/submit-service/supplementary-schemes-input"
            }; 
            return View(supplementarySchemeViewModel);
        }

        [HttpPost("supplementary-schemes")]
        public async Task<IActionResult> SaveSupplementarySchemes(SupplementarySchemeViewModel supplementarySchemeViewModel, string action)
        {
            bool fromSummaryPage = supplementarySchemeViewModel.FromSummaryPage;
            bool fromDetailsPage = supplementarySchemeViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<SupplementarySchemeDto> availableSupplementarySchemes = await cabService.GetSupplementarySchemes();
            supplementarySchemeViewModel.IsAmendment = summaryViewModel.IsAmendment;
            supplementarySchemeViewModel.AvailableSchemes = availableSupplementarySchemes;
            supplementarySchemeViewModel.SelectedSupplementarySchemeIds =  supplementarySchemeViewModel.SelectedSupplementarySchemeIds??new List<int>();
            if (supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Count > 0)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = availableSupplementarySchemes.Where(c => supplementarySchemeViewModel.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
            summaryViewModel.SupplementarySchemeViewModel.FromSummaryPage = false;

            if (ModelState.IsValid)
            {
                string nextPage;
                string controller;
                object routeValues = null;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                if (summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4)
                {                   
                    HttpContext?.Session.Set("SelectedSchemeIds", supplementarySchemeViewModel.SelectedSupplementarySchemeIds);
                    int firstSchemeId = supplementarySchemeViewModel.SelectedSupplementarySchemeIds[0];
                    nextPage = "SchemeGPG45";
                    controller = "TrustFramework0_4";
                    routeValues= new { schemeId = firstSchemeId };
                    return await HandleSchemeSelectionActions(action, summaryViewModel, false, fromDetailsPage, nextPage, controller, routeValues!);
                }
                else
                {
                    nextPage = "CertificateUploadPage";
                    controller = "CabService";
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, nextPage, controller, routeValues!);

                }
                
            }
            else
            {
                return View("SupplementarySchemes", supplementarySchemeViewModel);
            }
           
        }
        #endregion

        #region File upload/download

        [HttpGet("certificate-upload")]
        public async Task<IActionResult> CertificateUploadPage(bool fromSummaryPage, bool remove, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;            
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            ViewBag.IsTFVersionChanged = summaryViewModel.IsTFVersionChanged;
            var lastScheme = summaryViewModel?.SchemeQualityLevelMapping?.LastOrDefault() ?? null;
            CertificateFileViewModel certificateFileViewModel = new()
            {
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL()
                : lastScheme == null ? "/cab-service/submit-service/supplementary-schemes-input"
                : lastScheme.HasGPG44.GetValueOrDefault() ? "/cab-service/submit-service/scheme/gpg44?schemeId=" + lastScheme.SchemeId
                : "/cab-service/submit-service/scheme/gpg44-input?schemeId=" + lastScheme.SchemeId,
                IsAmendment = summaryViewModel.IsAmendment
            };

            if (remove)
            {
                summaryViewModel.FileLink = null;
                summaryViewModel.FileName = null;               
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                certificateFileViewModel.FileRemoved = true;
                certificateFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
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
                certificateFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
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
            bool fromDetailsPage = certificateFileViewModel.FromDetailsPage;
            certificateFileViewModel.FromSummaryPage = false;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            certificateFileViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (Convert.ToBoolean(certificateFileViewModel.FileUploadedSuccessfully) == false)
            {
                if (ModelState["File"].Errors.Count == 0)
                {
                    using var memoryStream = new MemoryStream();
                    await certificateFileViewModel.File.CopyToAsync(memoryStream);
                    GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, certificateFileViewModel.File.FileName);
                    if (genericResponse.Success)
                    {
                        summaryViewModel.FileName = certificateFileViewModel.File.FileName;
                        summaryViewModel.FileSizeInKb = Math.Round((decimal)certificateFileViewModel.File.Length / 1024, 1);
                        summaryViewModel.FileLink = genericResponse.Data;
                        certificateFileViewModel.FileUploadedSuccessfully = true;
                        certificateFileViewModel.FileName = certificateFileViewModel.File.FileName;
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);

                       
                        if (action == "continue" || action == "amend")
                        {
                            return View("CertificateUploadPage", certificateFileViewModel);
                        }                       
                        else if (action == "draft")
                        {
                            return await SaveAsDraftAndRedirect(summaryViewModel);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid action parameter");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Unable to upload the file provided");
                        return View("CertificateUploadPage", certificateFileViewModel);
                    }

                }
               
                else
                {
                    return View("CertificateUploadPage", certificateFileViewModel);
                }
            }

            else 
            {
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "ConfirmityIssueDate");
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
        public IActionResult ConfirmityIssueDate(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateViewModel dateViewModel = new()
            {
                PropertyName = "ConfirmityIssueDate"
               
            };
            if (summaryViewModel.ConformityIssueDate != null)
            {
                dateViewModel = ViewModelHelper.GetDayMonthYear(summaryViewModel.ConformityIssueDate);
            }
            dateViewModel.RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/certificate-upload";
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
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
            bool fromDetailsPage = dateViewModel.FromDetailsPage;
            dateViewModel.FromSummaryPage = false;
            dateViewModel.FromDetailsPage = false;
            dateViewModel.PropertyName = "ConfirmityIssueDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityIssueDate = ValidationHelper.ValidateIssueDate(dateViewModel, summaryViewModel.ConformityExpiryDate, fromSummaryPage,ModelState,
            summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4);
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate = conformityIssueDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "ConfirmityExpiryDate");               
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);
            }
        }



        [HttpGet("enter-expiry-date")]
        public IActionResult ConfirmityExpiryDate(bool fromDetailsPage)
        {
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            DateViewModel dateViewModel = new()
            {
                PropertyName = "ConfirmityExpiryDate"
            };

            if (summaryViewModel.ConformityExpiryDate != null)
            {
                dateViewModel = ViewModelHelper.GetDayMonthYear(summaryViewModel.ConformityExpiryDate);
            }
            if (summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4)
            {
                dateViewModel.IsTfVersion0_4 = true;
            }
            dateViewModel.RefererURL =  fromDetailsPage ? GetRefererURL() : "/cab-service/submit-service/enter-issue-date";
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
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
            bool fromDetailsPage = dateViewModel.FromDetailsPage;
            dateViewModel.FromDetailsPage = false;
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityExpiryDate;
            if (dateViewModel.IsTfVersion0_4)
            {
                conformityExpiryDate = ValidationHelper.ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate), ModelState, dateViewModel. IsTfVersion0_4, years: 3, days: 59);                
            }
               
            else
            {
                conformityExpiryDate = ValidationHelper.ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate), ModelState);
            }
              
            dateViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityExpiryDate = conformityExpiryDate;
                summaryViewModel.IsTFVersionChanged = false;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, true, fromDetailsPage, "ServiceSummary");               
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
            SetRefererURL();
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            return View(summaryViewModel);
        }

        [HttpPost("check-your-answers")]
        public async Task<IActionResult> SaveServiceSummary()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.ServiceStatus = ServiceStatusEnum.Submitted;

            if (!HasAllRequiredData(summaryViewModel))
            {
                throw new InvalidOperationException("SummarySaveFail: All fields must be completed before submitting.");
            }

            ServiceDto serviceDto = _mapper.Map<ServiceDto>(summaryViewModel);
            
            ViewModelHelper.MapTFVersion0_4Fields(summaryViewModel, serviceDto);

            if (!IsValidCabId(summaryViewModel.CabId))
                return HandleInvalidCabId(summaryViewModel.CabId);

            GenericResponse genericResponse = new();
            if (summaryViewModel.IsResubmission)
            {
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail, summaryViewModel.IsReupload.GetValueOrDefault());
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            }
            if (genericResponse.Success)
            {
                ProviderProfileDto provider = await cabService.GetProvider(summaryViewModel.ProviderProfileId, summaryViewModel.CabId);
                string providerName = provider?.RegisteredName;
                int providerId = provider.Id;
                return RedirectToAction("InformationSubmitted", new { providerName, serviceName = summaryViewModel.ServiceName, providerId});
            }
            else
            {
                throw new InvalidOperationException("Service submission failed");
            }
        }        

        /// <summary>
        ///Final page if save success
        /// </summary>       
        /// <returns></returns>
        [HttpGet("service-submitted")]
        public async Task <IActionResult> InformationSubmitted(string providerName, string serviceName, int? providerId)
        {
            ViewBag.ServiceName = serviceName;
            ViewBag.ProviderName = providerName;
            ViewBag.ProviderId = providerId;
            HttpContext?.Session.Remove("ServiceSummary");
            ViewBag.Email = UserEmail;
            await emailSender.SendEmailCabInformationSubmitted(UserEmail, UserEmail, providerName, serviceName);
            await emailSender.SendCertificateInfoSubmittedToDSIT();
            return View();

        }

        #endregion       
       

        #region Private Methods
        private async Task<IActionResult> SaveAsDraftAndRedirect(ServiceSummaryViewModel serviceSummary)
        {
            GenericResponse genericResponse = new();
            serviceSummary.ServiceStatus = ServiceStatusEnum.SavedAsDraft;
            ServiceDto serviceDto = _mapper.Map<ServiceDto>(serviceSummary);

            ViewModelHelper.MapTFVersion0_4Fields(serviceSummary, serviceDto);

            if (!IsValidCabId(serviceSummary.CabId))
                return HandleInvalidCabId(serviceSummary.CabId);

            if (serviceDto.CabUserId < 0) throw new InvalidDataException("Invalid CabUserId");

            if (serviceSummary.IsResubmission)
            {
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail, serviceSummary.IsReupload.GetValueOrDefault());
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

        private async Task HandleInvalidProfileAndCab(int providerProfileId, CabUserDto cabUserDto)
        {
            // to prevent another cab changing the providerProfileId from url
            bool isValid = await cabService.CheckValidCabAndProviderProfile(providerProfileId, cabUserDto.CabId);
            if (!isValid)
                throw new InvalidOperationException("Invalid provider profile ID for Cab ID");
        }

        #region Handle save/draft/amend actions

        private async Task<IActionResult> HandleActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, 
            string nextPage,  string controller = "CabService", object routeValues = null!)
        {
            switch (action)
            {
                case "continue":
                    return fromSummaryPage ? RedirectToAction("ServiceSummary")
                        : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                        : routeValues== null? RedirectToAction(nextPage,controller): RedirectToAction(nextPage, controller, routeValues);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return serviceSummary.IsTFVersionChanged.GetValueOrDefault() ? routeValues == null ? RedirectToAction(nextPage, controller) : RedirectToAction(nextPage, controller, routeValues) :
                    RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");

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
                        return RedirectToAction("GPG44", new { fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : RedirectToAction("GPG45Input");
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
                        return RedirectToAction("GPG44");
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg44(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("GPG45Input", new {fromSummaryPage, fromDetailsPage }) : RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandleGpg45Actions(string action, ServiceSummaryViewModel summaryViewModel, bool fromSummaryPage, bool fromDetailsPage)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(summaryViewModel.HasGPG45))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("GPG45", new { fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : RedirectToAction("HasSupplementarySchemesInput");
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
                        return RedirectToAction("GPG45");
                    }
                    else
                    {
                        ViewModelHelper.ClearGpg45(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("HasSupplementarySchemesInput") : RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> HandleSchemeActions(string action, ServiceSummaryViewModel summaryViewModel, bool fromSummaryPage, bool fromDetailsPage)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SupplementarySchemes", new { fromSummaryPage, fromDetailsPage });
                    }
                    else
                    {
                        ViewModelHelper.ClearSchemes(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return fromSummaryPage ? RedirectToAction("ServiceSummary") : fromDetailsPage ? await SaveAsDraftAndRedirect(summaryViewModel)
                       : RedirectToAction("CertificateUploadPage");
                    }

                case "draft":
                    if (!Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                    {
                        ViewModelHelper.ClearSchemes(summaryViewModel);
                    }
                    HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                    return await SaveAsDraftAndRedirect(summaryViewModel);

                case "amend":

                    if (Convert.ToBoolean(summaryViewModel.HasSupplementarySchemes))
                    {
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return RedirectToAction("SupplementarySchemes");
                    }
                    else
                    {
                        ViewModelHelper.ClearSchemes(summaryViewModel);
                        HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                        return summaryViewModel.IsTFVersionChanged.GetValueOrDefault() ? RedirectToAction("CertificateUploadPage") : RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");
                    }

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandleSchemeSelectionActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage,
            string nextPage, string controller = "CabService", object routeValues = null!)
        {
            switch (action)
            {
                case "continue":
                    return fromSummaryPage ? RedirectToAction("ServiceSummary")
                        : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                        : routeValues == null ? RedirectToAction(nextPage, controller) : RedirectToAction(nextPage, controller, routeValues);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return RedirectToAction(nextPage, controller, routeValues);

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private bool HasAllRequiredData(ServiceSummaryViewModel vm)
        {
            if (vm == null)
                return false;

            bool hasBasicInfo =
                !string.IsNullOrWhiteSpace(vm.ServiceName) &&
                vm.TFVersionViewModel?.SelectedTFVersion != null &&
                !string.IsNullOrWhiteSpace(vm.ServiceURL) &&
                !string.IsNullOrWhiteSpace(vm.CompanyAddress) &&
                vm.RoleViewModel?.SelectedRoles != null &&
                vm.ConformityIssueDate != null &&
                vm.ConformityExpiryDate != null;

            bool hasGpg44Data =
                vm.HasGPG44.HasValue &&
                (!vm.HasGPG44.GetValueOrDefault() ||
                 (vm.QualityLevelViewModel?.SelectedLevelOfProtections != null &&
                  vm.QualityLevelViewModel?.SelectedQualityofAuthenticators != null));

            bool hasGpg45Data =
                vm.HasGPG45.HasValue &&
                (!vm.HasGPG45.GetValueOrDefault() ||
                 vm.IdentityProfileViewModel?.SelectedIdentityProfiles != null);

            bool hasSupplementaryData =
                vm.HasSupplementarySchemes.HasValue &&
                (!vm.HasSupplementarySchemes.GetValueOrDefault() ||
                 vm.SupplementarySchemeViewModel?.SelectedSupplementarySchemes != null);

            bool hasTfVersion04Data = true;
            if (vm.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion0_4)
            {
                hasTfVersion04Data =
                    vm.ServiceType.HasValue &&
                    (vm.ServiceType != ServiceTypeEnum.WhiteLabelled ||
                        // White-labelled checks
                        (vm.IsUnderpinningServicePublished == true && vm.SelectedUnderPinningServiceId != null) ||
                        (vm.IsUnderpinningServicePublished == false && vm.SelectedManualUnderPinningServiceId != null) ||
                        (vm.IsUnderpinningServicePublished == false &&
                         !string.IsNullOrWhiteSpace(vm.UnderPinningProviderName) &&
                         !string.IsNullOrWhiteSpace(vm.UnderPinningServiceName) &&
                         vm.UnderPinningServiceExpiryDate != null &&
                         !string.IsNullOrWhiteSpace(vm.SelectCabViewModel?.SelectedCabName)))
                    &&
                    // Supplementary scheme checks only for TF 0.4
                    (vm.HasSupplementarySchemes.HasValue &&
                     (!vm.HasSupplementarySchemes.GetValueOrDefault() ||
                      ((vm.SchemeIdentityProfileMapping.Count() == vm.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Count() &&
                        vm.SchemeIdentityProfileMapping.All(sc =>
                            sc.IdentityProfile?.SelectedIdentityProfiles != null)) &&
                       (vm.SchemeQualityLevelMapping.Count == vm.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Count() &&
                        vm.SchemeQualityLevelMapping.All(sc => sc.HasGPG44.HasValue &&
                            (!sc.HasGPG44.GetValueOrDefault() ||
                            (sc.QualityLevel?.SelectedLevelOfProtections != null &&
                             sc.QualityLevel?.SelectedQualityofAuthenticators != null)))))));
            }

            return hasBasicInfo
                && hasGpg44Data
                && hasGpg45Data
                && hasSupplementaryData
                && hasTfVersion04Data;
        }


        #endregion





        #endregion
    }
}