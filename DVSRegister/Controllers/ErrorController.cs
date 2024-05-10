using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    [Route("pre-registration/service-error")]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult HandleException()
        {
            HttpContext?.Session.Clear();
            return View("ServiceIssue");
        }
    }
}
