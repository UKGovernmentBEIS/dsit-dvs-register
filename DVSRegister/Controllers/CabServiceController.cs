using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;


namespace DVSRegister.Controllers
{
    [Route("cab-service")]
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

        [HttpGet("list-providers")]
        public IActionResult ListProviders()
        {
            return View();
        }

        [HttpGet("provider-overview")]
        public IActionResult ProviderOverview()
        {
            return View();
        }

        [HttpGet("provider-profile-details")]
        public IActionResult ProviderProfileDetails()
        {
            return View();
        }

        [HttpGet("provider-service-details")]
        public IActionResult ProviderServiceDetails()
        {
            return View();
        }
    }
}