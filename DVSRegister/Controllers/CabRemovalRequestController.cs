using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/remove")]
    public class CabRemovalRequestController(ICabService cabService, ICabRemovalRequestService cabRemovalRequestService, ILogger<CabRemovalRequestController> logger) : BaseController(logger)
    {
        private readonly ICabService cabService = cabService;
        private readonly ICabRemovalRequestService cabRemovalRequestService = cabRemovalRequestService;
        private readonly ILogger<CabRemovalRequestController> logger = logger;


        [HttpGet("reason-for-removing")]
        public async Task<IActionResult> ReasonForRemoval(int providerId, int serviceId)
        {
            string reasonForRemoval = HttpContext.Session.GetString("ReasonForRemoval") ?? string.Empty;
            bool isProviderRemoval = await cabRemovalRequestService.IsLastService(serviceId, providerId);
            RemovalRequestViewModel removalRequestViewModel = new()
            {
                ProviderId = providerId,
                ServiceId = serviceId,
                RemovalReasonByCab = !string.IsNullOrEmpty(reasonForRemoval) ? reasonForRemoval : string.Empty,
                IsProviderRemoval = isProviderRemoval
               
            };            
            return View(removalRequestViewModel);
        }



        [HttpPost("reason-for-removing")]
        public async Task<IActionResult> SaveReasonForRemoval(RemovalRequestViewModel removalRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("ReasonForRemoval", removalRequestViewModel.RemovalReasonByCab);
             
                if (removalRequestViewModel.IsProviderRemoval)
                {
                    return RedirectToAction("ProviderWillBeRemoved", new { serviceId = removalRequestViewModel.ServiceId });
                   
                }
                else
                {
                    return RedirectToAction("AboutToRemove", new { serviceId = removalRequestViewModel.ServiceId });
                }               
            }
            else
            {
                return View("ReasonForRemoval", removalRequestViewModel);
            }
        }

        [HttpGet("provider-will-be-removed")]
        public async Task<IActionResult> ProviderWillBeRemoved(int serviceId)
        {
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
            return View("ProviderWillBeRemoved", serviceDto);
        }

        [HttpGet("about-to-remove")]
        public async Task<IActionResult> AboutToRemove(int serviceId)
        {
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
            string removalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval") ?? string.Empty;
            ViewBag.RemovalReasonByCab = removalReasonByCab;
            ViewBag.IsProviderRemoval = await cabRemovalRequestService.IsLastService(serviceId, serviceDto.ProviderProfileId);
            return View(serviceDto);
        }

        [HttpPost("about-to-remove")]
        public async Task<IActionResult> RequestRemoval(int providerId, int serviceId)
        {
        
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (providerId <= 0)
                throw new ArgumentException("RequestRemoval failed: Invalid ProviderId.");

            if (serviceId <= 0)
                throw new ArgumentException("RequestRemoval failed: Invalid ServiceId.");

            string removalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval") ?? string.Empty;
            ViewBag.RemovalReasonByCab = removalReasonByCab;
            HttpContext.Session.Remove("ReasonForRemoval");
            bool isProviderRemoval = await cabRemovalRequestService.IsLastService(serviceId, providerId);
            ViewBag.IsProviderRemoval = isProviderRemoval;
            GenericResponse genericResponse = await cabRemovalRequestService.AddServiceRemovalrequest(CabId,serviceId, UserEmail, removalReasonByCab, isProviderRemoval);
            if (genericResponse.Success)
            {
                ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
                         return View("RemovalRequested", serviceDto);
            }
            else
            {
                throw new InvalidOperationException("RequestRemoval failed: Unable to update removal status.");
            }
        }
    }
}