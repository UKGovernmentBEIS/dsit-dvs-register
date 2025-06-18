using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("trust-framework-0.4")]
    public class TrustFramework0_4Controller(ICabService cabService,IMapper mapper, ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;
        private readonly ICabService cabService = cabService;
        private readonly IMapper mapper = mapper;


        [HttpGet("select-underpinning")]
        public IActionResult SelectUnderpinning()
        {
            return View();
        }

        [HttpGet("select-cab")]
        public IActionResult SelectCabOfUnderpinningService()
        {   
            // need to get cabs from repo
            var allCabs = new List<CabDto>
            {
                new CabDto { Id = 1, CabName = "Cab A" },
                new CabDto { Id = 2, CabName = "Cab B" },
                new CabDto { Id = 3, CabName = "Cab C" }
            };
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
    }
}
