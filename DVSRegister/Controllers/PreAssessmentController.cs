using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.PreAssessment;
using DVSRegister.Extensions;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DVSRegister.Controllers
{
    public class PreAssessmentController : Controller
    {
        private readonly ILogger<PreAssessmentController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PreAssessmentController(ILogger<PreAssessmentController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Load - first screen
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> StartPage()
        {
            return View("StartPage");
        }

        [HttpPost]
        public IActionResult Continue()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
            return RedirectToAction("SelectApplicationSponsor");
        }

        /// <summary>
        /// Select Application Sponsor or Not
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SelectApplicationSponsor()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            return View(summaryViewModel);
        }

      
        [HttpPost]
        public IActionResult SaveApplicationSponsorSelection(SummaryViewModel viewModel)
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            summaryViewModel.IsApplicationSponsor = viewModel.IsApplicationSponsor;
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
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
        public IActionResult Sponsor()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            return View(summaryViewModel.SponsorViewModel);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            return View(summaryViewModel.SponsorViewModel.ContactViewModel);
        }

        [HttpPost]
        public IActionResult SaveSponsor(SponsorViewModel sponsorViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            summaryViewModel.SponsorViewModel = sponsorViewModel;
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
            return RedirectToAction("Country");
        }

        [HttpPost]
        public IActionResult SaveContact(ContactViewModel contactViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            summaryViewModel.SponsorViewModel.ContactViewModel = contactViewModel;
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
            return RedirectToAction("Country");
        }

        [HttpGet]
        public IActionResult Country()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            CountryViewModel countryViewModel = new CountryViewModel();
            countryViewModel.SelectedCountryIds = summaryViewModel.SelectedCountries.Select(c => c.Id).ToList();
            countryViewModel.AvailableCountries = new List<CountryDto>  //ToDo: Change to db call
            { new CountryDto { Id = 1, CountryName = "Abu Dhabi" },
            new CountryDto { Id = 25, CountryName = "Belgium" },
            new CountryDto { Id = 52, CountryName = "China" } };

            return View(countryViewModel);
        }

        [HttpPost]
        public IActionResult SaveCountry(CountryViewModel countryViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            List<CountryDto> availableCountries = JsonConvert.DeserializeObject<List<CountryDto>>(HttpContext.Request.Form["AvailableCountries"]);
            summaryViewModel.SelectedCountries = availableCountries
            .Where(c => countryViewModel.SelectedCountryIds.Contains(c.Id)).ToList();
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
            return RedirectToAction("Company");
        }

        [HttpGet]
        public IActionResult Company()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            return View(summaryViewModel.CompanyViewModel);
        }

        [HttpPost]
        public IActionResult SaveCompany(CompanyViewModel companyViewModel)
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            summaryViewModel.CompanyViewModel = companyViewModel;
            _httpContextAccessor?.HttpContext?.Session.Set("PreAssessmentSummary", summaryViewModel);
            return RedirectToAction("Summary");
        }

        [HttpGet]
        public IActionResult Summary()
        {
            SummaryViewModel summaryViewModel = GetPreAssessmentSummary();
            return View(summaryViewModel);
        }

        [HttpPost]
        public IActionResult SaveSummaryAndSubmit(SummaryViewModel summaryViewModel)
        {
            SummaryViewModel model = GetPreAssessmentSummary();
            model.ConfirmAccuracy =summaryViewModel.ConfirmAccuracy;
            MapViewModelToDto(model);
            //ToDo: Call Service to save to DB 
            return RedirectToAction("ApplicationComplete");
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
        private SummaryViewModel GetPreAssessmentSummary()
        {
            SummaryViewModel model = _httpContextAccessor?.HttpContext?.Session.Get<SummaryViewModel>("PreAssessmentSummary") ?? new SummaryViewModel
            {
                IsApplicationSponsor = false,
                SponsorViewModel = new SponsorViewModel { ContactViewModel = new ContactViewModel() },
                CompanyViewModel = new CompanyViewModel(),
                CountryViewModel = new CountryViewModel(),
                ConfirmAccuracy = false,
                SelectedCountries = new List<CountryDto>()
            };
            return model;
        }

        /// <summary>
        /// Map View model to Dto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private PreAssessmentDto MapViewModelToDto(SummaryViewModel model)
        {
            PreAssessmentDto preAssessmentDto = new PreAssessmentDto();
            //ToDo: Map fields
            return preAssessmentDto;
        }
        #endregion
    }
}
