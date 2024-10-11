using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/create-profile")]
    [ValidCognitoToken]
    public class CabProviderController : Controller
    {

        private readonly ICabService cabService;
        private readonly IUserService userService;


        public CabProviderController(ICabService cabService, IUserService userService)
        {
            this.cabService = cabService;
            this.userService = userService;
        }

        [HttpGet("before-you-start")]
        public IActionResult BeforeYouStart()
        {            
            return View();
        }     


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
                bool registeredNameExist = await cabService.CheckProviderRegisteredNameExists(profileSummaryViewModel.RegisteredName);
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
            profileSummaryViewModel.HasRegistrationNumber = summaryViewModel.HasRegistrationNumber; // required to add condition for back link
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
            if (ModelState.IsValid)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();             
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
            if (ModelState.IsValid)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
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
            if (ModelState["ProviderTelephoneNumber"]?.Errors.Count ==0)
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
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            CabUserDto cabUserDto = await userService.GetUser(email);
            ProviderProfileDto providerDto = MapViewModelToDto(summaryViewModel,cabUserDto.Id);
            if(providerDto !=null)
            {
                GenericResponse genericResponse = await cabService.SaveProviderProfile(providerDto);
                if (genericResponse.Success)
                {
                    return RedirectToAction("InformationSubmitted");
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
                }
            }
            else
            {
                return RedirectToAction("CabHandleException", "Error");
            }
          
        }

        /// <summary>
        ///Final page if save success
        /// </summary>       
        /// <returns></returns>
        [HttpGet("profile-submitted")]
        public IActionResult InformationSubmitted()
        {
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty;
            HttpContext?.Session.Remove("ProfileSummary");
            ViewBag.Email = email;
            return View();

        }
        #endregion


        #region Edit Company Information
        [HttpGet("edit-company-information")]
        public IActionResult EditCompanyInformation()
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            CompanyViewModel companyViewModel = new()
            {
                RegisteredName = providerProfileDto.RegisteredName,
                TradingName = providerProfileDto.TradingName,
                HasRegistrationNumber = providerProfileDto.HasRegistrationNumber,
                CompanyRegistrationNumber =providerProfileDto.CompanyRegistrationNumber,
                DUNSNumber = providerProfileDto.DUNSNumber,
                HasParentCompany = providerProfileDto.HasParentCompany,
                ParentCompanyRegisteredName = providerProfileDto.ParentCompanyRegisteredName,
                ParentCompanyLocation = providerProfileDto.ParentCompanyLocation,
                ProviderId = providerProfileDto.Id
            };

            return View(companyViewModel);

        }
        [HttpPost("edit-company-information")]
        public async Task<IActionResult> UpdateCompanyInformation(CompanyViewModel companyViewModel)
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            if (!string.IsNullOrEmpty(companyViewModel.RegisteredName))
            {
                bool registeredNameExist = await cabService.CheckProviderRegisteredNameExists(companyViewModel.RegisteredName,companyViewModel.ProviderId);
                if (registeredNameExist)
                {
                    ModelState.AddModelError("RegisteredName", Constants.RegisteredNameExistsError);
                }
            }
            if (ModelState.IsValid)
            {
                providerProfileDto.RegisteredName = companyViewModel.RegisteredName;
                providerProfileDto.TradingName = companyViewModel.TradingName??string.Empty;              
                providerProfileDto.CompanyRegistrationNumber =companyViewModel.CompanyRegistrationNumber;
                providerProfileDto.DUNSNumber = companyViewModel.DUNSNumber;
                providerProfileDto.ParentCompanyRegisteredName = companyViewModel.ParentCompanyRegisteredName;
                providerProfileDto.ParentCompanyLocation = companyViewModel.ParentCompanyLocation;

                GenericResponse genericResponse = await cabService.UpdateProviderProfile(providerProfileDto);
                if (genericResponse.Success)
                {
                    return RedirectToAction("ProviderProfileDetails","Cab", new { providerId = providerProfileDto.Id });
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
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
        public IActionResult EditPrimaryContact()
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            PrimaryContactViewModel primaryContactViewModel = new()
            {
                PrimaryContactFullName = providerProfileDto.PrimaryContactFullName,
                PrimaryContactEmail = providerProfileDto.PrimaryContactEmail,
                PrimaryContactJobTitle = providerProfileDto.PrimaryContactJobTitle,
                PrimaryContactTelephoneNumber =providerProfileDto.PrimaryContactTelephoneNumber,
                ProviderId = providerProfileDto.Id
            };

            return View(primaryContactViewModel);

        }
        [HttpPost("edit-primary-contact")]
        public async Task<IActionResult> UpdatePrimaryContact(PrimaryContactViewModel primaryContactViewModel)
        {

            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            if (ModelState.IsValid)
            {
                providerProfileDto.PrimaryContactFullName = primaryContactViewModel.PrimaryContactFullName;
                providerProfileDto.PrimaryContactEmail = primaryContactViewModel.PrimaryContactEmail;
                providerProfileDto.PrimaryContactJobTitle =primaryContactViewModel.PrimaryContactJobTitle;
                providerProfileDto.PrimaryContactTelephoneNumber = primaryContactViewModel.PrimaryContactTelephoneNumber;

                GenericResponse genericResponse = await cabService.UpdateProviderProfile(providerProfileDto);
                if (genericResponse.Success)
                {
                    return RedirectToAction("ProviderProfileDetails", "Cab", new { providerId = providerProfileDto.Id });
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
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
        public IActionResult EditSecondaryContact()
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            SecondaryContactViewModel secondaryContactViewModel = new()
            {
                SecondaryContactFullName = providerProfileDto.SecondaryContactFullName,
                SecondaryContactEmail = providerProfileDto.SecondaryContactEmail,
                SecondaryContactJobTitle = providerProfileDto.SecondaryContactJobTitle,
                SecondaryContactTelephoneNumber =providerProfileDto.SecondaryContactTelephoneNumber,          
                ProviderId = providerProfileDto.Id
            };

            return View(secondaryContactViewModel);

        }
        [HttpPost("edit-secondary-contact")]
        public async Task<IActionResult> UpdateSecondaryContact(SecondaryContactViewModel secondaryContactViewModel)
        {
           
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            if (ModelState.IsValid)
            {
                providerProfileDto.SecondaryContactFullName = secondaryContactViewModel.SecondaryContactFullName;
                providerProfileDto.SecondaryContactEmail = secondaryContactViewModel.SecondaryContactEmail;
                providerProfileDto.SecondaryContactJobTitle =secondaryContactViewModel.SecondaryContactJobTitle;
                providerProfileDto.SecondaryContactTelephoneNumber = secondaryContactViewModel.SecondaryContactTelephoneNumber;

                GenericResponse genericResponse = await cabService.UpdateProviderProfile(providerProfileDto);
                if (genericResponse.Success)
                {
                    return RedirectToAction("ProviderProfileDetails", "Cab", new { providerId = providerProfileDto.Id });
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
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
        public IActionResult EditPublicProviderInformation()
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            PublicContactViewModel publicContactViewModel = new()
            {
                PublicContactEmail = providerProfileDto.PublicContactEmail,
                ProviderTelephoneNumber = providerProfileDto.ProviderTelephoneNumber,
                ProviderWebsiteAddress = providerProfileDto.ProviderWebsiteAddress,              
                ProviderId = providerProfileDto.Id
            };
            return View(publicContactViewModel);

        }
        [HttpPost("edit-public-provider-information")]
        public async Task<IActionResult> UpdatePublicProviderInformation(PublicContactViewModel publicContactViewModel)
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();
            if (ModelState.IsValid)
            {
                providerProfileDto.PublicContactEmail = publicContactViewModel.PublicContactEmail;
                providerProfileDto.ProviderTelephoneNumber = publicContactViewModel.ProviderTelephoneNumber;
                providerProfileDto.ProviderWebsiteAddress =publicContactViewModel.ProviderWebsiteAddress;        

                GenericResponse genericResponse = await cabService.UpdateProviderProfile(providerProfileDto);
                if (genericResponse.Success)
                {
                    return RedirectToAction("ProviderProfileDetails", "Cab", new { providerId = providerProfileDto.Id });
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
                }
            }
            else
            {
                return View("EditPublicProviderInformation", publicContactViewModel);
            }

        }
        #endregion


        #region Private methods
        private ProfileSummaryViewModel GetProfileSummary()
        {
            ProfileSummaryViewModel model = HttpContext?.Session.Get<ProfileSummaryViewModel>("ProfileSummary") ?? new ProfileSummaryViewModel
            {
                PrimaryContact = new PrimaryContactViewModel(),
                SecondaryContact = new SecondaryContactViewModel()
            };
            return model;
        }

        private ProviderProfileDto MapViewModelToDto(ProfileSummaryViewModel model, int cabUserId)
        {
            
            ProviderProfileDto providerDto = null;
            if (model != null && !string.IsNullOrEmpty(model.RegisteredName)  && model.HasRegistrationNumber!=null && model.HasParentCompany!=null
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactFullName) && !string.IsNullOrEmpty(model?.PrimaryContact.PrimaryContactJobTitle)
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactEmail) && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactFullName) && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactJobTitle)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactEmail) &&  !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.PublicContactEmail)  && !string.IsNullOrEmpty(model.ProviderWebsiteAddress) && cabUserId>0)
            {
                providerDto = new();
                providerDto.RegisteredName = model.RegisteredName;
                providerDto.TradingName = model.TradingName??string.Empty;
                providerDto.HasRegistrationNumber = model.HasRegistrationNumber??false;
                providerDto.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                providerDto.DUNSNumber = model.DUNSNumber;
                providerDto.HasParentCompany =  model.HasParentCompany??false;
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
                providerDto.PublicContactEmail= model.PublicContactEmail;
                providerDto.ProviderTelephoneNumber = model.ProviderTelephoneNumber;
                providerDto.ProviderWebsiteAddress = model.ProviderWebsiteAddress;
                providerDto.CabUserId = cabUserId;
                providerDto.ProviderStatus = ProviderStatusEnum.Unpublished;
                providerDto.CreatedTime = DateTime.UtcNow;
            }


            return providerDto;

        }
        #endregion

    }
}
