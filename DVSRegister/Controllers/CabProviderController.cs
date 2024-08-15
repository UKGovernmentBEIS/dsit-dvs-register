using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-provider")]
    /*[ValidCognitoToken]*/
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
        //First 5 Screens


        #region Registered Name

        [HttpGet("submit-profile/provider-registered-name")]
        public IActionResult RegisteredName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("RegisteredName", profileSummaryViewModel);
        }
        [HttpPost("submit-profile/service-providers-registered-name")]
        public IActionResult SaveRegisteredName(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["RegisteredName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.PublicContactEmail = profileSummaryViewModel.PublicContactEmail;
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

        [HttpGet("submit-profile/provider-trading-name")]
        public IActionResult TradingName(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("TradingName", profileSummaryViewModel);
        }
        [HttpPost("submit-profile/service-providers-trading-name")]
        public IActionResult SaveTradingName(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["TradingName"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.PublicContactEmail = profileSummaryViewModel.TradingName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PrimaryContact");
            }
            else
            {
                return View("PublicContactEmail", profileSummaryViewModel);
            }

        }
        #endregion


        /*#region Primary Contact

        [HttpGet("submit-profile/provider-primary-contact")]
        public IActionResult PrimaryContact(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("PrimaryContact", profileSummaryViewModel);
        }
        [HttpPost("submit-profile/service-providers-primary-contact")]
        public IActionResult SavePrimaryContact(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["PrimaryContact"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.PublicContactEmail = profileSummaryViewModel.TradingName;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("PrimaryContact");
            }
            else
            {
                return View("PrimaryContact", profileSummaryViewModel);
            }

        }
        #endregion

        #region Secondary Contact

        [HttpGet("submit-profile/provider-secondary-contact")]
        public IActionResult SecondaryContact(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("PrimaryContact", profileSummaryViewModel);
        }
        [HttpPost("submit-profile/service-providers-secondary-contact")]
        public IActionResult SaveSecondaryContact(ProfileSummaryViewModel profileSummaryViewModel)
        {
            bool fromSummaryPage = profileSummaryViewModel.FromSummaryPage;
            profileSummaryViewModel.FromSummaryPage = false;
            if (ModelState["SecondaryContact"].Errors.Count == 0)
            {
                ProfileSummaryViewModel profileSummary = GetProfileSummary();
                profileSummary.SecondaryContact = profileSummaryViewModel.SecondaryContact;
                HttpContext?.Session.Set("ProfileSummary", profileSummary);
                return fromSummaryPage ? RedirectToAction("ProfileSummary") : RedirectToAction("SecondaryContact");
            }
            else
            {
                return View("SecondaryContact", profileSummaryViewModel);
            }

        }
        #endregion*/

        //-- Last 5 screens 


        #region Public contact email

        [HttpGet("submit-profile/provider-public-contact-email")]
        public IActionResult PublicContactEmail(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("PublicContactEmail", profileSummaryViewModel);
        }
        [HttpPost("submit-profile/service-providers-public-contact-email")]
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
        [HttpGet("submit-profile/provider-telephone-number")]
        public IActionResult TelephoneNumber(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("TelephoneNumber", profileSummaryViewModel);
        }

        [HttpPost("submit-certificate-information/service-providers-telephone-number")]
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
        [HttpGet("submit-profile/service-providers-website-address")]
        public IActionResult WebsiteAddress(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View("WebsiteAddress", profileSummaryViewModel);
        }

        [HttpPost("submit-profile/service-providers-website-address")]
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

        [HttpGet("profile-summary")]
        public IActionResult ProfileSummary()
        {
            ProfileSummaryViewModel summaryViewModel = GetProfileSummary();
            return View(summaryViewModel);
        }
        [HttpPost("profile-summary")]
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
        [HttpGet("submit-certificate-information/information-submitted")]
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
