using DVSRegister.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> logger;     
        private readonly IRegisterService registerService;

        public RegisterController(ILogger<RegisterController> logger, IRegisterService registerService)
        {
            this.logger = logger;          
            this.registerService = registerService;

        }
        [HttpGet("register-search")]
        public IActionResult Register()
        {
            //ToDO: call service GetProviders
            //registerService
            return View("Register");
        }


        //Add method to fetch for Updates screen

    }
}
