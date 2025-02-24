using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("update-request")]
    public class DSITEdit2iController : Controller
    {

        [HttpGet("provider-changes")]
        public IActionResult ProviderChanges()
        {
           
            return View();
        }

        [HttpGet("provider-changes/approved")]

        public IActionResult ProviderChangesApproved()
        {
            return View();
        }

        [HttpGet("provider-changes/cancelled")]
        public IActionResult ProviderChangesCancelled()
        {
            return View();
        }

        [HttpGet("provider-changes/already-approved")]
        public IActionResult ProviderChangesAlreadyApproved()
        {
            return View();
        }

        [HttpGet("provider-changes/error")]
        public IActionResult ProviderChangesError()
        {
            return View();
        }

        [HttpGet("service-changes")]
        public IActionResult ServiceChanges()
        {
            return View();
        }


        [HttpGet("service-changes/approved")]
        public IActionResult ServiceChangesApproved()
        {
            return View();
        }


        [HttpGet("service-changes/cancelled")]
        public IActionResult ServiceChangesCancelled()
        {
            return View();
        }

        [HttpGet("service-changes/already-approved")]
        public IActionResult ServiceChangesAlreadyApproved()
        {
            return View();
        }

        [HttpGet("service-changes/error")]
        public IActionResult ServiceChangesError()
        {
            return View();
        }
    }
}
