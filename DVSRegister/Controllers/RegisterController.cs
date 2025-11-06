using CsvHelper;
using CsvHelper.Configuration;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Models;
using DVSRegister.Models.Register;
using DVSRegister.Models.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;

namespace DVSRegister.Controllers
{
    [Route("")]
    [Route("register")]
    public class RegisterController(IRegisterService registerService, ICabService cabService) : Controller
    {         
        private readonly IRegisterService registerService = registerService;
        private readonly ICabService cabService = cabService;        
        private readonly decimal TFVersionNumber = 0.4m;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10);


        [Route("")]
        #region All services
        [HttpGet("all-services")]
        public async Task<IActionResult> AllServices(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds, List<int> SelectedTfVersionIds,
            int RemoveRole = 0, int RemoveScheme = 0, int RemoveTfVersion = 0, bool RemoveSort = false, string SearchAction = "", string SearchText = "", string SortBy = "", int PageNum = 1)
        {
            AllServicesViewModel allServicesViewModel = new();

            await SetRoles(SelectedRoleIds, RemoveRole, allServicesViewModel);
            await SetSchemes(SelectedSupplementarySchemeIds, RemoveScheme, allServicesViewModel);
            await SetTfVersion(SelectedTfVersionIds, RemoveTfVersion, allServicesViewModel);
            allServicesViewModel.SortBy = RemoveSort ? "" : SortBy;

            if (SearchAction == "clearFilter")
            {
                ModelState.Clear();
                allServicesViewModel.SortBy = "";
                allServicesViewModel.SelectedRoleIds = [];
                allServicesViewModel.SelectedSupplementarySchemeIds = [];
                allServicesViewModel.SelectedTrustFrameworkVersionId = [];
                allServicesViewModel.SelectedRoles = [new RoleDto()];
                allServicesViewModel.SelectedSupplementarySchemes = [new SupplementarySchemeDto()];
                allServicesViewModel.SelectedTrustFrameworkVersion = [new TrustFrameworkVersionDto()];
            }

            var results = await registerService.GetServices(allServicesViewModel.SelectedRoleIds, allServicesViewModel.SelectedSupplementarySchemeIds, allServicesViewModel.SelectedTrustFrameworkVersionId,
                PageNum, SearchText, allServicesViewModel.SortBy);            
            allServicesViewModel.PageNumber = PageNum;
            allServicesViewModel.Services = results.Items;
            allServicesViewModel.TotalResults = results.TotalCount;
            allServicesViewModel.TotalPages = (int)Math.Ceiling((double)results.TotalCount / 10);
            var lastUpdatedDate = await registerService.GetLastUpdatedDate();
            allServicesViewModel.LastUpdated = lastUpdatedDate?.ToString("dd MMMM yyyy");
            return View(allServicesViewModel);
        }
        #endregion

        #region All providers

        [HttpGet("all-providers")]
        public async Task<IActionResult> AllProviders(List<int> SelectedRoleIds, List<int> SelectedSupplementarySchemeIds, List<int> SelectedTfVersionIds,
            int RemoveRole = 0, int RemoveScheme = 0, int RemoveTfVersion = 0, bool RemoveSort = false, string SearchAction = "", string SearchText = "", string SortBy = "", int PageNum = 1)
        {
            AllProvidersViewModel allProvidersViewModel = new();
            await SetRoles(SelectedRoleIds, RemoveRole, allProvidersViewModel);
            await SetSchemes(SelectedSupplementarySchemeIds, RemoveScheme, allProvidersViewModel);
            await SetTfVersion(SelectedTfVersionIds, RemoveTfVersion, allProvidersViewModel);
            allProvidersViewModel.SortBy = RemoveSort ? "" : SortBy;

            if (SearchAction == "clearSearch")
            {
                ModelState.Clear();
                allProvidersViewModel.SearchText = null;
                SearchText = string.Empty;
            }
            else if (SearchAction == "clearFilter")
            {
                ModelState.Clear();
                allProvidersViewModel.SelectedRoleIds = [];
                allProvidersViewModel.SelectedSupplementarySchemeIds = [];
                allProvidersViewModel.SelectedTrustFrameworkVersionId = [];
                allProvidersViewModel.SelectedRoles = [new RoleDto()];
                allProvidersViewModel.SelectedSupplementarySchemes = [new SupplementarySchemeDto()];
                allProvidersViewModel.SelectedTrustFrameworkVersion = [new TrustFrameworkVersionDto()];
            }
            var results = await registerService.GetProviders(allProvidersViewModel.SelectedRoleIds, allProvidersViewModel.SelectedSupplementarySchemeIds, allProvidersViewModel.SelectedTrustFrameworkVersionId,
                PageNum, SearchText, allProvidersViewModel.SortBy);            
            allProvidersViewModel.PageNumber = PageNum;
            allProvidersViewModel.Providers = results.Items;
            allProvidersViewModel.TotalResults = results.TotalCount;
            allProvidersViewModel.TotalPages = (int)Math.Ceiling((double)results.TotalCount / 10);
            
            var lastUpdatedDate = await registerService.GetLastUpdatedDate();
            allProvidersViewModel.LastUpdated = lastUpdatedDate?.ToString("dd MMMM yyyy");
            return View(allProvidersViewModel);
        }

        #endregion

        #region Guidance
        [HttpGet("guidance-page")]
        public IActionResult Guidance()
        {
            return View("Guidance");
        }
        #endregion

