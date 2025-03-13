using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/remove")]
    public class CabRemovalRequestController : BaseController
    {
        private readonly ICabService cabService;
        private readonly ICabRemovalRequestService cabRemovalRequestService;  
        private readonly ILogger<CabRemovalRequestController> _logger;

   
        public CabRemovalRequestController(ICabService cabService,  ICabRemovalRequestService cabRemovalRequestService, ILogger<CabRemovalRequestController> logger)
        {
            this.cabService = cabService;
            this.cabRemovalRequestService = cabRemovalRequestService;
            _logger = logger;
            
        }

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
            if (CabId > 0 && providerId > 0 && serviceId > 0)
            {
                string removalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval");
                string whatToRemove = HttpContext.Session.GetString("WhatToRemove");

                HttpContext.Session.Remove("ReasonForRemoval");
                HttpContext.Session.Remove("WhatToRemove");

                GenericResponse genericResponse = await cabRemovalRequestService.UpdateRemovalStatus(CabId, providerId, serviceId, UserEmail, removalReasonByCab, whatToRemove);
                if (genericResponse.Success)
                {
                    ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, CabId);
                    ViewBag.WhatToRemove = whatToRemove;
                    return View("RemovalRequested", serviceDto);
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("RequestRemoval failed: Unable to update removal status."));
                    return RedirectToAction("CabHandleException", "Error");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("RequestRemoval failed: Invalid CabId, ProviderId, or ServiceId."));
                return RedirectToAction("CabHandleException", "Error");
            }
        }


    }
}
