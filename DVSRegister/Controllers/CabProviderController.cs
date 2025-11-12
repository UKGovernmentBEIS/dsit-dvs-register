using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB.Provider;
using DVSRegister.Models.CAB;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/create-profile")] 
    public class CabProviderController(ICabService cabService, IUserService userService, IActionLogService actionLogService, ILogger<CabProviderController> logger) : BaseController(logger)
    {
        private readonly ICabService cabService = cabService;
        private readonly IUserService userService = userService;
        private readonly IActionLogService actionLogService = actionLogService;

        [HttpGet("before-you-start")]
        public IActionResult BeforeYouStart() => View();


        #region Registered Name

        [HttpGet("reg-name")]
        public IActionResult RegisteredName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("RegisteredName", profileSummaryViewModel);
        }

        [HttpPost("reg-name")]
        public async Task<IActionResult> SaveRegisteredName(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;

            if (!string.IsNullOrEmpty(profileSummaryViewModel.RegisteredName))
            {
                bool registeredNameExist =
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
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("TradingName");
            }
            else
            {
                return View("RegisteredName", profileSummaryViewModel);
            }
        }

        #endregion


        #region Trading Name

        [HttpGet("trading-name")]
        public IActionResult TradingName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("TradingName", profileSummaryViewModel);
        }

        [HttpPost("trading-name")]
        public IActionResult SaveTradingName(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["TradingName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.TradingName = profileSummaryViewModel.TradingName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("HasRegistrationNumber");
            }
            else
            {
                return View("TradingName", profileSummaryViewModel);
            }
        }

        #endregion


        #region HasCompanyRegistrationNumber

        [HttpGet("company-number")]
        public IActionResult HasRegistrationNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }


        [HttpPost("company-number")]
        public IActionResult SaveHasRegistrationNumber(ProfileSummaryViewModel profileSummaryViewModel)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["HasRegistrationNumber"].Errors.Count == 0)
            {
                summaryViewModel.HasRegistrationNumber = profileSummaryViewModel.HasRegistrationNumber;
                if (Convert.ToBoolean(summaryViewModel.HasRegistrationNumber))
                {
                    summaryViewModel.DUNSNumber = null;
                    HttpContext?.Session.Set("ProfileSummary", summaryViewModel);
                    return RedirectToAction("CompanyRegistrationNumber", new { fromSummaryPage = fromSummaryPage });
                }
                else
                {
                    summaryViewModel.CompanyRegistrationNumber = null;
                    HttpContext?.Session.Set("ProfileSummary", summaryViewModel);
                    return RedirectToAction("DUNSNumber", new { fromSummaryPage = fromSummaryPage });
                }
            }
            else
            {
                return View("HasRegistrationNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region Registration number

        [HttpGet("company-number-input")]
        public IActionResult CompanyRegistrationNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }

        [HttpPost("company-number-input")]
        public IActionResult SaveCompanyRegistrationNumber(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["CompanyRegistrationNumber"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.CompanyRegistrationNumber = profileSummaryViewModel.CompanyRegistrationNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("HasParentCompany");
            }
            else
            {
                return View("CompanyRegistrationNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region DUNSNumber

        [HttpGet("duns-number")]
        public IActionResult DUNSNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }

        [HttpPost("duns-number")]
        public IActionResult SaveDUNSNumber(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["DUNSNumber"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.DUNSNumber = profileSummaryViewModel.DUNSNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("HasParentCompany");
            }
            else
            {
                return View("DUNSNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region HasParentCompany

        [HttpGet("parent-company")]
        public IActionResult HasParentCompany(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }

        [HttpPost("parent-company")]
        public IActionResult SaveHasParentCompany(ProfileSummaryViewModel profileSummaryViewModel)
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            profileSummaryViewModel.HasRegistrationNumber =
                summaryViewModel.HasRegistrationNumber; // required to add condition for back link
            if (ModelState["HasParentCompany"].Errors.Count == 0)
            {
                summaryViewModel.HasParentCompany = profileSummaryViewModel.HasParentCompany;
                if (Convert.ToBoolean(summaryViewModel.HasParentCompany))
                {
                    HttpContext?.Session.Set("ProfileSummary", summaryViewModel);
                    return RedirectToAction("ParentCompanyRegisteredName", new { fromSummaryPage = fromSummaryPage });
                }
                else
                {
                    summaryViewModel.ParentCompanyLocation = null;
                    summaryViewModel.ParentCompanyRegisteredName = null;
                    HttpContext?.Session.Set("ProfileSummary", summaryViewModel);
                    return RedirectToAction("PrimaryContact", new { fromSummaryPage = fromSummaryPage });
                }
            }
            else
            {
                return View("HasParentCompany", profileSummaryViewModel);
            }
        }

        #endregion

        #region Parent company registered name

        [HttpGet("parent-company-registered-name-input")]
        public IActionResult ParentCompanyRegisteredName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }


        [HttpPost("parent-company-registered-name-input")]
        public IActionResult SaveParentCompanyRegisteredName(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ParentCompanyRegisteredName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ParentCompanyRegisteredName = profileSummaryViewModel.ParentCompanyRegisteredName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("ParentCompanyLocation");
            }
            else
            {
                return View("ParentCompanyRegisteredName", profileSummaryViewModel);
            }
        }

        #endregion

        #region Parent company location

        [HttpGet("parent-company-location-input")]
        public IActionResult ParentCompanyLocation(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }

        [HttpPost("parent-company-location-input")]
        public IActionResult SaveParentCompanyLocation(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ParentCompanyLocation"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ParentCompanyLocation = profileSummaryViewModel.ParentCompanyLocation;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PrimaryContact");
            }
            else
            {
                return View("ParentCompanyLocation", profileSummaryViewModel);
            }
        }

        #endregion

        #region Primary Contact

        [HttpGet("primary-contact-information")]
        public IActionResult PrimaryContact(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ViewBag.hasParentCompany = profileSummaryViewModel.HasParentCompany;
            return View(profileSummaryViewModel.PrimaryContact);
        }

        [HttpPost("primary-contact-information")]
        public IActionResult SavePrimaryContact(PrimaryContactViewModel primaryContactViewModel)
        {
            bool fromSummaryPage = primaryContactViewModel.FromSummaryPage;
            primaryContactViewModel.FromSummaryPage = false;

            ProfileSummaryViewModel profileSummary = GetProfileSummary();


            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryContactViewModel.PrimaryContactEmail,
                profileSummary.SecondaryContact?.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                )
            );

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: primaryContactViewModel.PrimaryContactTelephoneNumber,
                secondaryValue: profileSummary.SecondaryContact?.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                )
            );

            if (ModelState.IsValid)
            {
                profileSummary.PrimaryContact = primaryContactViewModel;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("SecondaryContact");
            }
            else
            {
                return View("PrimaryContact", primaryContactViewModel);
            }
        }

        #endregion

        #region Secondary Contact

        [HttpGet("secondary-contact-information")]
        public IActionResult SecondaryContact(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View(profileSummaryViewModel.SecondaryContact);
        }


        [HttpPost("secondary-contact-information")]
        public IActionResult SaveSecondaryContact(SecondaryContactViewModel secondaryContactViewModel)
        {
            bool fromSummaryPage = secondaryContactViewModel.FromSummaryPage;
            secondaryContactViewModel.FromSummaryPage = false;

            ProfileSummaryViewModel profileSummary = GetProfileSummary();

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: profileSummary.PrimaryContact?.PrimaryContactEmail,
                secondaryValue: secondaryContactViewModel.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                )
            );

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: profileSummary.PrimaryContact?.PrimaryContactTelephoneNumber,
                secondaryValue: secondaryContactViewModel.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                )
            );

            if (ModelState.IsValid)
            {
                profileSummary.SecondaryContact = secondaryContactViewModel;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PublicContactEmail");
            }
            else
            {
                return View("SecondaryContact", secondaryContactViewModel);
            }
        }

        #endregion

        #region Public contact email

        [HttpGet("public-email")]
        public IActionResult PublicContactEmail(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("PublicContactEmail", profileSummaryViewModel);
        }

        [HttpPost("public-email")]
        public IActionResult SavePublicContactEmail(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["PublicContactEmail"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.PublicContactEmail = profileSummaryViewModel.PublicContactEmail;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("TelephoneNumber");
            }
            else
            {
                return View("PublicContactEmail", profileSummaryViewModel);
            }
        }

        #endregion

        #region Telephone number

        [HttpGet("public-telephone")]
        public IActionResult TelephoneNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("TelephoneNumber", profileSummaryViewModel);
        }

        [HttpPost("public-telephone")]
        public IActionResult SaveTelephoneNumber(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ProviderTelephoneNumber"]?.Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ProviderTelephoneNumber = profileSummaryViewModel.ProviderTelephoneNumber;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("WebsiteAddress");
            }
            else
            {
                return View("TelephoneNumber", profileSummaryViewModel);
            }
        }

        #endregion

        #region Website address

        [HttpGet("public-website")]
        public IActionResult WebsiteAddress(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("WebsiteAddress", profileSummaryViewModel);
        }

        [HttpPost("public-website")]
        public IActionResult SaveWebsiteAddress(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["ProviderWebsiteAddress"]?.Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ProviderWebsiteAddress = profileSummaryViewModel.ProviderWebsiteAddress;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return RedirectToAction("ProfileSummary");
            }
            else
            {
                return View("WebsiteAddress", profileSummaryViewModel);
            }
        }

        #endregion


        #region Providers Link To Contact Page

        [HttpGet("providers-link-to-contact-page")]
        public IActionResult ProvidersLinkToContactPage(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("ProvidersLinkToContactPage", profileSummaryViewModel);
        }


        [HttpPost("providers-link-to-contact-page")]
        public IActionResult SaveProvidersLinkToContactPage(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;


            if (ModelState.IsValid)
            {

                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.ProvidersLinkToContactPage = profileSummaryViewModel.ProvidersLinkToContactPage;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);

                return RedirectToAction("ProfileSummary");
            }
            else
            {
                return View("ProvidersLinkToContactPage", profileSummaryViewModel);
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
            ProviderProfileDto providerDto = MapViewModelToDto(summaryViewModel, cabUserDto.Id);
            
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
        public async Task<IActionResult> ProviderDetails(int providerId, string CurrentSort = "service", string CurrentSortAction = "ascending", string NewSort = "")
        {
            if (NewSort != string.Empty)
            {
                if (CurrentSort == NewSort)
                {
                    CurrentSortAction = CurrentSortAction == "ascending" ? "descending" : "ascending";
                }
                else
                {
                    CurrentSort = NewSort;
                    CurrentSortAction = "ascending";
                }
            }

            var provider = await cabService.GetProvider(providerId, CabId);
            
            if (provider == null)
            {
                return NotFound();
            }

            // Sort services
            if (provider.Services != null && provider.Services.Any())
            {
                provider.Services = CurrentSort switch
                {
                    "service" => CurrentSortAction == "ascending" 
                        ? provider.Services.OrderBy(s => s.ServiceName).ToList()
                        : provider.Services.OrderByDescending(s => s.ServiceName).ToList(),
                    "status" => CurrentSortAction == "ascending"
                        ? provider.Services.OrderBy(s => s.ServiceStatus).ToList()
                        : provider.Services.OrderByDescending(s => s.ServiceStatus).ToList(),
                    _ => provider.Services.OrderBy(s => s.ServiceName).ToList()
                };
            }

            var viewModel = new ProviderDetailsViewModel
            {
                Provider = provider,
                CurrentSort = CurrentSort,
                CurrentSortAction = CurrentSortAction
            };

            ViewBag.IsEditable = true;

            return View(viewModel);
        }
        
        # endregion

        #region Edit Company Information

        [HttpGet("edit-company-information")]
        public async Task<IActionResult> EditCompanyInformation(int providerId)
        {
          
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            if (providerId <= 0)
                throw new ArgumentException("Invalid ProviderId: Unable to retrieve company information.");

            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
    
            if (!cabService.CheckCompanyInfoEditable(providerProfileDto))
                throw new InvalidOperationException("Company information is not editable.");
            
            return View(new CompanyViewModel
            {
                RegisteredName = providerProfileDto.RegisteredName,
                TradingName = providerProfileDto.TradingName,
                HasRegistrationNumber = providerProfileDto.HasRegistrationNumber,
                CompanyRegistrationNumber = providerProfileDto.CompanyRegistrationNumber,
                DUNSNumber = providerProfileDto.DUNSNumber,
                HasParentCompany = providerProfileDto.HasParentCompany,
                ParentCompanyRegisteredName = providerProfileDto.ParentCompanyRegisteredName,
                ParentCompanyLocation = providerProfileDto.ParentCompanyLocation,
                ProviderId = providerProfileDto.Id
            });
        }
        
        [HttpPost("edit-company-information")]
        public async Task<IActionResult> UpdateCompanyInformation(CompanyViewModel companyViewModel)
        {
            if (!string.IsNullOrEmpty(companyViewModel.RegisteredName))
            {
                bool registeredNameExist =
                    await cabService.CheckProviderRegisteredNameExists(companyViewModel.RegisteredName,
                        companyViewModel.ProviderId);
                if (registeredNameExist)
                    ModelState.AddModelError("RegisteredName", Constants.RegisteredNameExistsError);
            }

            if (ModelState.IsValid)
            {
                ProviderProfileDto previousData = await cabService.GetProvider(companyViewModel.ProviderId, CabId);
                ProviderProfileDto providerProfileDto = new()
                {
                    Id = companyViewModel.ProviderId,
                    RegisteredName = companyViewModel.RegisteredName,
                    TradingName = companyViewModel.TradingName ?? string.Empty,
                    CompanyRegistrationNumber = companyViewModel.CompanyRegistrationNumber,
                    DUNSNumber = companyViewModel.DUNSNumber,
                    ParentCompanyRegisteredName = companyViewModel.ParentCompanyRegisteredName,
                    ParentCompanyLocation = companyViewModel.ParentCompanyLocation
                };

                GenericResponse genericResponse = await cabService.UpdateCompanyInfo(providerProfileDto, UserEmail);
                if (genericResponse.Success)
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = cabService.GetCompanyValueUpdates(providerProfileDto, previousData);
                    await SaveActionLogs(ActionDetailsEnum.BusinessDetailsUpdate, previousData, current, previous, isProviderPublishedBefore);
                    return RedirectToAction("ProviderDetails", "CabProvider",new { providerId = providerProfileDto.Id });
                }
                else
                {
                    throw new InvalidOperationException("Company information update failed.");
                }
            }
            else
            {
                return View("EditCompanyInformation", companyViewModel);
            }
        }

        #endregion

        #region Edit primary contact

        [HttpGet("edit-primary-contact")]
        public async Task<IActionResult> EditPrimaryContact(int providerId)
        {
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            if (providerId <= 0)
                throw new ArgumentException("Failed to edit primary contact. Invalid ProviderId.");
            
            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
            PrimaryContactViewModel primaryContactViewModel = new()
            {
                PrimaryContactFullName = providerProfileDto.PrimaryContactFullName,
                PrimaryContactEmail = providerProfileDto.PrimaryContactEmail,
                PrimaryContactJobTitle = providerProfileDto.PrimaryContactJobTitle,
                PrimaryContactTelephoneNumber = providerProfileDto.PrimaryContactTelephoneNumber,
                ProviderId = providerProfileDto.Id
            };
            
            return View(primaryContactViewModel);
            
        }

        [HttpPost("edit-primary-contact")]
        public async Task<IActionResult> UpdatePrimaryContact(PrimaryContactViewModel primaryContactViewModel)
        {

            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (primaryContactViewModel.ProviderId <= 0) 
                throw new ArgumentException("Invalid ProviderId.");

            // Fetch the latest provider data from the database
            ProviderProfileDto previousData = await cabService.GetProvider(primaryContactViewModel.ProviderId, CabId); 
            if (previousData == null)
                throw new InvalidOperationException("ProviderProfile not found for the given ProviderId and CabId."); 
            
            
            ValidationHelper.ValidateDuplicateFields(
                ModelState, 
                primaryValue: primaryContactViewModel.PrimaryContactEmail,
                secondaryValue: previousData.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                    )
                ); 

            ValidationHelper.ValidateDuplicateFields(
                ModelState, 
                primaryValue: primaryContactViewModel.PrimaryContactTelephoneNumber,
                secondaryValue: previousData.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                    )
                );
            
            if (ModelState.IsValid) 
            {
                ProviderProfileDto providerProfileDto = new();
                providerProfileDto.PrimaryContactFullName = primaryContactViewModel.PrimaryContactFullName; 
                providerProfileDto.PrimaryContactEmail = primaryContactViewModel.PrimaryContactEmail; 
                providerProfileDto.PrimaryContactJobTitle = primaryContactViewModel.PrimaryContactJobTitle; 
                providerProfileDto.PrimaryContactTelephoneNumber = primaryContactViewModel.PrimaryContactTelephoneNumber;
                providerProfileDto.Id = previousData.Id;
                GenericResponse genericResponse = await cabService.UpdatePrimaryContact(providerProfileDto, UserEmail); 
                if (genericResponse.Success)
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = cabService.GetPrimaryContactUpdates(providerProfileDto, previousData);
                    await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, previousData, current, previous, isProviderPublishedBefore);
                    return RedirectToAction("ProviderDetails", "CabProvider", new { providerId = providerProfileDto.Id });
                }
                else
                {
                    throw new InvalidOperationException("Failed to update primary contact information.");

                }
            }
            else
            {
                return View("EditPrimaryContact", primaryContactViewModel);
            }
        }     

        #endregion

        #region Edit secondary contact

        [HttpGet("edit-secondary-contact")]
        public async Task<IActionResult> EditSecondaryContact(int providerId)
        {
            
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
      
            if (providerId <= 0)
                throw new ArgumentException("Failed to edit secondary contact. Invalid ProviderId.");
            
            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
            SecondaryContactViewModel secondaryContactViewModel = new()
            {
                SecondaryContactFullName = providerProfileDto.SecondaryContactFullName,
                SecondaryContactEmail = providerProfileDto.SecondaryContactEmail,
                SecondaryContactJobTitle = providerProfileDto.SecondaryContactJobTitle,
                SecondaryContactTelephoneNumber = providerProfileDto.SecondaryContactTelephoneNumber,
                ProviderId = providerProfileDto.Id
            };

            return View(secondaryContactViewModel);
        }

        [HttpPost("edit-secondary-contact")]
        public async Task<IActionResult> UpdateSecondaryContact(SecondaryContactViewModel secondaryContactViewModel) 
        {         
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (secondaryContactViewModel.ProviderId <= 0)
                throw new ArgumentException("Invalid ProviderId.");
            
            // Fetch the latest provider data from the database
            ProviderProfileDto previousData = await cabService.GetProvider(secondaryContactViewModel.ProviderId, CabId); 
            if (previousData == null)
                throw new InvalidOperationException("ProviderProfile not found for the given ProviderId and CabId.");
            
            ValidationHelper.ValidateDuplicateFields(
                ModelState, 
                primaryValue: previousData.PrimaryContactEmail,
                secondaryValue: secondaryContactViewModel.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                    )
                ); 
            
            ValidationHelper.ValidateDuplicateFields(
                ModelState, 
                primaryValue: previousData.PrimaryContactTelephoneNumber,
                secondaryValue: secondaryContactViewModel.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                    )
                );
            
            if (ModelState.IsValid) 
            {
                ProviderProfileDto providerProfileDto = new();
                providerProfileDto.SecondaryContactFullName = secondaryContactViewModel.SecondaryContactFullName; 
                providerProfileDto.SecondaryContactEmail = secondaryContactViewModel.SecondaryContactEmail; 
                providerProfileDto.SecondaryContactJobTitle = secondaryContactViewModel.SecondaryContactJobTitle; 
                providerProfileDto.SecondaryContactTelephoneNumber = secondaryContactViewModel.SecondaryContactTelephoneNumber;
                providerProfileDto.Id = previousData.Id;
                GenericResponse genericResponse = await cabService.UpdateSecondaryContact(providerProfileDto, UserEmail); 
                if (genericResponse.Success) 
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = cabService.GetSecondaryContactUpdates(providerProfileDto, previousData);
                    await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, previousData, current, previous, isProviderPublishedBefore);
                    return RedirectToAction("ProviderDetails", "CabProvider", new { providerId = providerProfileDto.Id }); 
                }
                else
                {
                    throw new InvalidOperationException("Failed to update secondary contact information.");
                }
            }
            else
            {
                return View("EditSecondaryContact", secondaryContactViewModel);
            }
        }

        #endregion

        #region Edit public provider information

        [HttpGet("edit-public-provider-information")]
        public async Task<IActionResult> EditPublicProviderInformation(int providerId)
        {
           
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            if (providerId <= 0)
                throw new ArgumentException("Failed to edit public provider information. Invalid ProviderId.");
            
            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
            PublicContactViewModel publicContactViewModel = new()
            {
                PublicContactEmail = providerProfileDto.PublicContactEmail,
                ProviderTelephoneNumber = providerProfileDto.ProviderTelephoneNumber,
                ProviderWebsiteAddress = providerProfileDto.ProviderWebsiteAddress,
                LinkToContactPage = providerProfileDto.LinkToContactPage,
                ProviderId = providerProfileDto.Id
            };
            return View(publicContactViewModel);
        }

        [HttpPost("edit-public-provider-information")]
        public async Task<IActionResult> UpdatePublicProviderInformation(PublicContactViewModel publicContactViewModel)
        {
            if (ModelState.IsValid)
            {
                ProviderProfileDto previousData = await cabService.GetProvider(publicContactViewModel.ProviderId, CabId);
                ProviderProfileDto providerProfileDto = new()
                {
                    PublicContactEmail = publicContactViewModel.PublicContactEmail,
                    ProviderTelephoneNumber = publicContactViewModel.ProviderTelephoneNumber,
                    ProviderWebsiteAddress = publicContactViewModel.ProviderWebsiteAddress,
                    LinkToContactPage = publicContactViewModel.LinkToContactPage,
                    Id = publicContactViewModel.ProviderId
                };

                GenericResponse genericResponse =
                    await cabService.UpdatePublicProviderInformation(providerProfileDto, UserEmail);
                if (genericResponse.Success)
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = cabService.GetPublicContactUpdates(providerProfileDto, previousData);
                    await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, previousData, current, previous, isProviderPublishedBefore);
                    return RedirectToAction("ProviderDetails", "CabProvider",  new { providerId = providerProfileDto.Id });
                }
                else
                {
                    throw new InvalidOperationException("Failed to update public provider information information.");
                }
            }
            else
            {
                return View("EditPublicProviderInformation", publicContactViewModel);
            }
        }

        #endregion


        #region Private methods

        private async Task SaveActionLogs(ActionDetailsEnum actionDetailsEnum, ProviderProfileDto providerProfileDto, 
            Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, bool isProviderPublishedBefore)
        {
            ActionLogsDto actionLogsDto = new()
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = actionDetailsEnum,
                LoggedInUserEmail = UserEmail,
                ProviderId = providerProfileDto.Id,
                ProviderName = providerProfileDto.RegisteredName,
                PreviousData = previous,
                UpdatedData = current,
                IsProviderPreviouslyPublished = isProviderPublishedBefore
            };
            await actionLogService.SaveActionLogs(actionLogsDto);
        }

        private ProfileSummaryViewModel GetProfileSummary()
        {
            ProfileSummaryViewModel model = HttpContext?.Session.Get<ProfileSummaryViewModel>("ProfileSummary") ??
                                            new ProfileSummaryViewModel
                                            {
                                                PrimaryContact = new PrimaryContactViewModel(),
                                                SecondaryContact = new SecondaryContactViewModel()
                                            };
            return model;
        }

        private ProviderProfileDto MapViewModelToDto(ProfileSummaryViewModel model, int cabUserId)
        {
            ProviderProfileDto providerDto = null;
            if (model != null && !string.IsNullOrEmpty(model.RegisteredName) && model.HasRegistrationNumber != null &&
                model.HasParentCompany != null
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactFullName) &&
                !string.IsNullOrEmpty(model?.PrimaryContact.PrimaryContactJobTitle)
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactEmail) &&
                !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactFullName) &&
                !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactJobTitle)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactEmail) &&
                !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.ProviderWebsiteAddress) && cabUserId > 0)
            {
                providerDto = new();
                providerDto.ProviderProfileCabMapping = [new ProviderProfileCabMappingDto { CabId = CabId }];
                providerDto.RegisteredName = model.RegisteredName;
                providerDto.TradingName = model.TradingName ?? string.Empty;
                providerDto.HasRegistrationNumber = model.HasRegistrationNumber ?? false;
                providerDto.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                providerDto.DUNSNumber = model.DUNSNumber;
                providerDto.HasParentCompany = model.HasParentCompany ?? false;
                providerDto.ParentCompanyRegisteredName = model.ParentCompanyRegisteredName;
                providerDto.ParentCompanyLocation = model.ParentCompanyLocation;
                providerDto.PrimaryContactFullName = model.PrimaryContact.PrimaryContactFullName;
                providerDto.PrimaryContactJobTitle = model.PrimaryContact.PrimaryContactJobTitle;
                providerDto.PrimaryContactEmail = model.PrimaryContact.PrimaryContactEmail;
                providerDto.PrimaryContactTelephoneNumber = model.PrimaryContact.PrimaryContactTelephoneNumber;
                providerDto.SecondaryContactFullName = model.SecondaryContact.SecondaryContactFullName;
                providerDto.SecondaryContactJobTitle = model.SecondaryContact.SecondaryContactJobTitle;
                providerDto.SecondaryContactEmail = model.SecondaryContact.SecondaryContactEmail;
                providerDto.SecondaryContactTelephoneNumber = model.SecondaryContact.SecondaryContactTelephoneNumber;
                providerDto.PublicContactEmail = model.PublicContactEmail;
                providerDto.ProviderTelephoneNumber = model.ProviderTelephoneNumber;
                providerDto.ProviderWebsiteAddress = model.ProviderWebsiteAddress;
                providerDto.LinkToContactPage = model.LinkToContactPage;
                providerDto.ProviderProfileCabMapping = [new ProviderProfileCabMappingDto { CabId = CabId }];
                providerDto.ProviderStatus = ProviderStatusEnum.NA;
                providerDto.CreatedTime = DateTime.UtcNow;
            }


            return providerDto;
        }
           

    #endregion
}
}