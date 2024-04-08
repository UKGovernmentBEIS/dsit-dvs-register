using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.PreAssessment;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DVSRegister.Controllers
{
    [Route("pre-registration/start-your-application")]
    public class PreRegistrationController : Controller
    {
        private readonly ILogger<PreRegistrationController> logger;
        private readonly IPreRegistrationService preRegistrationService;

        public PreRegistrationController(ILogger<PreRegistrationController> logger, IPreRegistrationService preRegistrationService)
        {
            this.logger = logger;
            this.preRegistrationService = preRegistrationService;
            
        }

        /// <summary>
        /// Load - first screen
        /// </summary>
        /// <returns></returns>
        [HttpGet("what-we-will-need-from-you")]
        public async Task<IActionResult> StartPage()
        {          
            return View("StartPage");
        }

       /// <summary>
       /// Redirects to Select application sponsor page 
       /// on continue button click
       /// </summary>
       /// <returns></returns>
        [HttpPost("what-we-will-need-from-you")]
        public IActionResult Continue()
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
            return RedirectToAction("SelectApplicationSponsor");
        }

        /// <summary>
        /// Select Application Sponsor or Not
        /// </summary>
        /// <returns></returns>
        [HttpGet("who-sponsors-this-application")]
        public IActionResult SelectApplicationSponsor()
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        /// Updates IsApplicationSponsor variable in session
        /// and redirect based on IsApplicationSponsor
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("who-sponsors-this-application")]
        public IActionResult SaveApplicationSponsorSelection(SummaryViewModel viewModel)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.IsApplicationSponsor = viewModel.IsApplicationSponsor;
            //Need to differentiate as the ConfirmAccuracy boolean field for last screen is also present in same view model
            if (ModelState["IsApplicationSponsor"].Errors.Count == 0)
            {              
                HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
                if (Convert.ToBoolean(summaryViewModel.IsApplicationSponsor))
                {
                    //Provide Contact details
                    return RedirectToAction("Contact");
                }
                else
                {
                    //Provider sponser contact and the user's contact
                    return RedirectToAction("Sponsor");
                } 
            }
            return View("SelectApplicationSponsor", viewModel);
        }

        /// <summary>
        /// Sponsor details
        /// </summary>
        /// <returns></returns>
        [HttpGet("provide-application-sponsor-details")]
        public IActionResult Sponsor(bool fromSummaryPage)
        {
            // Retrieve form validation and first visit info from Session
            ViewBag.IsFirstVisit = HttpContext?.Session.GetString("IsFirstVisit");

            ContactViewModelValidator.RetrieveAndSetValidationParametersForContactModel(ViewBag, HttpContext);

            ContactViewModelValidator.RetrieveAndSetValidationParametersForSponsorModel(ViewBag, HttpContext);
            ViewBag.fromSummaryPage = fromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel.SponsorViewModel);
        }

        /// <summary>
        ///Contact details of the user
        /// </summary>
        /// <param name="fromSummaryPage"></param>
        /// <returns></returns>

        [HttpGet("provide-your-contact-details")]
        public IActionResult Contact(bool fromSummaryPage)
        {
            // Retrieve form validation and first visit info from Session
            ViewBag.IsFirstVisit = HttpContext?.Session.GetString("IsFirstVisit");
            ContactViewModelValidator.RetrieveAndSetValidationParametersForContactModel(ViewBag, HttpContext);
            ViewBag.fromSummaryPage = fromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel.SponsorViewModel.ContactViewModel);
        }

        /// <summary>
        /// Save Sponsor details to session
        /// </summary>
        /// <param name="sponsorViewModel"></param>
        /// <returns></returns>

        [HttpPost("provide-application-sponsor-details")]
        public IActionResult SaveSponsor(SponsorViewModel sponsorViewModel)
        {
            bool fromSummaryPage = sponsorViewModel.FromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.SponsorViewModel = sponsorViewModel;
            summaryViewModel.SponsorViewModel.FromSummaryPage = false;
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);

            ContactViewModelValidator.ValidateSponsorViewModel(HttpContext, sponsorViewModel);
            ContactViewModelValidator.ValidateContactViewModel(HttpContext, sponsorViewModel.ContactViewModel);

            if (string.IsNullOrEmpty(HttpContext?.Session.GetString("IsFirstVisit")))
            {
                HttpContext?.Session.SetString("IsFirstVisit", "true");
            }
            else
            {
                HttpContext?.Session.SetString("IsFirstVisit", "false");
            }

            if (ContactViewModelValidator.IsContactModelValid(HttpContext) && ContactViewModelValidator.IsSponsorModelValid(HttpContext))
            {
                return RedirectToAction("Sponsor");
            }

            return fromSummaryPage ? RedirectToAction("Summary") : RedirectToAction("Country");
        }


        /// <summary>
        /// Save sponsor and contact detailsto session
        /// </summary>
        /// <param name="contactViewModel"></param>
        /// <returns></returns>
        [HttpPost("provide-your-contact-details")]
        public IActionResult SaveContact(ContactViewModel contactViewModel)
        {
            bool fromSummaryPage = contactViewModel.FromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.SponsorViewModel.ContactViewModel = contactViewModel;
            summaryViewModel.SponsorViewModel.ContactViewModel.FromSummaryPage = false;
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);

            ContactViewModelValidator.ValidateContactViewModel(HttpContext, contactViewModel);
            
            if (string.IsNullOrEmpty(HttpContext?.Session.GetString("IsFirstVisit")))
            {
                HttpContext?.Session.SetString("IsFirstVisit", "true");
            }
            else
            {
                HttpContext?.Session.SetString("IsFirstVisit", "false");
            }

            if (ContactViewModelValidator.IsContactModelValid(HttpContext))
            {
                return RedirectToAction("Contact");
            }

            return fromSummaryPage? RedirectToAction("Summary") : RedirectToAction("Country");
        }

        /// <summary>
        /// Load country list 
        /// (fetched from database)
        /// </summary>
        /// <param name="fromSummaryPage"></param>
        /// <returns></returns>

        [HttpGet("countries-and-territories-your-company-currently-trades-in")]
        public async Task <IActionResult> Country(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            CountryViewModel countryViewModel = new CountryViewModel();
            countryViewModel.IsApplicationSponsor = summaryViewModel.IsApplicationSponsor;
            countryViewModel.SelectedCountryIds = summaryViewModel.CountryViewModel.SelectedCountries.Select(c => c.Id).ToList();
            countryViewModel.AvailableCountries = await preRegistrationService.GetCountries();
            return View(countryViewModel);
        }

        /// <summary>
        /// Save selected countries to session
        /// </summary>
        /// <param name="countryViewModel"></param>
        /// <returns></returns>
        [HttpPost("countries-and-territories-your-company-currently-trades-in")]
        public async Task<IActionResult> SaveCountry(CountryViewModel countryViewModel)
        {
            bool fromSummaryPage = countryViewModel.FromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            List<CountryDto> availableCountries = JsonConvert.DeserializeObject<List<CountryDto>>(HttpContext.Request.Form["AvailableCountries"]);
            if(availableCountries == null  || availableCountries.Count == 0)
             availableCountries = await preRegistrationService.GetCountries();
            countryViewModel.AvailableCountries = availableCountries;
            countryViewModel.SelectedCountryIds =  countryViewModel.SelectedCountryIds??new List<int>();
            if (countryViewModel.SelectedCountryIds.Count > 0 )
            summaryViewModel.CountryViewModel.SelectedCountries = availableCountries.Where(c => countryViewModel.SelectedCountryIds.Contains(c.Id)).ToList();
            summaryViewModel.CountryViewModel.FromSummaryPage = false;
            if (ModelState.IsValid)
            {              
                HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("Summary") : RedirectToAction("Company"); 
            }
            else
            {
                return View("Country", countryViewModel);
            }
        }

        /// <summary>
        /// Comapny details
        /// </summary>
        /// <returns></returns>

        [HttpGet("your-company-details")]
        public IActionResult Company()
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel.CompanyViewModel);
        }

        /// <summary>
        /// Save company details to session
        /// </summary>
        /// <param name="companyViewModel"></param>
        /// <returns></returns>
        [HttpPost("your-company-details")]
        public IActionResult SaveCompany(CompanyViewModel companyViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.CompanyViewModel = companyViewModel;

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
                return RedirectToAction("Summary"); 
            }
            else
            {
                return View("Company", companyViewModel);
            }
        }

        /// <summary>
        /// Method to reload summary page
        /// based on Show All Hide All 
        /// link click
        /// </summary>
        /// <param name="hideCountries"></param>
        /// <returns></returns>
        [HttpGet("show-hide-countries")]
        public ActionResult ShowHideCountries(bool hideCountries)
        {
            return RedirectToAction("Summary", new { hideCountries = hideCountries });
        }

        /// <summary>
        /// Summary page displaying data saved in session
        /// </summary>
        /// <param name="hideCountries"></param>
        /// <param name="confirmAccuracy"></param>
        /// <returns></returns>
        [HttpGet("check-your-answers")]
        public IActionResult Summary(bool hideCountries, bool confirmAccuracy = true)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            ViewBag.HideCountries = hideCountries;
            ViewBag.ConfirmAccuracy = confirmAccuracy;
            return View(summaryViewModel);
        }

        /// <summary>
        /// Get data from session, convert to dto and save
        /// to database on Confirm click
        /// </summary>
        /// <param name="summaryViewModel"></param>
        /// <returns></returns>

        [HttpPost("check-your-answers")]
        public async Task<IActionResult> SaveSummaryAndSubmit(SummaryViewModel summaryViewModel)
        {
            SummaryViewModel model = GetPreRegistrationSummary();
            summaryViewModel.SponsorViewModel = model.SponsorViewModel;
            summaryViewModel.CompanyViewModel = model.CompanyViewModel;
            summaryViewModel.CountryViewModel = model.CountryViewModel;
            summaryViewModel.IsApplicationSponsor = model.IsApplicationSponsor;

            if (ModelState.IsValid)
            {               
                PreRegistrationDto preRegistrationDto = MapViewModelToDto(summaryViewModel);
                GenericResponse genericResponse =  await preRegistrationService.SavePreRegistration(preRegistrationDto);
                if(genericResponse.Success && genericResponse.EmailSent) 
                {
                    return RedirectToAction("ApplicationComplete");
                }
                else
                {                   
                    return RedirectToAction("HandleException","Error");
                }                
            }
            else
            {

                return View("Summary", summaryViewModel);
            }
            
        }

        /// <summary>
        /// Success screen
        /// </summary>
        /// <returns></returns>

        [HttpGet("application-submitted")]
        public async Task<IActionResult> ApplicationComplete()
        {
            SummaryViewModel model = GetPreRegistrationSummary();
            HttpContext?.Session.Clear();
            return View(model.SponsorViewModel);
        }
        #region

        /// <summary>
        /// Get viewmodels from session
        /// initialize if null
        /// </summary>
        /// <returns></returns>
        private SummaryViewModel GetPreRegistrationSummary()
        {
            SummaryViewModel model = HttpContext?.Session.Get<SummaryViewModel>("PreRegistrationSummary") ?? new SummaryViewModel
            {               
                SponsorViewModel = new SponsorViewModel { ContactViewModel = new ContactViewModel()},
                CompanyViewModel = new CompanyViewModel(),
                CountryViewModel = new CountryViewModel { SelectedCountries = new List<CountryDto>()},               
                ConfirmAccuracy = false
            };
            return model;
        }

        /// <summary>
        /// Map View model to Dto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private PreRegistrationDto MapViewModelToDto(SummaryViewModel model)
        {
            PreRegistrationDto preRegistrationDto = new PreRegistrationDto();  
            ICollection<PreRegistrationCountryMappingDto> preRegistrationCountryMappings = new List<PreRegistrationCountryMappingDto>();
            foreach (var item in model.CountryViewModel.SelectedCountries)
            {
                preRegistrationCountryMappings.Add(new PreRegistrationCountryMappingDto { CountryId = item.Id });
            }
           
            preRegistrationDto.IsApplicationSponsor =  Convert.ToBoolean(model.IsApplicationSponsor);
            preRegistrationDto.FullName =  model.SponsorViewModel.ContactViewModel.FullName;
            preRegistrationDto.JobTitle = model.SponsorViewModel.ContactViewModel.JobTitle;
            preRegistrationDto.Email = model.SponsorViewModel.ContactViewModel.Email;
            preRegistrationDto.TelephoneNumber = model.SponsorViewModel.ContactViewModel.TelephoneNumber;
            preRegistrationDto.SponsorFullName = model.SponsorViewModel.SponsorFullName;
            preRegistrationDto.SponsorJobTitle = model.SponsorViewModel.SponsorJobTitle;
            preRegistrationDto.SponsorEmail = model.SponsorViewModel.SponsorEmail;
            preRegistrationDto.SponsorTelephoneNumber = model.SponsorViewModel.SponsorTelephoneNumber;
            preRegistrationDto.PreRegistrationCountryMappings = preRegistrationCountryMappings;
            preRegistrationDto.RegisteredCompanyName = model.CompanyViewModel.RegisteredCompanyName;
            preRegistrationDto.TradingName = model.CompanyViewModel.TradingName;
            preRegistrationDto.CompanyRegistrationNumber =model.CompanyViewModel.CompanyRegistrationNumber;
            preRegistrationDto.HasParentCompany = model.CompanyViewModel.HasParentCompany == null? false :Convert.ToBoolean(model.CompanyViewModel.HasParentCompany);
            preRegistrationDto.ParentCompanyRegisteredName =  model.CompanyViewModel.ParentCompanyRegisteredName;
            preRegistrationDto.ParentCompanyLocation = model.CompanyViewModel.ParentCompanyLocation;
            preRegistrationDto.ConfirmAccuracy = YesNoEnum.Yes;
            preRegistrationDto.PreRegistrationStatus = PreRegistrationStatusEnum.Received;
            return preRegistrationDto;

        }
        #endregion
    }
}