        #region Updates
        [HttpGet("update-logs")]
        public async Task<IActionResult> Updates(int PageNum = 1)
        {
            var updateLogs= await registerService.GetUpdateLogs(PageNum);
            RegisterUpdatesLogsViewModel registerPublishLogsViewModel = new();         

            registerPublishLogsViewModel.PageNumber = PageNum;
            registerPublishLogsViewModel.RegisterUpdatesLog = updateLogs.Items;
            registerPublishLogsViewModel.TotalResults = updateLogs.TotalCount;
            registerPublishLogsViewModel.TotalPages = (int)Math.Ceiling((double)updateLogs.TotalCount / 10);

            registerPublishLogsViewModel.LastUpdated = updateLogs.LastUpdated;
            return View("Updates", registerPublishLogsViewModel);
        }
        #endregion


        [HttpGet("provider-details")]
        public async Task<IActionResult> ProviderDetails(int providerId, bool? fromServicePage, int? previousServiceId)
        {
            if (fromServicePage == true)
            {
                ViewBag.FromServicePage = fromServicePage;
                ViewBag.PreviousServiceId = previousServiceId;
            }
            ProviderProfileDto providerProfileDto = await registerService.GetProviderWithServiceDeatils(providerId);
            if(providerProfileDto == null)
                return RedirectToAction("RegisterPageNotFound", "Error");
            return View(providerProfileDto);
        }

        [HttpGet("service-details")]
        public async Task<IActionResult> ServiceDetails(int serviceId, bool? fromProviderPage, int? previousServiceId)
        {
            if (fromProviderPage == true)
            {
                ViewBag.FromProviderPage = true;
            }
            if (previousServiceId != null)
            {
                ViewBag.PreviousServiceId = previousServiceId;
            }
            ServiceDto service = await registerService.GetServiceDetails(serviceId);
            if (service == null)
                return RedirectToAction("RegisterPageNotFound", "Error");
            return View(service);
        }

        [EnableRateLimiting("DownloadRequestLimit")]
        [HttpGet("download-register")]
        public async Task<IActionResult> DownloadRegister(CancellationToken cancellationToken)
        {         
            if (!await _semaphore.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken))
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, "Too many concurrent downloads. Please try again later.");
            }

            try
            {
                var services = await registerService.GetPublishedServices();
                if (services == null || services.Count == 0)
                    throw new InvalidOperationException("No data to export");
                Response.ContentType = "text/csv";
                Response.Headers["Content-Disposition"] = $"attachment; filename=dvs-register_{DateTime.Now:ddMMyyyy}.csv";

                await using var writer = new StreamWriter(Response.Body);
                await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    Quote = '"',
                    ShouldQuote = args => args.Row.Index == 0 || args.Field.Contains(",")
                });
                csv.Context.RegisterClassMap<PfrCsvMap>();
                await csv.WriteRecordsAsync(services, cancellationToken);
                await writer.FlushAsync().ConfigureAwait(false);
                return new EmptyResult();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("No data available for download", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while attempting to download the register.", ex);
            }

            finally
            {
                _semaphore.Release();
            }

        }

        #region Private methods
        private async Task SetSchemes(List<int>? selectedIds, int removeId, PaginationAndFilteringParameters vm)
        {
            vm.AvailableSchemes = await cabService.GetSupplementarySchemes();

            if (selectedIds == null || selectedIds.Count == 0)
            {
                vm.SelectedSupplementarySchemeIds = [];
            }
            else
            {
                if (removeId > 0)
                    selectedIds.Remove(removeId);
                vm.SelectedSupplementarySchemeIds = selectedIds;
            }

            if (vm.SelectedSupplementarySchemeIds.Count > 0)
            {
                vm.SelectedSupplementarySchemes = vm.AvailableSchemes.Where(c => vm.SelectedSupplementarySchemeIds.Contains(c.Id)).ToList();
            }
        }
        private async Task SetRoles(List<int>? selectedIds, int removeId, PaginationAndFilteringParameters vm)
        {
            vm.AvailableRoles = await cabService.GetRoles(TFVersionNumber);
            if (selectedIds == null || selectedIds.Count == 0)
            {
                vm.SelectedRoleIds = [];
            }
            else
            {
                if (removeId > 0)
                    selectedIds.Remove(removeId);
                vm.SelectedRoleIds = selectedIds;
            }

            if (vm.SelectedRoleIds?.Count > 0)
            {
                vm.SelectedRoles = vm.AvailableRoles.Where(c => vm.SelectedRoleIds.Contains(c.Id)).ToList();
            }
        }

        private async Task SetTfVersion(List<int>? selectedIds, int removeId, PaginationAndFilteringParameters vm)
        {
            vm.AvailableTrustFrameworkVersion = await cabService.GetTfVersion();
            if (selectedIds == null || selectedIds.Count == 0)
            {
                vm.SelectedTrustFrameworkVersionId = [];
            }
            else
            {
                if (removeId > 0)
                    selectedIds.Remove(removeId);
                vm.SelectedTrustFrameworkVersionId = selectedIds;
            }

            if (vm.SelectedTrustFrameworkVersionId?.Count > 0)
            {
                vm.SelectedTrustFrameworkVersion = vm.AvailableTrustFrameworkVersion.Where(c => vm.SelectedTrustFrameworkVersionId.Contains(c.Id)).ToList();
            }
        }
        #endregion
    }
}