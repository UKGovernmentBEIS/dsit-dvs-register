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
        public async Task<IActionResult> StartPage()
        {
            HttpContext?.Session.Set("IsFirstVisit", true);
            return View("StartPage");
        }

        [HttpPost]
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
        [HttpGet]
        public IActionResult SelectApplicationSponsor()
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel);
        }

      
        [HttpPost]
        public IActionResult SaveApplicationSponsorSelection(SummaryViewModel viewModel)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.IsApplicationSponsor = viewModel.IsApplicationSponsor;
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
            if (summaryViewModel.IsApplicationSponsor)
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

        /// <summary>
        /// Load - Sponsor details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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

        [HttpGet]
        public IActionResult Contact(bool fromSummaryPage)
        {
            // Retrieve form validation and first visit info from Session
            ViewBag.IsFirstVisit = HttpContext?.Session.GetString("IsFirstVisit");
            ContactViewModelValidator.RetrieveAndSetValidationParametersForContactModel(ViewBag, HttpContext);
            ViewBag.fromSummaryPage = fromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel.SponsorViewModel.ContactViewModel);
        }

        [HttpPost]
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

        [HttpPost]
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

        [HttpGet]
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

        [HttpPost]
        public IActionResult SaveCountry(CountryViewModel countryViewModel)
        {
            bool fromSummaryPage = countryViewModel.FromSummaryPage;
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            List<CountryDto> availableCountries = JsonConvert.DeserializeObject<List<CountryDto>>(HttpContext.Request.Form["AvailableCountries"]);            
            summaryViewModel.CountryViewModel.SelectedCountries = availableCountries.Where(c => countryViewModel.SelectedCountryIds.Contains(c.Id)).ToList();
            summaryViewModel.CountryViewModel.FromSummaryPage = false;
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
            return fromSummaryPage ? RedirectToAction("Summary") : RedirectToAction("Company");
        }

        [HttpGet]
        public IActionResult Company()
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            return View(summaryViewModel.CompanyViewModel);
        }

        [HttpPost]
        public IActionResult SaveCompany(CompanyViewModel companyViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            summaryViewModel.CompanyViewModel = companyViewModel;
            HttpContext?.Session.Set("PreRegistrationSummary", summaryViewModel);
            return RedirectToAction("Summary");
        }

        [HttpGet]
        public ActionResult ShowHideCountries(bool hideCountries)
        {
            return RedirectToAction("Summary", new { hideCountries = hideCountries });
        }

        [HttpGet]
        public IActionResult Summary(bool hideCountries, bool confirmAccuracy = true)
        {
            SummaryViewModel summaryViewModel = GetPreRegistrationSummary();
            ViewBag.HideCountries = hideCountries;
            ViewBag.ConfirmAccuracy = confirmAccuracy;
            return View(summaryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSummaryAndSubmit(SummaryViewModel summaryViewModel)
        {
            SummaryViewModel model = GetPreRegistrationSummary();
            model.ConfirmAccuracy = summaryViewModel.ConfirmAccuracy;
          
            if (model.ConfirmAccuracy)
            {               
                PreRegistrationDto preRegistrationDto = MapViewModelToDto(model);
                GenericResponse genericResponse =  await preRegistrationService.SavePreRegistration(preRegistrationDto);
                if(genericResponse.Success && genericResponse.EmailSent) 
                {
                    return RedirectToAction("ApplicationComplete");
                }
                else //TODO:Error views
                {
                    model.ErrorMessage = Constants.FailedMessage;
                    return RedirectToAction("Summary");
                }                
            }
            else
            {
                
                return RedirectToAction("Summary", new { confirmAccuracy = model.ConfirmAccuracy });
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationComplete()
        {
            return View("ApplicationComplete");
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
                IsApplicationSponsor = false,
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
           
            preRegistrationDto.IsApplicationSponsor = model.IsApplicationSponsor;
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
