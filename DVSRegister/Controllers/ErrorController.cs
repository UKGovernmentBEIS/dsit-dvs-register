using DVSRegister.Models.Error;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("")]
    public class ErrorController : Controller
    {       
      

        [HttpGet("cab-service/service-error")]
        public IActionResult CabHandleException()
        {
            ErrorViewModel errorViewModel = new() { ApplicationType = "cab" };
            ClearHttpContextAndSetHttpStatus();
            return View("ServiceIssue", errorViewModel);
        }

        
        [HttpGet("service-error")]
        public IActionResult ServiceError()
        {   
            ErrorViewModel errorViewModel = new() { ApplicationType = string.Empty };
            ClearHttpContextAndSetHttpStatus();
            return View("ServiceIssue", errorViewModel);
        }

        private void ClearHttpContextAndSetHttpStatus()
        {
            if (HttpContext != null)
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Clear();
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        [HttpGet("cab-service/page-not-found")]
        public IActionResult CabServicePageNotFound()
        {   
            ViewData["Layout"] = "~/Views/Shared/_Layout_CAB.cshtml";
            return View("PageNotFound");
        }

        [HttpGet("register/page-not-found")]
        public IActionResult RegisterPageNotFound()
        {           
            ViewData["Layout"] = "~/Views/Shared/_Layout_Register.cshtml";
            return View("PageNotFound");
        }

    }
}