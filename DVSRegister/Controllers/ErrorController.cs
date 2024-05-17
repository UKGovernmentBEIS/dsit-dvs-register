using DVSRegister.Models.Error;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    [Route("")]
    public class ErrorController : Controller
    {
        [HttpGet("pre-registration/service-error")]
        public IActionResult HandleException()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel { ApplicationType = "pre-reg" };
            HttpContext?.Session.Clear();
            return View("ServiceIssue", errorViewModel);
        }

        [HttpGet("cab-registration/service-error")]
        public IActionResult CabHandleException()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel { ApplicationType = "cab" };
            HttpContext?.Session.Clear();
            return View("ServiceIssue", errorViewModel);
        }

        [HttpGet("service-error")]
        public IActionResult ServiceError()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel { ApplicationType = string.Empty };
            HttpContext?.Session.Clear();
            return View("ServiceIssue", errorViewModel);
        }
    }
}
