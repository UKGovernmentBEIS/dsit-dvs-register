using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.CommonUtility;

namespace DVSRegister.Controllers
{
    [Route("cab-service/remove")]
    public class CabRemovalRequestController(ICabService cabService, ICabRemovalRequestService cabRemovalRequestService, ILogger<CabRemovalRequestController> logger) : BaseController(logger)
    {
        private readonly ICabService cabService = cabService;
        private readonly ICabRemovalRequestService cabRemovalRequestService = cabRemovalRequestService;
        private readonly ILogger<CabRemovalRequestController> logger = logger;


        [HttpGet("reason-for-removing/{providerId}/{serviceId}/{serviceKey}")]
        public async Task<IActionResult> ReasonForRemoval(int providerId, int serviceId, int serviceKey)
        { 
            string reasonForRemoval = HttpContext.Session.GetString("ReasonForRemoval") ?? string.Empty;
            bool isProviderRemoval = await cabRemovalRequestService.IsLastService(serviceId, providerId);
            RemovalRequestViewModel removalRequestViewModel = new()
            {
                ProviderId = providerId,
                ServiceId = serviceId,
                ServiceKey = serviceKey,
                RemovalReasonByCab = !string.IsNullOrEmpty(reasonForRemoval) ? reasonForRemoval : string.Empty,
                IsProviderRemoval = isProviderRemoval
               
            };            
            return View(removalRequestViewModel);
        }



        [HttpPost("reason-for-removing")]
        public IActionResult SaveReasonForRemoval(RemovalRequestViewModel removalRequestViewModel)
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

        [HttpGet("provider-will-be-removed/{serviceId}")]
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
            ViewBag.RefererURL = GetRefererURL();
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

        [HttpGet("cancel-removal-request/{serviceId}")]
        public async Task<IActionResult> CancelRemovalRequest(int serviceId)
        {
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
            return View(serviceDto);
        }

        [HttpPost("cancel-removal-request")]
        public async Task<IActionResult> CancelRemovalRequestConfirmation(int serviceId)
        {
            GenericResponse genericResponse = await cabRemovalRequestService.CancelServiceRemovalRequest(CabId, serviceId, UserEmail);
            if (genericResponse.Success)
            {
                ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
                return View("RemovalRequestCancelled", serviceDto);
            }
            else
            {
                if(genericResponse.ErrorType == ErrorTypeEnum.RequestAlreadyProcessed)
                    return Redirect(Constants.ActionNotCompletedErrorPath);
                throw new InvalidOperationException("CancelRemovalRequest failed: Unable to cancel removal request.");
            }
        }
    }
}