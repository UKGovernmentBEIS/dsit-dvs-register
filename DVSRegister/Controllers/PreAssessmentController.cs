using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    public class PreAssessmentController : Controller
    {
        private readonly ILogger<PreAssessmentController> _logger;
        public PreAssessmentController(ILogger<PreAssessmentController> logger)
        {
            _logger = logger;
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
            return RedirectToAction("Sponsor");
        }
        /// <summary>
        /// Load - Sponsor details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Sponsor(SponsorViewModel sponsorViewModel)
        {
            return View(sponsorViewModel);
        }

        [HttpPost]
        public IActionResult SaveSponsor(SponsorViewModel sponsorViewModel)
        {           
            return RedirectToAction("Country");
        }

        [HttpGet]
        public async Task<IActionResult> Country(CountryViewModel countryViewModel)
        {
            return View(countryViewModel);
        }

        [HttpPost]
        public IActionResult SaveCountry(CountryViewModel countryViewModel)
        {
            return RedirectToAction("Company");
        }

        [HttpGet]
        public async Task<IActionResult> Company(CompanyViewModel companyViewModel)
        {
            return View(companyViewModel);
        }

        [HttpPost]
        public IActionResult SaveCompany(CompanyViewModel companyViewModel)
        {
            return RedirectToAction("Summary");
        }

        [HttpGet]
        public async Task<IActionResult> Summary(SummaryViewModel summaryViewModel)
        {
            return View(summaryViewModel);
        }

        [HttpPost]
        public IActionResult SaveSummary(SummaryViewModel summaryViewModel)
        {
            return RedirectToAction("ApplicationComplete");
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationComplete()
        {
            return View("ApplicationComplete");
        }

        [HttpPost]
        public IActionResult SubmitApplication(SummaryViewModel summaryViewModel)
        {
            //Logic to convert to dto and save
            return View("ApplicationComplete");
        }
    }
}
