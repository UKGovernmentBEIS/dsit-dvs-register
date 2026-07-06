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
using Microsoft.Extensions.Options;


namespace DVSRegister.Controllers
{
    [Route("cab-service/submit-service")]
  
    public class CabServiceController(ICabService cabService, IBucketService bucketService, IUserService userService, IActionLogService actionLogService,
    CabEmailSender emailSender, IOptions<S3Configuration> config, ILogger<CabServiceController> logger, IMapper mapper) : BaseController(logger)
    {

        private readonly ICabService cabService = cabService;
        private readonly IBucketService bucketService = bucketService;
        private readonly IUserService userService = userService;
        private readonly IActionLogService actionLogService = actionLogService;
        private readonly CabEmailSender emailSender = emailSender;
        private readonly ILogger<CabServiceController> _logger = logger;
        private readonly S3Configuration config = config.Value;
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
        [HttpGet("name-of-service")]
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
        [HttpGet("service-url")]
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

                if (serviceSummary.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion1_0)
                {
                    return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, "TermsOfUseUpload", "TrustFramework0_4");
                }
                else
                {
                    return await HandleActions(action, serviceSummary, fromSummaryPage, fromDetailsPage, "ProviderRoles");
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
        public async Task<IActionResult> ProviderRoles(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();            
            RoleViewModel roleViewModel = new()
            {
                SelectedRoleIds = summaryViewModel?.RoleViewModel?.SelectedRoles?.Select(c => c.Id).ToList(),
                AvailableRoles = await cabService.GetRoles(summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version),
                TrustFrameworkVersion = summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version,
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = fromSummaryPage || fromDetailsPage ? GetRefererURL() :
                summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion1_0 ? "/cab-service/submit-service/terms-of-use-upload" :
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
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "SelectServiceType", "TrustFramework0_4");
                else
                    return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "VouchingGuidance", "TrustFramework0_4");                
            }
            else
                return View("ProviderRoles", roleViewModel);
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
                : summaryViewModel.HasGPG44 == true ? "/cab-service/submit-service/service/gpg44"
                : "/cab-service/submit-service/service/gpg44-input";
            return View(summaryViewModel);
        }

        [HttpPost("supplementary-schemes-input")]
        public async Task<IActionResult> SaveHasSupplementarySchemesInput(ServiceSummaryViewModel viewModel, string action)
        {
            var summaryViewModel = GetServiceSummary();
            viewModel.IsAmendment = summaryViewModel.IsAmendment;

            bool allLowGPG45 = summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles.All(ip => ip.IdentityProfileType == IdentityProfileTypeEnum.Low);
            bool isTFVersion1_0 = summaryViewModel.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion1_0;

            if (ModelState["HasSupplementarySchemes"].Errors.Count > 0)
                return View("HasSupplementarySchemesInput", viewModel);

            if (viewModel.HasSupplementarySchemes == true && isTFVersion1_0 && allLowGPG45)
            {
                ModelState.AddModelError("HasSupplementarySchemes", "The service cannot be certified against any supplementary codes. Select ‘No’ to proceed or change your GPG45 identity profiles selection");
                return View("HasSupplementarySchemesInput", viewModel);
            }
            summaryViewModel.IsSchemeEditedFromSummary = viewModel.FromSummaryPage;
            summaryViewModel.HasSupplementarySchemes = viewModel.HasSupplementarySchemes;
            summaryViewModel.RefererURL = viewModel.RefererURL;

            return await HandleSchemeActions(action, summaryViewModel, viewModel.FromSummaryPage, viewModel.FromDetailsPage);
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
        public async Task<IActionResult> SaveSupplementarySchemes(SupplementarySchemeViewModel model, string action)
        {
            if (!ModelState.IsValid)
            {
                return View("SupplementarySchemes", model);
            }

            var summary = GetServiceSummary();
            var availableSchemes = await cabService.GetSupplementarySchemes();

            model.SelectedSupplementarySchemeIds ??= [];

            summary.SupplementarySchemeViewModel.SelectedSupplementarySchemes = availableSchemes.Where(s => model.SelectedSupplementarySchemeIds.Contains(s.Id)).ToList();

            summary.SupplementarySchemeViewModel.FromSummaryPage = false;

            HttpContext?.Session.Set("ServiceSummary", summary);
            HttpContext?.Session.Set("SelectedSchemeIds", model.SelectedSupplementarySchemeIds);

            var firstSchemeId = model.SelectedSupplementarySchemeIds[0];

            return await HandleSchemeSelectionActions(action, summary, false, model.FromDetailsPage, "SchemeGPG45", "TrustFramework0_4", new { schemeId = firstSchemeId });
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
                var fileContent = await bucketService.DownloadFileAsync(summaryViewModel.FileLink, config.BucketName);
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
                    if (!ValidationHelper.ValidatePdfSignature(memoryStream))
                    {
                        ModelState.AddModelError("File", "The uploaded file does not appear to be a valid PDF.");
                        return View("CertificateUploadPage", certificateFileViewModel);
                    }
                    GenericResponse genericResponse = await bucketService.WriteToS3Bucket(memoryStream, certificateFileViewModel.File.FileName, config.BucketName);

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
            DateTime? conformityIssueDate = ValidationHelper.ValidateIssueDate(dateViewModel, summaryViewModel.ConformityExpiryDate, fromSummaryPage,ModelState);
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
            conformityExpiryDate = ValidationHelper.ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate), ModelState);

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
            InProgressApplicationParameters parameters = null!;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            if (!summaryViewModel.IsReupload.GetValueOrDefault())
            {
                var serviceList = await cabService.GetServiceList(summaryViewModel.ServiceKey, CabId);
                InProgressApplicationParameters inProgressApplicationParameters = ViewModelHelper.GetInProgressApplicationParameters(serviceList);
                if (inProgressApplicationParameters != null && (inProgressApplicationParameters.HasInProgressApplication || inProgressApplicationParameters.HasActiveReassignmentRequest
                || inProgressApplicationParameters.HasActiveRemovalRequest || inProgressApplicationParameters.InProgressAndUpdateRequested))
                {
                    parameters = inProgressApplicationParameters;
                }
            }
            return await SaveServiceSummary(summaryViewModel, parameters);

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
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail, serviceSummary.IsReupload.GetValueOrDefault(), null);                              
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);               
            }

            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ServiceSummary");
                return RedirectToAction("ServiceDraftDetails", "CabServiceReApplication", new { serviceId = genericResponse.InstanceId});
            }
            else
            {
                throw new InvalidOperationException("SaveAsDraftAndRedirect: Failed to save draft");
            }


        }
        private async Task<IActionResult> SaveServiceSummary(ServiceSummaryViewModel summaryViewModel, InProgressApplicationParameters? inProgressApplicationParameters)
        {
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
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail, summaryViewModel.IsReupload.GetValueOrDefault(), inProgressApplicationParameters);
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            }
            if (genericResponse.Success)
            { 
                ServiceDto submittedService = await cabService.GetServiceDetailsWithProvider(genericResponse.InstanceId, CabId);
                string providerName = submittedService.Provider?.RegisteredName ?? string.Empty;
                int providerId = submittedService.Provider!.Id;

                await actionLogService.AddActionLog(submittedService, ActionCategoryEnum.CR, ActionDetailsEnum.CR_Submitted,UserEmail);
                return RedirectToAction("InformationSubmitted", new { providerName, serviceName = summaryViewModel.ServiceName, providerId });
            }
            else
            {
                throw new InvalidOperationException("Service submission failed");
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
            if (vm.TFVersionViewModel.SelectedTFVersion.Version >= Constants.TFVersion0_4)
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

            bool hasTfVersion1_0Data = true;
            if (vm.TFVersionViewModel.SelectedTFVersion.Version == Constants.TFVersion1_0)
            {
                hasTfVersion1_0Data = vm.TOUFileName != null && vm.HasVouchingGuidance.HasValue;
            }

            return hasBasicInfo
                && hasGpg44Data
                && hasGpg45Data
                && hasSupplementaryData
                && hasTfVersion04Data
                && hasTfVersion1_0Data;
        }


        #endregion
        #endregion
    }
}