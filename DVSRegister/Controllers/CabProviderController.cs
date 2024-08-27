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
            if(!string.IsNullOrEmpty(profileSummaryViewModel.RegisteredName))
            {
                bool registeredNameExist = await cabService.CheckProviderRegisteredNameExists(profileSummaryViewModel.RegisteredName);
                if(registeredNameExist)
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
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PrimaryContact");
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
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PrimaryContact");
            }
            else
            {
                return View("DUNSNumber", profileSummaryViewModel);
            }

        }
        #endregion

        #region Primary Contact
        [HttpGet("primary-contact-information")]
        public IActionResult PrimaryContact(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;           
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ViewBag.hasRegistrationNumber = profileSummaryViewModel.HasRegistrationNumber;
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
            GenericResponse genericResponse = await cabService.SaveProviderProfile(providerDto);
            if (genericResponse.Success)
            {
                return RedirectToAction("InformationSubmitted");
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
        }
        #endregion


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
            ProviderProfileDto providerDto = new();
            if (model != null)
            {
                providerDto.RegisteredName = model.RegisteredName??string.Empty;
                providerDto.TradingName = model.TradingName??string.Empty;


                providerDto.HasRegistrationNumber = model.HasRegistrationNumber??false;
                providerDto.CompanyRegistrationNumber = model.CompanyRegistrationNumber??string.Empty;
                providerDto.DUNSNumber = model.DUNSNumber??string.Empty;
                providerDto.PrimaryContactFullName = model.PrimaryContact?.PrimaryContactFullName??string.Empty;
                providerDto.PrimaryContactJobTitle = model.PrimaryContact?.PrimaryContactJobTitle??string.Empty;
                providerDto.PrimaryContactEmail = model.PrimaryContact?.PrimaryContactEmail??string.Empty;
                providerDto.PrimaryContactTelephoneNumber = model.PrimaryContact?.PrimaryContactTelephoneNumber??string.Empty;
                providerDto.SecondaryContactFullName = model.SecondaryContact?.SecondaryContactFullName??string.Empty;
                providerDto.SecondaryContactJobTitle = model.SecondaryContact?.SecondaryContactJobTitle??string.Empty;
                providerDto.SecondaryContactEmail = model.SecondaryContact?.SecondaryContactEmail??string.Empty;
                providerDto.SecondaryContactTelephoneNumber = model.SecondaryContact?.SecondaryContactTelephoneNumber??string.Empty;
                providerDto.PublicContactEmail= model.PublicContactEmail??string.Empty;
                providerDto.ProviderTelephoneNumber = model.ProviderTelephoneNumber??string.Empty;
                providerDto.ProviderWebsiteAddress = model.ProviderWebsiteAddress??string.Empty;
                providerDto.CabUserId = cabUserId;
                providerDto.ProviderStatus = ProviderStatusEnum.Unpublished;
                providerDto.CreatedTime = DateTime.UtcNow;
            }


            return providerDto;

        }
        #endregion

    }
}
