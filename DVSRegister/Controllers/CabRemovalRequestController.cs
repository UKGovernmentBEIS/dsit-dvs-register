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
        private readonly ILogger<CabRemovalRequestController> _logger = logger;

        [HttpGet("reason-for-removing")]
        public IActionResult ReasonForRemoval(int providerid, int serviceId, string whatToRemove)
        {
            string reasonForRemoval = HttpContext.Session.GetString("ReasonForRemoval") ?? string.Empty;
            RemovalRequestViewModel removalRequestViewModel = new()
            {
                ProviderId = providerid,
                ServiceId = serviceId,
                RemovalReasonByCab = !string.IsNullOrEmpty(reasonForRemoval) ? reasonForRemoval : string.Empty,
                WhatToRemove = whatToRemove
            };
            HttpContext.Session.SetString("WhatToRemove", removalRequestViewModel.WhatToRemove);
            return View(removalRequestViewModel);
        }



        [HttpPost("reason-for-removing")]
        public IActionResult SaveReasonForRemoval(RemovalRequestViewModel removalRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("ReasonForRemoval", removalRequestViewModel.RemovalReasonByCab);
                return RedirectToAction("AboutToRemove", new { serviceId = removalRequestViewModel.ServiceId });
            }
            else
            {
                return View("ReasonForRemoval", removalRequestViewModel);
            }
        }

        [HttpGet("about-to-remove")]
        public async Task<IActionResult> AboutToRemove(int serviceId)
        {
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);

            serviceDto.RemovalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval");
            ViewBag.WhatToRemove = HttpContext.Session.GetString("WhatToRemove");

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

            string removalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval");
            string whatToRemove = HttpContext.Session.GetString("WhatToRemove");

            HttpContext.Session.Remove("ReasonForRemoval");
            HttpContext.Session.Remove("WhatToRemove");

            GenericResponse genericResponse = await cabRemovalRequestService.AddServiceRemovalrequest(CabId,serviceId, UserEmail, removalReasonByCab, whatToRemove);
            if (genericResponse.Success)
            {
                ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
                ViewBag.WhatToRemove = whatToRemove;
                return View("RemovalRequested", serviceDto);
            }
            else
            {
                throw new InvalidOperationException("RequestRemoval failed: Unable to update removal status.");
            }
        }
    }
}