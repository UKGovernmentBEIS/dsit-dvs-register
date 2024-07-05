using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> logger;     
        private readonly IRegisterService registerService;
        private readonly ICabService cabService;
       

        public RegisterController(ILogger<RegisterController> logger, IRegisterService registerService, ICabService cabService)
        {
            this.logger = logger;          
            this.registerService = registerService;
            this.cabService = cabService;
            

        }
        [HttpGet("register-search")]
        public async Task<IActionResult> Register(string searchText = "")
        {
            RegisterListViewModel registerListViewModel = new RegisterListViewModel();
            registerListViewModel.AvailableRoles =  HttpContext?.Session.Get<List<RoleDto>>("AvailableRoles")?? await cabService.GetRoles();
            registerListViewModel.SelectedRoleIds = new List<int>();
            registerListViewModel.AvailableSchemes =   HttpContext?.Session.Get<List<SupplementarySchemeDto>>("AvailableSchemes")?? await cabService.GetSupplementarySchemes();
            registerListViewModel.SelectedSupplementarySchemeIds = new List<int>();
            registerListViewModel.Providers = await registerService.GetProviders(registerListViewModel.SelectedRoleIds, registerListViewModel.SelectedSupplementarySchemeIds, searchText);
             List<RegisterPublishLogDto> list = await registerService.GetRegisterPublishLogs();
            if(list!=null && list.Any())
            {
                registerListViewModel.LastUpdated =  list[0].CreatedTime.ToString("dd MMMM yyyy");
            }
           
            return View("Register", registerListViewModel);
        }

        [HttpGet("apply-filter")]
        public async Task<IActionResult> ApplyFilter(List<int> roles, List<int> schemes, string searchText= "" )
        {
           RegisterListViewModel registerListViewModel = new RegisterListViewModel();   
           registerListViewModel.Providers = await registerService.GetProviders(roles, schemes, searchText);
            return View("Register");
        }

        [HttpGet("publish-logs")]
        public async Task<ActionResult<List<RegisterPublishLogDto>>> GetRegisterPublishLogs()
        {
            var logs = await registerService.GetRegisterPublishLogs();
            return Ok(logs);
        }
    }
}
