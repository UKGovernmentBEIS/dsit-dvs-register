using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-service/remove")]
    [ValidCognitoToken]
    public class CabRemovalRequestController : Controller
    {
        private readonly ICabService cabService;
        private readonly ICabRemovalRequestService cabRemovalRequestService;  

        private string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        public CabRemovalRequestController(ICabService cabService,  ICabRemovalRequestService cabRemovalRequestService)
        {
            this.cabService = cabService;
            this.cabRemovalRequestService = cabRemovalRequestService;
            
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
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);

            serviceDto.RemovalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval");
            ViewBag.WhatToRemove = HttpContext.Session.GetString("WhatToRemove");

            return View(serviceDto);
        }

        [HttpPost("about-to-remove")]
        public async Task<IActionResult> RequestRemoval(int providerId, int serviceId)
        {
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0 && providerId > 0 && serviceId > 0)
            {
                string removalReasonByCab = HttpContext.Session.GetString("ReasonForRemoval");
                string whatToRemove = HttpContext.Session.GetString("WhatToRemove");

                HttpContext.Session.Remove("ReasonForRemoval");
                HttpContext.Session.Remove("WhatToRemove");

                GenericResponse genericResponse = await cabRemovalRequestService.UpdateRemovalStatus(cabId, providerId, serviceId, UserEmail, removalReasonByCab, whatToRemove);
                if (genericResponse.Success)
                {
                    ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);
                    ViewBag.WhatToRemove = whatToRemove;
                    return View("RemovalRequested", serviceDto);
                }
                else
                {
                    return RedirectToAction("CabHandleException", "Error");
                }
            }
            else
            {
                return RedirectToAction("CabHandleException", "Error");
            }
        }


    }
}
