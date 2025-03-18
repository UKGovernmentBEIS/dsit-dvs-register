using DVSRegister.CommonUtility;
using DVSRegister.Models.Error;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("")]
    public class ErrorController : Controller
    {       
        private readonly ILogger<ErrorController> _logger;
        
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("cab-service/service-error")]
        public IActionResult CabHandleException()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Cab Service encountered an error."));

            ErrorViewModel errorViewModel = new ErrorViewModel { ApplicationType = "cab" };
            HttpContext?.Session.Clear();
            return View("ServiceIssue", errorViewModel);
        }

        [HttpGet("service-error")]
        public IActionResult ServiceError()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("A general service error occurred."));

            ErrorViewModel errorViewModel = new ErrorViewModel { ApplicationType = string.Empty };
            HttpContext?.Session.Clear();
            return View("ServiceIssue", errorViewModel);
        }
    }
}
