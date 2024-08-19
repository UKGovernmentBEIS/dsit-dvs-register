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
    [Route("cab-service/submit-service")]
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

        [HttpGet("name-of-service")]
        public IActionResult ServiceName()
        {

            return View();
        }

        [HttpGet("service-url")]
        public IActionResult ServiceURL()
        {

            return View();
        }

        [HttpGet("company-address")]
        public IActionResult CompanyAddress()
        {

            return View();
        }

        [HttpGet("provider-roles")]
        public IActionResult ProviderRoles()
        {

            return View();
        }

        [HttpGet("gpg44")]
        public IActionResult GPG44()
        {

            return View();
        }

        [HttpGet("gpg45")]
        public IActionResult GPG45()
        {

            return View();
        }
    }
}