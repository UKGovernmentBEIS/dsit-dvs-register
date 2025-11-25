using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Provider;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/provider")] 
    public class CabProviderController(ICabService cabService,  IUserService userService, IActionLogService actionLogService, IMapper mapper, ILogger<CabProviderController> logger) : BaseController(logger)
    {
        private readonly ICabService cabService = cabService;
        
        private readonly IUserService userService = userService;
        private readonly IActionLogService actionLogService = actionLogService;
        private readonly IMapper mapper = mapper;
      

        [HttpGet("before-you-start")]
        public IActionResult BeforeYouStart()
        {           
            HttpContext?.Session.Remove("ProfileSummary"); // clear data in session before adding a new profile
            return View();
        }


        #region Registered Name

        [HttpGet("reg-name/{sourcePage?}")]
        public IActionResult RegisteredName(SourcePageEnum? sourcePage)
        {          
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
                || sourcePage == SourcePageEnum.ProfileEditSummary 
                || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() :Constants.CabProviderBaseURL+  "/before-you-start";
            return View("RegisteredName", profileSummaryViewModel);
        }

        [HttpPost("reg-name")]
        public async Task<IActionResult> SaveRegisteredName(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
            profileSummaryViewModel.FromSummaryPage = false;          
            if (!string.IsNullOrEmpty(profileSummaryViewModel.RegisteredName))
            {
                bool registeredNameExist = profileSummaryViewModel.ProviderId > 0?
                    await cabService.CheckProviderRegisteredNameExists(profileSummaryViewModel.RegisteredName, profileSummaryViewModel.ProviderId) :
                    await cabService.CheckProviderRegisteredNameExists(profileSummaryViewModel.RegisteredName);
                if (registeredNameExist)
                {
                    ModelState.AddModelError("RegisteredName", Constants.RegisteredNameExistsError);
                }
            }

            if (ModelState["RegisteredName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.RegisteredName = profileSummaryViewModel.RegisteredName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return  await HandleActions(action, profileSummary, sourcePage, "TradingName");              

            }
            else
            {
                return View("RegisteredName", profileSummaryViewModel);
            }
        }

        #endregion


        #region Trading Name

        [HttpGet("trading-name")]
        public IActionResult TradingName(SourcePageEnum sourcePage)
        {
            
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
             || sourcePage == SourcePageEnum.ProfileEditSummary
             || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() :  Constants.CabProviderBaseURL+ "/reg-name";
            return View("TradingName", profileSummaryViewModel);
        }

        [HttpPost("trading-name")]
        public async Task<IActionResult> SaveTradingName(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
  
            if (ModelState["TradingName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.TradingName = profileSummaryViewModel.TradingName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "HasRegistrationNumber");
               
            }
            else
            {
                return View("TradingName", profileSummaryViewModel);
            }
        }

        #endregion


        #region HasCompanyRegistrationNumber

        [HttpGet("company-number")]
        public IActionResult HasRegistrationNumber(SourcePageEnum sourcePage)
        {           
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
             || sourcePage == SourcePageEnum.ProfileEditSummary
             || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() : Constants.CabProviderBaseURL + "/trading-name";
            return View(summaryViewModel);
        }


        [HttpPost("company-number")]
        public async Task<IActionResult> SaveHasRegistrationNumber(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
           
            if (ModelState["HasRegistrationNumber"].Errors.Count == 0)
            {
                summaryViewModel.HasRegistrationNumber = profileSummaryViewModel.HasRegistrationNumber;
                return await HandleHasCompanyRegistration(action, summaryViewModel, sourcePage);
            }
            else
            {
                return View("HasRegistrationNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region Registration number

        [HttpGet("company-number-input")]
        public IActionResult CompanyRegistrationNumber(SourcePageEnum sourcePage)
        {           
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileEditSummary
            || sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() : Constants.CabProviderBaseURL + "/company-number";
            return View(summaryViewModel);
        }

        [HttpPost("company-number-input")]
        public async Task<IActionResult> SaveCompanyRegistrationNumber(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;      
            if (ModelState["CompanyRegistrationNumber"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.CompanyRegistrationNumber = profileSummaryViewModel.CompanyRegistrationNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "HasParentCompany");
               
            }
            else
            {
                return View("CompanyRegistrationNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region DUNSNumber

        [HttpGet("duns-number")]
        public IActionResult DUNSNumber(SourcePageEnum sourcePage)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
             || sourcePage == SourcePageEnum.ProfileEditSummary
             || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() : Constants.CabProviderBaseURL + "/company-number";
            return View(summaryViewModel);
        }

        [HttpPost("duns-number")]
        public async Task<IActionResult> SaveDUNSNumber(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;        
            if (ModelState["DUNSNumber"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.DUNSNumber = profileSummaryViewModel.DUNSNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "HasParentCompany");
               
            }
            else
            {
                return View("DUNSNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region HasParentCompany

        [HttpGet("parent-company")]
        public IActionResult HasParentCompany(SourcePageEnum sourcePage)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary
            || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL() 
            : summaryViewModel.HasRegistrationNumber == true? Constants.CabProviderBaseURL + "/company-number-input" : Constants.CabProviderBaseURL + "/duns-number";
            return View(summaryViewModel);
        }

        [HttpPost("parent-company")]
        public async Task<IActionResult> SaveHasParentCompany(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();          
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
            if (ModelState["HasParentCompany"].Errors.Count == 0)
            {
                summaryViewModel.HasParentCompany = profileSummaryViewModel.HasParentCompany;
                return await HandleHasParentCompany(action, summaryViewModel, sourcePage, "PrimaryContact");
            }
            else
            {
                return View("HasParentCompany", profileSummaryViewModel);
            }
        }

        #endregion

        #region Parent company registered name

        [HttpGet("parent-company-registered-name-input")]
        public IActionResult ParentCompanyRegisteredName(SourcePageEnum sourcePage)
        {           
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary|| sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
            :"/parent-company";
            return View(summaryViewModel);
        }


        [HttpPost("parent-company-registered-name-input")]
        public async Task<IActionResult> SaveParentCompanyRegisteredName(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
            if (ModelState["ParentCompanyRegisteredName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ParentCompanyRegisteredName = profileSummaryViewModel.ParentCompanyRegisteredName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "ParentCompanyLocation");
               
            }
            else
            {
                return View("ParentCompanyRegisteredName", profileSummaryViewModel);
            }
        }

        #endregion

        #region Parent company location

        [HttpGet("parent-company-location-input")]
        public IActionResult ParentCompanyLocation(SourcePageEnum sourcePage)
        {          
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            summaryViewModel.SourcePage = sourcePage;
            summaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
            : Constants.CabProviderBaseURL + "/parent-company-registered-name-input";
            return View(summaryViewModel);
        }

        [HttpPost("parent-company-location-input")]
        public async Task<IActionResult> SaveParentCompanyLocation(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage ;
            if (ModelState["ParentCompanyLocation"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ParentCompanyLocation = profileSummaryViewModel.ParentCompanyLocation;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "PrimaryContact");               
            }
            else
            {
                return View("ParentCompanyLocation", profileSummaryViewModel);
            }
        }

        #endregion

        #region Primary Contact

        [HttpGet("primary-contact-information")]
        public IActionResult PrimaryContact(SourcePageEnum? sourcePage)
        {         
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.PrimaryContact.SourcePage = sourcePage;
            profileSummaryViewModel.PrimaryContact.IsInCompleteApplication = profileSummaryViewModel.IsInCompleteApplication;
            ViewBag.hasParentCompany = profileSummaryViewModel.HasParentCompany;
            profileSummaryViewModel.PrimaryContact.RefererURL = sourcePage == SourcePageEnum.ProfileSummary? GetRefererURL()
            :profileSummaryViewModel.HasParentCompany == true? Constants.CabProviderBaseURL + "/parent-company-location-input" : Constants.CabProviderBaseURL + "/parent-company";
            return View(profileSummaryViewModel.PrimaryContact);
        }

        [HttpPost("primary-contact-information")]
        public async Task<IActionResult> SavePrimaryContact(PrimaryContactViewModel primaryContactViewModel, string action)
        {
          SourcePageEnum? sourcePage = primaryContactViewModel.SourcePage ;
            ProfileSummaryViewModel profileSummary = GetProfileSummary();
            primaryContactViewModel.IsInCompleteApplication = profileSummary.IsInCompleteApplication;
            ValidationHelper.ValidateDuplicateFields(ModelState,primaryContactViewModel.PrimaryContactEmail,profileSummary.SecondaryContact?.SecondaryContactEmail,
             new ValidationHelper.FieldComparisonConfig(
            "PrimaryContactEmail",
            "SecondaryContactEmail",
            "Email address of secondary contact cannot be the same as primary contact"));

            ValidationHelper.ValidateDuplicateFields(
                ModelState,primaryValue: primaryContactViewModel.PrimaryContactTelephoneNumber,secondaryValue: profileSummary.SecondaryContact?.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                "PrimaryContactTelephoneNumber",
                "SecondaryContactTelephoneNumber",
                "Telephone number of secondary contact cannot be the same as primary contact" ));

            if (ModelState.IsValid)
            {
                profileSummary.PrimaryContact = primaryContactViewModel;            
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandlePrimaryAndSecondaryContactActions(action, profileSummary, sourcePage, "SecondaryContact");
              
            }
            else
            {
                return View("PrimaryContact", primaryContactViewModel);
            }
        }

        #endregion

        #region Secondary Contact

        [HttpGet("secondary-contact-information")]
        public IActionResult SecondaryContact(SourcePageEnum? sourcePage)
        {
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SecondaryContact.SourcePage = sourcePage;
            profileSummaryViewModel.SecondaryContact.IsInCompleteApplication = profileSummaryViewModel.IsInCompleteApplication;
            profileSummaryViewModel.SecondaryContact.RefererURL = sourcePage == SourcePageEnum.ProfileSummary ? GetRefererURL()
            : Constants.CabProviderBaseURL + "/primary-contact-information";
            return View(profileSummaryViewModel.SecondaryContact);
        }


        [HttpPost("secondary-contact-information")]
        public async Task<IActionResult> SaveSecondaryContact(SecondaryContactViewModel secondaryContactViewModel, string action)
        {
            SourcePageEnum? sourcePage = secondaryContactViewModel.SourcePage;
            ProfileSummaryViewModel profileSummary = GetProfileSummary();
            secondaryContactViewModel.IsInCompleteApplication = profileSummary.IsInCompleteApplication;
            ValidationHelper.ValidateDuplicateFields(
            ModelState,primaryValue: profileSummary.PrimaryContact?.PrimaryContactEmail, secondaryValue: secondaryContactViewModel.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                "PrimaryContactEmail",
                "SecondaryContactEmail",
                "Email address of secondary contact cannot be the same as primary contact"));

            ValidationHelper.ValidateDuplicateFields(
                ModelState, primaryValue: profileSummary.PrimaryContact?.PrimaryContactTelephoneNumber, secondaryValue: secondaryContactViewModel.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                "PrimaryContactTelephoneNumber",
                "SecondaryContactTelephoneNumber",
                "Telephone number of secondary contact cannot be the same as primary contact"));

            if (ModelState.IsValid)
            {
                profileSummary.SecondaryContact = secondaryContactViewModel;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);              
                return await HandlePrimaryAndSecondaryContactActions(action, profileSummary, sourcePage, "PublicContactEmail");
            }
            else
            {
                return View("SecondaryContact", secondaryContactViewModel);
            }
        }

        #endregion

        #region Public contact email

        [HttpGet("public-email")]
        public IActionResult PublicContactEmail(SourcePageEnum sourcePage)
        {
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
            : Constants.CabProviderBaseURL + "/secondary-contact-information";
            return View("PublicContactEmail", profileSummaryViewModel);
        }

        [HttpPost("public-email")]
        public async Task<IActionResult> SavePublicContactEmail(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;
          
            if (ModelState["PublicContactEmail"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.PublicContactEmail = profileSummaryViewModel.PublicContactEmail;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "TelephoneNumber");
               
            }
            else
            {
                return View("PublicContactEmail", profileSummaryViewModel);
            }
        }

        #endregion

        #region Telephone number

        [HttpGet("public-telephone")]
        public IActionResult TelephoneNumber(SourcePageEnum sourcePage)
        {            
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
            : Constants.CabProviderBaseURL + "/public-email";
            return View("TelephoneNumber", profileSummaryViewModel);
        }

        [HttpPost("public-telephone")]
        public async Task<IActionResult> SaveTelephoneNumber(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;        
            if (ModelState["ProviderTelephoneNumber"]?.Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ProviderTelephoneNumber = profileSummaryViewModel.ProviderTelephoneNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "WebsiteAddress");               
            }
            else
            {
                return View("TelephoneNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region Website address

        [HttpGet("public-website")]
        public IActionResult WebsiteAddress(SourcePageEnum sourcePage)
        {
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
           || sourcePage == SourcePageEnum.ProfileEditSummary || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
          : Constants.CabProviderBaseURL + "/public-telephone";
            return View("WebsiteAddress", profileSummaryViewModel);
        }

        [HttpPost("public-website")]
        public async Task<IActionResult> SaveWebsiteAddress(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;          
            if (ModelState["ProviderWebsiteAddress"]?.Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ProviderWebsiteAddress = profileSummaryViewModel.ProviderWebsiteAddress;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "LinkToContactPage");
               
            }
            else
            {
                return View("WebsiteAddress", profileSummaryViewModel);
            }
        }

        #endregion
        
        #region Link To Contact Page

        [HttpGet("link-to-contact-page")]
        public IActionResult LinkToContactPage(SourcePageEnum sourcePage)
        {
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            profileSummaryViewModel.SourcePage = sourcePage;
            profileSummaryViewModel.RefererURL = sourcePage == SourcePageEnum.ProfileSummary
            || sourcePage == SourcePageEnum.ProfileEditSummary || sourcePage == SourcePageEnum.ProfileDetails ? GetRefererURL()
            : Constants.CabProviderBaseURL + "/public-website";
            return View("LinkToContactPage", profileSummaryViewModel);
        }


        [HttpPost("link-to-contact-page")]
        public async Task<IActionResult> SaveLinkToContactPage(ProfileSummaryViewModel profileSummaryViewModel, string action)
        {
            SourcePageEnum? sourcePage = profileSummaryViewModel.SourcePage;          

            if (ModelState["LinkToContactPage"]?.Errors.Count == 0 || !ModelState.ContainsKey("LinkToContactPage"))
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.LinkToContactPage = profileSummaryViewModel.LinkToContactPage;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return await HandleActions(action, profileSummary, sourcePage, "ProfileSummary");
            }
            else
            {
                return View("LinkToContactPage", profileSummaryViewModel);
            }
        }

        #endregion


        #region Summary

        [HttpGet("check-answers")]
        public IActionResult ProfileSummary()
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }

        [HttpPost("check-answers")]
        public async Task<IActionResult> SaveProfileSummary()
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
            ProviderProfileDto providerDto = ViewModelHelper.MapViewModelToDto(summaryViewModel, cabUserDto.Id, CabId);
            
            if (providerDto == null)
                throw new InvalidOperationException("An error occurred while saving the profile summary.");

            GenericResponse genericResponse = await cabService.SaveProviderProfile(providerDto, UserEmail);
            if (genericResponse.Success)
            {
                return RedirectToAction("InformationSubmitted");
            }
            else
            {
                throw new InvalidOperationException("Failed to save provider profile.");
            }
        }

        /// <summary>
        ///Final page if save success
        /// </summary>       
        /// <returns></returns>
        [HttpGet("profile-submitted")]
        public IActionResult InformationSubmitted()
        {
            HttpContext?.Session.Remove("ProfileSummary");
            ViewBag.Email = UserEmail;
            return View();
        }

        #endregion

        # region Provider Details
        
        [HttpGet("provider-details/{providerId}")]
        public async Task<IActionResult> ProviderDetails(int providerId)
        {
            var provider = await cabService.GetProviderWithLatestVersionServices(providerId, CabId);
            SetProviderDataToSession(provider);
            if (provider == null)
            {
                return NotFound();
            }

            var viewModel = new ProviderDetailsViewModel
            {
                Provider = provider               
            };

            return View(viewModel);
        }
        
        # endregion

        #region Private methods      

        private ProfileSummaryViewModel GetProfileSummary()
        {
            ProfileSummaryViewModel model = HttpContext?.Session.Get<ProfileSummaryViewModel>("ProfileSummary") ??
                                            new ProfileSummaryViewModel
                                            {
                                                PrimaryContact = new PrimaryContactViewModel(),
                                                SecondaryContact = new SecondaryContactViewModel(),
                                                IsInCompleteApplication = true
                                            };
            return model;
        }

     

        protected void SetProviderDataToSession(ProviderProfileDto providerProfileDto)
        {
            ProfileSummaryViewModel profileSummaryViewModel = new();
            profileSummaryViewModel.ProviderId = providerProfileDto.Id;
            profileSummaryViewModel.RegisteredName = providerProfileDto.RegisteredName;
            profileSummaryViewModel.TradingName = providerProfileDto?.TradingName;
            profileSummaryViewModel.HasRegistrationNumber = providerProfileDto?.HasRegistrationNumber;            
            profileSummaryViewModel.CompanyRegistrationNumber = providerProfileDto.CompanyRegistrationNumber;
            profileSummaryViewModel.DUNSNumber = providerProfileDto.DUNSNumber;
            profileSummaryViewModel.HasParentCompany = providerProfileDto.HasParentCompany;
            profileSummaryViewModel.ParentCompanyRegisteredName = providerProfileDto.ParentCompanyRegisteredName;
            profileSummaryViewModel.ParentCompanyLocation = providerProfileDto.ParentCompanyLocation;
            profileSummaryViewModel.PrimaryContact = new PrimaryContactViewModel();
            profileSummaryViewModel.PrimaryContact.PrimaryContactFullName = providerProfileDto.PrimaryContactFullName;
            profileSummaryViewModel.PrimaryContact.PrimaryContactJobTitle = providerProfileDto.PrimaryContactJobTitle;
            profileSummaryViewModel.PrimaryContact.PrimaryContactEmail = providerProfileDto.PrimaryContactEmail;
            profileSummaryViewModel.SecondaryContact = new SecondaryContactViewModel();
            profileSummaryViewModel.PrimaryContact.PrimaryContactTelephoneNumber = providerProfileDto.PrimaryContactTelephoneNumber;
            profileSummaryViewModel.SecondaryContact.SecondaryContactFullName = providerProfileDto.SecondaryContactFullName;
            profileSummaryViewModel.SecondaryContact.SecondaryContactJobTitle = providerProfileDto.SecondaryContactJobTitle;
            profileSummaryViewModel.SecondaryContact.SecondaryContactEmail = providerProfileDto.SecondaryContactEmail;
            profileSummaryViewModel.SecondaryContact.SecondaryContactTelephoneNumber = providerProfileDto.SecondaryContactTelephoneNumber;
            profileSummaryViewModel.PublicContactEmail = providerProfileDto.PublicContactEmail;
            profileSummaryViewModel.ProviderTelephoneNumber = providerProfileDto.ProviderTelephoneNumber;
            profileSummaryViewModel.ProviderWebsiteAddress = providerProfileDto.ProviderWebsiteAddress;
            profileSummaryViewModel.LinkToContactPage = providerProfileDto.LinkToContactPage;
            profileSummaryViewModel.ProviderStatus = providerProfileDto.ProviderStatus;
            profileSummaryViewModel.IsInCompleteApplication = providerProfileDto.Id == 0 || providerProfileDto.ProviderStatus == ProviderStatusEnum.SavedAsDraft;
            HttpContext?.Session.Set("ProfileSummary", profileSummaryViewModel);

        }

        private async Task<IActionResult> SaveAsDraftAndRedirect(ProfileSummaryViewModel profileSummary)
        {
            ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(profileSummary);
            providerProfileDto.ProviderStatus = ProviderStatusEnum.SavedAsDraft;
            providerProfileDto.ProviderProfileCabMapping = [new ProviderProfileCabMappingDto { CabId = CabId }];
            GenericResponse genericResponse = await cabService.SaveProviderProfile(providerProfileDto, UserEmail);
            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ProfileSummary");
                return RedirectToAction("ProviderDetails", new { providerId  = genericResponse.InstanceId});
            }
            else
            {
                throw new InvalidOperationException("SaveAsDraftAndRedirect: Failed to save provider draft");
            }

        }


        private async Task<IActionResult> HandleHasCompanyRegistration(string action, ProfileSummaryViewModel profileSummary, SourcePageEnum? sourcePage)
        {
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(profileSummary.HasRegistrationNumber))
                    {
                        profileSummary.DUNSNumber = null;
                        HttpContext?.Session.Set("ProfileSummary", profileSummary);
                        return RedirectToAction("CompanyRegistrationNumber", new { sourcePage });
                    }
                    else
                    {
                        profileSummary.CompanyRegistrationNumber = null;
                        HttpContext?.Session.Set("ProfileSummary", profileSummary);
                        return RedirectToAction("DUNSNumber", new { sourcePage });                      
                    }

                case "draft":
                    return await SaveAsDraftAndRedirect(profileSummary);
                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandleHasParentCompany(string action, ProfileSummaryViewModel profileSummary, SourcePageEnum? sourcePage, string nextPage)
        {
            bool isDraft = profileSummary.ProviderStatus == ProviderStatusEnum.SavedAsDraft;
            switch (action)
            {
                case "continue":
                    if (Convert.ToBoolean(profileSummary.HasParentCompany))
                    {                   
                        HttpContext?.Session.Set("ProfileSummary", profileSummary);
                        return RedirectToAction("ParentCompanyRegisteredName", new { sourcePage });
                    }
                    else
                    {
                        profileSummary.ParentCompanyLocation = null;
                        profileSummary.ParentCompanyRegisteredName = null;
                        HttpContext?.Session.Set("ProfileSummary", profileSummary);                   

                        return sourcePage switch
                        {
                            SourcePageEnum.ProfileSummary => RedirectToAction("ProfileSummary"),
                            SourcePageEnum.ProfileEditSummary => RedirectToAction("ProfileEditSummary", "CabProviderEdit"),
                            SourcePageEnum.ProfileDetails => isDraft ? await SaveAsDraftAndRedirect(profileSummary) : RedirectToAction("ProfileEditSummary", "CabProviderEdit"),
                            SourcePageEnum.ProfileDraft => RedirectToAction(nextPage),
                            _ => RedirectToAction(nextPage)
                        };
                    }

                case "draft":
                    return await SaveAsDraftAndRedirect(profileSummary);

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandleActions(string action, ProfileSummaryViewModel profileSummary, SourcePageEnum? sourcePage, string nextPage)
        {
            bool isDraft = profileSummary.ProviderStatus == ProviderStatusEnum.SavedAsDraft;
            switch (action)
            {
                case "continue":
                    
                        return  sourcePage switch
                        {
                            SourcePageEnum.ProfileSummary => RedirectToAction("ProfileSummary"),
                            SourcePageEnum.ProfileEditSummary  => RedirectToAction("ProfileEditSummary", "CabProviderEdit"),
                             SourcePageEnum.ProfileDetails =>  isDraft ? await SaveAsDraftAndRedirect(profileSummary) : RedirectToAction("ProfileEditSummary", "CabProviderEdit"),
                            SourcePageEnum.ProfileDraft => RedirectToAction(nextPage),
                           _ => RedirectToAction(nextPage)
                        };

                case "draft":
                    return await SaveAsDraftAndRedirect(profileSummary);            

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        private async Task<IActionResult> HandlePrimaryAndSecondaryContactActions(string action, ProfileSummaryViewModel profileSummary, SourcePageEnum? sourcePage, string nextPage)
        {            
            switch (action)
            {
                case "continue":

                    return sourcePage switch
                    {
                        SourcePageEnum.ProfileSummary => RedirectToAction("ProfileSummary"),                       
                        SourcePageEnum.ProfileDetails => await SaveAsDraftAndRedirect(profileSummary),
                        SourcePageEnum.ProfileDraft => RedirectToAction(nextPage),
                        _ => RedirectToAction(nextPage)
                    };

                case "draft":
                    return await SaveAsDraftAndRedirect(profileSummary);
                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }

        #endregion
    }
}