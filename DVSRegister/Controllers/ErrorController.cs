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

    }
}