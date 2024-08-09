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
    [Route("cab-provider")]
    //[ValidCognitoToken]//
    public class CabProviderController : Controller
    {

        private readonly ICabService cabService;


        public CabProviderController(ICabService cabService)
        {
            this.cabService = cabService;
        }

        [HttpGet("before-you-start")]
        public IActionResult BeforeYouStart()
        {
            
            return View();
        }

    }
}
