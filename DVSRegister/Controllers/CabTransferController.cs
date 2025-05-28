using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-transfer")]
    public class CabTransferController : Controller
    {


        [HttpGet("service-management-requests")]
        public IActionResult ServiceManagementRequests()
        {
            return View();
        }


        [HttpGet("reassignment-success")]
        public IActionResult ReAssignmentSuccess()
        {
            return View();
        }
    }
}
