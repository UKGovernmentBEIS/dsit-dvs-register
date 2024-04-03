using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
   
    public class ErrorController : Controller
    {      
        public IActionResult HandleException()
        {
            HttpContext?.Session.Clear();
            return View("ServiceIssue");
        }

    }
}
