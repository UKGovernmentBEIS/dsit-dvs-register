using DVSRegister.BusinessLogic.Services;
using DVSRegister.Models;
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
        public async Task<IActionResult> Register()
        {           
            return View("Register");
        }

        [HttpGet("apply-filter")]
        public async Task<IActionResult> ApplyFilter(List<int> roles, List<int> schemes, string searchText= "" )
        {
           RegisterListViewModel registerListViewModel = new RegisterListViewModel();   
           registerListViewModel.Registers = await registerService.GetProviders(roles, schemes, searchText);
            return View("Register");
        }


        //Add method to fetch for Updates screen

    }
}
