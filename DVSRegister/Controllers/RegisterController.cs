using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Data.Entities;
using DVSRegister.Extensions;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DVSRegister.Controllers
{
    [Route("")]
    [Route("register")]
    public class RegisterController(IRegisterService registerService, ICabService cabService) : Controller
    {         
        private readonly IRegisterService registerService = registerService;
        private readonly ICabService cabService = cabService;
        private readonly decimal TFVersionNumber = 0.3m;//To Do : update after all tf changes 

        [Route("")]
        [HttpGet("register-search")]
        public async Task<IActionResult> Register(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds, int RemoveRole = 0, int RemoveScheme = 0, string SearchAction = "", string SearchProvider = "")
        {
            RegisterListViewModel registerListViewModel = new ();
            Filters? filters = HttpContext?.Session.Get<Filters>("Filters");
            if (SelectedRoleIds!= null && SelectedRoleIds.Count>0 && SelectedSupplementarySchemeIds!=null && SelectedSupplementarySchemeIds.Count>0 &&
               string.IsNullOrEmpty(SearchAction) && string.IsNullOrEmpty(SearchProvider) &&RemoveRole == 0 && RemoveScheme == 0 && filters != null)
            {
                SelectedRoleIds = filters.SelectedRoleIds;
                SelectedSupplementarySchemeIds = filters.SelectedSupplementarySchemeIds;
                SearchAction = filters.SearchAction;
                SearchProvider = filters.SearchProvider;                
                RemoveRole = filters.RemoveRole;
                RemoveScheme = filters.RemoveScheme;
            }

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
                registerListViewModel.SelectedRoleIds = [];
                registerListViewModel.SelectedSupplementarySchemeIds = [];
                registerListViewModel.SelectedRoles = [new RoleDto()];
                registerListViewModel.SelectedSupplementarySchemes =   [new SupplementarySchemeDto()];
            }
            registerListViewModel.Providers = await registerService.GetProviders(registerListViewModel.SelectedRoleIds, registerListViewModel.SelectedSupplementarySchemeIds, SearchProvider);
            List<RegisterPublishLogDto> list = await registerService.GetRegisterPublishLogs();
            if (list!=null && list.Count != 0)
            {
                registerListViewModel.LastUpdated =  list[0].CreatedTime.ToString("dd MMMM yyyy");
            }

            SetFiltersInSession(SelectedRoleIds, SelectedSupplementarySchemeIds, RemoveRole, RemoveScheme, SearchAction, SearchProvider);
            return View("Register", registerListViewModel);
        }


        [HttpGet("provider-details")]
        public async Task<IActionResult> ProviderDetails(int providerId)
        {

            ProviderProfileDto providerProfileDto = await registerService.GetProviderWithServiceDeatils(providerId);
            ProviderDetailsViewModel providerDetailsViewModel = new()
            {
                Provider = providerProfileDto,
            };
            return View(providerDetailsViewModel);
        }

        [HttpGet("underpinning-service-details")]
        public async Task<IActionResult> UnderpinningServiceDetails(int serviceId, int previousProviderId)
        {
            ViewBag.PreviousProviderId = previousProviderId;
            ServiceDto service = await registerService.GetServiceDetails(serviceId);
            return View(service);
        }

        [HttpGet("publish-logs")]
        public async Task<IActionResult> Updates()
        {
            RegisterPublishLogsViewModel registerPublishLogsViewModel = new RegisterPublishLogsViewModel();
            registerPublishLogsViewModel.RegisterPublishLog = await registerService.GetRegisterPublishLogs();
            return View("Updates", registerPublishLogsViewModel);
        }

        #region Private methods
        private void SetFiltersInSession(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds,  int RemoveRole, int RemoveScheme, string SearchAction, string SearchProvider)
        {
            Filters filters = new()
            {
                SelectedRoleIds = SelectedRoleIds,
                SelectedSupplementarySchemeIds = SelectedSupplementarySchemeIds,
                SearchAction = SearchAction,
                SearchProvider = SearchProvider,
                RemoveRole = RemoveRole,
                RemoveScheme = RemoveScheme
            };
            HttpContext?.Session.Set("Filters", filters);
        }
        private async Task SetSchemes(List<int>? SelectedSupplementarySchemeIds, int RemoveScheme, RegisterListViewModel registerListViewModel)
        {
            registerListViewModel.AvailableSchemes =  await cabService.GetSupplementarySchemes();

            if (SelectedSupplementarySchemeIds==null || SelectedSupplementarySchemeIds.Count == 0)
            {
                registerListViewModel.SelectedSupplementarySchemeIds = [];
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

        private async Task SetRoles(List<int>? SelectedRoleIds, int RemoveRole, RegisterListViewModel registerListViewModel)
        {
            registerListViewModel.AvailableRoles = await cabService.GetRoles(TFVersionNumber);
            if (SelectedRoleIds==null || SelectedRoleIds.Count == 0)
            {
                registerListViewModel.SelectedRoleIds = [];
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

       


        #endregion
    }
}