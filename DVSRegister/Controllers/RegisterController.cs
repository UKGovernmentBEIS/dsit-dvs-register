using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace DVSRegister.Controllers
{    
    [Route("register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> logger;     
        private readonly IRegisterService registerService;
        private readonly ICabService cabService;
//        private readonly GoogleAnalyticsService googleAnalyticsService;


        public RegisterController(ILogger<RegisterController> logger, IRegisterService registerService, ICabService cabService)
        {
            this.logger = logger;          
            this.registerService = registerService;
            this.cabService = cabService;
//            this.googleAnalyticsService = googleAnalyticsService;


        }
        [Route("")]
        [HttpGet("register-search")]
        public async Task<IActionResult> Register(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds, bool FromDeatilsPage = false, int RemoveRole = 0, int RemoveScheme = 0, string SearchAction = "", string SearchProvider = "")
        {
            RegisterListViewModel registerListViewModel = new RegisterListViewModel();
            if (FromDeatilsPage)
            {
                Filters filters = HttpContext?.Session.Get<Filters>("Filters")??new Filters();
                SelectedRoleIds =  filters.SelectedRoleIds;
                SelectedSupplementarySchemeIds= filters.SelectedSupplementarySchemeIds;
                SearchAction =  filters.SearchAction;
                SearchProvider = filters.SearchProvider;
                FromDeatilsPage = filters.FromDeatilsPage;
                RemoveRole = filters.RemoveRole;
                RemoveScheme = filters.RemoveScheme;

            }
            SearchAction = InputSanitizeExtensions.CleanseInput(SearchAction);
            SearchProvider = InputSanitizeExtensions.CleanseInput(SearchProvider);

            await SetRoles(SelectedRoleIds, RemoveRole, registerListViewModel);
            await SetSchemes(SelectedSupplementarySchemeIds, RemoveScheme, registerListViewModel);

            if (SearchAction=="clearSearch")
            {
                ModelState.Clear();
                registerListViewModel.SearchProvider = null;
                SearchProvider = string.Empty;
            }
            else if (SearchAction == "clearFilter")
            {
                ModelState.Clear();
                registerListViewModel.SelectedRoleIds = new List<int>();
                registerListViewModel.SelectedSupplementarySchemeIds = new List<int>();
                registerListViewModel.SelectedRoles = new List<RoleDto> { new RoleDto() };
                registerListViewModel.SelectedSupplementarySchemes =   new List<SupplementarySchemeDto> { new SupplementarySchemeDto() };
            }
            registerListViewModel.Providers = await registerService.GetProviders(registerListViewModel.SelectedRoleIds, registerListViewModel.SelectedSupplementarySchemeIds, SearchProvider);
            List<RegisterPublishLogDto> list = await registerService.GetRegisterPublishLogs();
            if (list!=null && list.Any())
            {
                registerListViewModel.LastUpdated =  list[0].CreatedTime.ToString("dd MMMM yyyy");
                TempData["LastUpdated"] =   registerListViewModel.LastUpdated;
            }

            SetFiltersInSession(SelectedRoleIds, SelectedSupplementarySchemeIds, FromDeatilsPage, RemoveRole, RemoveScheme, SearchAction, SearchProvider);
            return View("Register", registerListViewModel);
        }


        [HttpGet("provider-details")]
        public async Task<IActionResult> ProviderDetails(int providerId)
        {

            ProviderProfileDto providerProfileDto = await registerService.GetProviderWithServiceDeatils(providerId);
            providerProfileDto.Services = AssignServiceNumber(providerProfileDto.Services);
            ProviderDetailsViewModel providerDetailsViewModel = new ProviderDetailsViewModel();
            providerDetailsViewModel.Provider = providerProfileDto;
            providerDetailsViewModel.LastUpdated = TempData.Peek("LastUpdated") as string ?? string.Empty;
            return View(providerDetailsViewModel);

            //ProviderDto providerDto = await registerService.GetProviderWithServiceDeatils(providerId);
            //providerDto.CertificateInformation = AssignServiceNumber(providerDto.CertificateInformation);
            //ProviderDetailsViewModel providerDetailsViewModel = new ProviderDetailsViewModel();
            //providerDetailsViewModel.Provider = providerDto;
            //providerDetailsViewModel.LastUpdated = TempData.Peek("LastUpdated") as string??string.Empty;
            //return View(providerDetailsViewModel);
        }


        [HttpGet("publish-logs")]
        public async Task<IActionResult> Updates()
        {
            RegisterPublishLogsViewModel registerPublishLogsViewModel = new RegisterPublishLogsViewModel();
            registerPublishLogsViewModel.RegisterPublishLog = await registerService.GetRegisterPublishLogs();
            return View("Updates", registerPublishLogsViewModel);
        }

        #region Private methods
        private void SetFiltersInSession(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds, bool FromDeatilsPage, int RemoveRole, int RemoveScheme, string SearchAction, string SearchProvider)
        {
            Filters filters = new Filters();
            filters.SelectedRoleIds = SelectedRoleIds;
            filters.SelectedSupplementarySchemeIds= SelectedSupplementarySchemeIds;
            filters.SearchAction = SearchAction;
            filters.SearchProvider = SearchProvider;
            filters.FromDeatilsPage = FromDeatilsPage;
            filters.RemoveRole = RemoveRole;
            filters.RemoveScheme = RemoveScheme;
            HttpContext?.Session.Set("Filters", filters);
        }
        private async Task SetSchemes(List<int> SelectedSupplementarySchemeIds, int RemoveScheme, RegisterListViewModel registerListViewModel)
        {
            registerListViewModel.AvailableSchemes =  await cabService.GetSupplementarySchemes();

            if (SelectedSupplementarySchemeIds==null || !SelectedSupplementarySchemeIds.Any())
            {
                registerListViewModel.SelectedSupplementarySchemeIds = new List<int>();
            }
            else
            {
                if (RemoveScheme>0)
                    SelectedSupplementarySchemeIds.Remove(RemoveScheme);
                registerListViewModel.SelectedSupplementarySchemeIds = SelectedSupplementarySchemeIds;
            }

            if (registerListViewModel.SelectedSupplementarySchemeIds.Count > 0)
                registerListViewModel.SelectedSupplementarySchemes =  registerListViewModel.AvailableSchemes.Where(c => registerListViewModel.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
        }

        private async Task SetRoles(List<int> SelectedRoleIds, int RemoveRole, RegisterListViewModel registerListViewModel)
        {
            registerListViewModel.AvailableRoles = await cabService.GetRoles();
            if (SelectedRoleIds==null || !SelectedRoleIds.Any())
            {
                registerListViewModel.SelectedRoleIds = new List<int>();
            }
            else
            {
                if (RemoveRole>0)
                    SelectedRoleIds.Remove(RemoveRole);
                registerListViewModel.SelectedRoleIds = SelectedRoleIds;
            }
            if (registerListViewModel?.SelectedRoleIds?.Count > 0)
                registerListViewModel.SelectedRoles =  registerListViewModel.AvailableRoles.Where(c => registerListViewModel.SelectedRoleIds.Contains(c.Id)).ToList();
        }

        private List<ServiceDto> AssignServiceNumber(ICollection<ServiceDto> serviceDtos)
        {
            List<ServiceDto> serviceDtosFiltered = serviceDtos
          .Where(x => x.ServiceStatus == ServiceStatusEnum.ReadyToPublish
          || x.ServiceStatus == ServiceStatusEnum.Published).ToList();
            serviceDtosFiltered.Select((item, index) => new { item, index })
            .ToList().ForEach(x => x.item.ServiceNumber = x.index + 1);
            return serviceDtosFiltered;

           // List<CertificateInfoDto> certificateInformationDtosFiltered = certificateInformationDtos
           //.Where(x => x.CertificateInfoStatus == CertificateInfoStatusEnum.ReadyToPublish
           //|| x.CertificateInfoStatus == CertificateInfoStatusEnum.Published).ToList();
           // certificateInformationDtosFiltered.Select((item, index) => new { item, index })
           // .ToList().ForEach(x => x.item.ServiceNumber = x.index + 1);
           // return certificateInformationDtosFiltered;
        }


        #endregion
    }
}
