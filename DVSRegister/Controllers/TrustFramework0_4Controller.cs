using DVSRegister.BusinessLogic.Models;
using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("trust-framework-0.4")]
    public class TrustFramework0_4Controller(ITrustFrameworkService trustFrameworkService, ICabService cabService,IMapper mapper, ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;        
        private readonly ITrustFrameworkService trustFrameworkService = trustFrameworkService;
        private readonly ICabService cabService = cabService;
        private readonly IMapper mapper = mapper;
        

        [HttpGet("select-underpinning")]
        public IActionResult SelectUnderpinning()
        {
            return View();
        }

        [HttpGet("select-cab")]
        public async Task<IActionResult> SelectCabOfUnderpinningService()
        {
            var allCabs = await trustFrameworkService.GetCabs();
            var AllCabsViewModel = new AllCabsViewModel
            {
                Cabs = allCabs
            };

            return View(AllCabsViewModel);
        }


        [HttpGet("select-service-type")]
        public IActionResult SelectSelectServiceType()
        {
            //Figma 604
            // ServiceTypeEnum to pupulate radios

            return View();
        }

        [HttpPost("select-service-type")]
        public async Task<IActionResult> SaveSelectServiceType(string action)
        {
            //to do - set data
            bool fromSummaryPage = false;
            bool fromDetailsPage = false;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                //Todo:
                string nextPage = string.Empty;
                string controller = string.Empty;
                //nextPage = GPG45 in cabservice controller for  underpinning or neither
                //nextpage = StatusOfUnderpinningService in this controller for whitelabelled
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, nextPage, controller);
            }
            else
            {
                return View("SelectSelectServiceType");
            }
        }



        [HttpGet("status-of-underpinning-service")]
        public IActionResult StatusOfUnderpinningService()
            => View();

        [HttpGet("select-underpinning-or-white-labelled")]
        public IActionResult UnderpinningChoice()
             => View();


        private async Task<IActionResult> HandleActions(string action, ServiceSummaryViewModel serviceSummary, bool fromSummaryPage, bool fromDetailsPage, string nextPage, string controller = "TrustFramework0_4")
        {
            switch (action)
            {
                case "continue":
                    return fromSummaryPage ? RedirectToAction("ServiceSummary")
                        : fromDetailsPage ? await SaveAsDraftAndRedirect(serviceSummary)
                        : RedirectToAction(nextPage, controller);

                case "draft":
                    return await SaveAsDraftAndRedirect(serviceSummary);

                case "amend":
                    return RedirectToAction("ServiceAmendmentsSummary", "CabServiceAmendment");

                default:
                    throw new ArgumentException("Invalid action parameter");
            }
        }
        private async Task<IActionResult> SaveAsDraftAndRedirect(ServiceSummaryViewModel serviceSummary)
        {
            GenericResponse genericResponse = new();
            serviceSummary.ServiceStatus = ServiceStatusEnum.SavedAsDraft;
            ServiceDto serviceDto = mapper.Map<ServiceDto>(serviceSummary);
            if (!IsValidCabId(serviceSummary.CabId))
                return HandleInvalidCabId(serviceSummary.CabId);

            if (serviceDto.CabUserId < 0) throw new InvalidDataException("Invalid CabUserId");

            if (serviceSummary.IsResubmission)
            {
                genericResponse = await cabService.SaveServiceReApplication(serviceDto, UserEmail);
            }
            else
            {
                genericResponse = await cabService.SaveService(serviceDto, UserEmail);
            }

            if (genericResponse.Success)
            {
                HttpContext?.Session.Remove("ServiceSummary");
                return RedirectToAction("ProviderServiceDetails", "Cab", new { serviceKey = genericResponse.InstanceId });
            }
            else
            {
                throw new InvalidOperationException("SaveAsDraftAndRedirect: Failed to save draft");
            }

        }

        [HttpGet("tf-version")]
        public async Task<IActionResult> SelectVersionOfTrustFrameWork(bool fromSummaryPage, bool fromDetailsPage)
        {
            ViewBag.fromSummaryPage = fromSummaryPage;
            ViewBag.fromDetailsPage = fromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            TFVersionViewModel TFVersionViewModel = new()
            {
                SelectedTFVersionId = summaryViewModel?.TFVersionViewModel?.SelectedTFVersionId,
                AvailableVersions = await trustFrameworkService.GetTrustFrameworkVersions(),
                IsAmendment = summaryViewModel.IsAmendment,
                RefererURL = GetRefererURL()
            };
            return View(TFVersionViewModel);
        }

        [HttpPost("tf-version")]
        public async Task<IActionResult> SaveTFVersion(TFVersionViewModel TFVersionViewModel, string action)
        {
            bool fromSummaryPage = TFVersionViewModel.FromSummaryPage;
            bool fromDetailsPage = TFVersionViewModel.FromDetailsPage;
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            List<TrustFrameworkVersionDto> availableVersion = await trustFrameworkService.GetTrustFrameworkVersions();
            TFVersionViewModel.AvailableVersions = availableVersion;
            TFVersionViewModel.SelectedTFVersionId = TFVersionViewModel.SelectedTFVersionId ?? null;
            TFVersionViewModel.IsAmendment = summaryViewModel.IsAmendment;
            if (TFVersionViewModel.SelectedTFVersionId != null)
            {
                summaryViewModel.TFVersionViewModel.SelectedTFVersion = availableVersion
                    .FirstOrDefault(c => c.Id == TFVersionViewModel.SelectedTFVersionId);
            }

            if (ModelState.IsValid)
            {
                HttpContext?.Session.Set("ServiceSummary", summaryViewModel);
                return await HandleActions(action, summaryViewModel, fromSummaryPage, fromDetailsPage, "ServiceName", "CabService");
            }
            else
            {
                return View("SelectVersionOfTrustFrameWork", TFVersionViewModel);
            }
        }
    }
}
