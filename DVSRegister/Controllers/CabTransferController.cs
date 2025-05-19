using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/cab-transfer")]
    public class CabTransferController : Controller
    {
        [HttpGet("select-cab")]
        public IActionResult SelectConformityAssessmentBody()
        {
            return View();
        }

        [HttpPost("select-cab")]
        public IActionResult SelectConformityAssessmentBodyPost()
        {
            var selectedCab = Request.Form["SelectedCab"].ToString();
            if (string.IsNullOrEmpty(selectedCab))
            {
                ViewData["Error"] = "Select the CAB this service should be reassigned to";
                return View("SelectConformityAssessmentBody");
            }

            return RedirectToAction("NextStep");
        }

        [HttpGet("next-step")]
        public IActionResult NextStep()
        {
            return View("NextStep");
        }
    }
}