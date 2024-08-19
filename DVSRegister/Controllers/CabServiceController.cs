using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service/submit-service")]
    //[ValidCognitoToken]//
    public class CabServiceController : Controller
    {

        private readonly ICabService cabService;

        public CabServiceController(ICabService cabService)
        {
            this.cabService = cabService;
        }

        [HttpGet("before-you-start")]
        public IActionResult BeforeYouStart()
        {

            return View();
        }

        [HttpGet("name-of-service")]
        public IActionResult ServiceName()
        {

            return View();
        }

        [HttpGet("service-url")]
        public IActionResult ServiceURL()
        {

            return View();
        }

        [HttpGet("company-address")]
        public IActionResult CompanyAddress()
        {

            return View();
        }

        [HttpGet("provider-roles")]
        public IActionResult ProviderRoles()
        {

            return View();
        }

        [HttpGet("gpg44")]
        public IActionResult GPG44()
        {

            return View();
        }

        [HttpGet("gpg45")]
        public IActionResult GPG45()
        {

            return View();
        }



        [HttpGet("enter-issue-date")]
        public IActionResult ConfirmityIssueDate(bool fromSummaryPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateViewModel dateViewModel = new DateViewModel();
            dateViewModel.PropertyName = "ConfirmityIssueDate";
            if (summaryViewModel.ConformityIssueDate != null)
            {
                dateViewModel = GetDayMonthYear(summaryViewModel.ConformityIssueDate);
            }

            return View(dateViewModel);
        }



        /// <summary>
        /// Updates confirmity issue date variable in session 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("enter-issue-date")]
        public IActionResult SaveConfirmityIssueDate(DateViewModel dateViewModel)
        {
            bool fromSummaryPage = dateViewModel.FromSummaryPage;
            dateViewModel.FromSummaryPage = false;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityIssueDate = ValidateIssueDate(dateViewModel);
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityIssueDate =conformityIssueDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return fromSummaryPage ? RedirectToAction("ServiceSummary") : RedirectToAction("ConfirmityExpiryDate");
            }
            else
            {
                return View("ConfirmityIssueDate", dateViewModel);
            }

        }

        [HttpGet("enter-expiry-date")]
        public IActionResult ConfirmityExpiryDate()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateViewModel dateViewModel = new DateViewModel();
            dateViewModel.PropertyName = "ConfirmityExpiryDate";
            if (summaryViewModel.ConformityExpiryDate != null)
            {
                dateViewModel = GetDayMonthYear(summaryViewModel.ConformityExpiryDate);
            }
            return View(dateViewModel);
        }

        /// <summary>
        /// Updates confirmity issue expiry date variable in session 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("enter-expiry-date")]
        public IActionResult SaveConfirmityExpiryDate(DateViewModel dateViewModel)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            DateTime? conformityExpiryDate = ValidateExpiryDate(dateViewModel, Convert.ToDateTime(summaryViewModel.ConformityIssueDate));
            if (ModelState.IsValid)
            {
                summaryViewModel.ConformityExpiryDate = conformityExpiryDate;
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return RedirectToAction(""); //ToDo: update after adding next views
            }
            else
            {
                return View("ConfirmityExpiryDate", dateViewModel);

            }
        }




        #region Private Methods

        private ServiceSummaryViewModel GetServiceSummary()
        {
            

            ServiceSummaryViewModel model = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel
            {
                QualityLevelViewModel = new QualityLevelViewModel { SelectedQualityLevels = new List<QualityLevelDto>() },
                RoleViewModel = new RoleViewModel { SelectedRoles = new List<RoleDto>() },
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = new List<IdentityProfileDto>() },
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } }
            };
            return model;
        }

        private DateViewModel GetDayMonthYear(DateTime? dateTime)
        {
            DateViewModel dateViewModel = new DateViewModel();
            DateTime conformityIssueDate = Convert.ToDateTime(dateTime);
            dateViewModel.Day = conformityIssueDate.Day;
            dateViewModel.Month = conformityIssueDate.Month;
            dateViewModel.Year = conformityIssueDate.Year;
            return dateViewModel;
        }

        private DateTime? ValidateIssueDate(DateViewModel dateViewModel)
        {
            DateTime? date = null;
            DateTime minDate = new DateTime(1900, 1, 1);
            try
            {
                if (dateViewModel.Day == null || dateViewModel.Month == null ||dateViewModel.Year == null)
                {
                    if (dateViewModel.Day == null)
                    {
                        ModelState.AddModelError("Day", Constants.ConformityIssueDayError);
                    }
                    if (dateViewModel.Month == null)
                    {
                        ModelState.AddModelError("Month", Constants.ConformityIssueMonthError);
                    }
                    if (dateViewModel.Year == null)
                    {
                        ModelState.AddModelError("Year", Constants.ConformityIssueYearError);
                    }
                }
                else
                {
                    date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                    if (date>DateTime.Today)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssuePastDateError);
                    }
                    if (date<minDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);
                    }
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);

            }
            return date;
        }

        private DateTime? ValidateExpiryDate(DateViewModel dateViewModel, DateTime issueDate)
        {
            DateTime? date = null;     
          
            try
            {
                if (dateViewModel.Day == null || dateViewModel.Month == null ||dateViewModel.Year == null)
                {
                    if (dateViewModel.Day == null)
                    {
                        ModelState.AddModelError("Day", Constants.ConformityExpiryDayError);
                    }
                    if (dateViewModel.Month == null)
                    {
                        ModelState.AddModelError("Month", Constants.ConformityExpiryMonthError);
                    }
                    if (dateViewModel.Year == null)
                    {
                        ModelState.AddModelError("Year", Constants.ConformityExpiryYearError);
                    }
                }
                else
                {
                    date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                    var maxExpiryDate = issueDate.AddYears(2);
                    if (date <= DateTime.Today)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityExpiryPastDateError);
                    }
                    else if (date <= issueDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityIssueDateExpiryDateError);
                    }
                    else if (date >= maxExpiryDate)
                    {
                        ModelState.AddModelError("ValidDate", Constants.ConformityMaxExpiryDateError);
                    }
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("ValidDate", Constants.ConformityExpiryDateInvalidError);

            }
            return date;
        }
        #endregion
    }
}