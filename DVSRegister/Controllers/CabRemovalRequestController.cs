using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Email;
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
        private readonly IEmailSender emailSender;

        private string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        public CabRemovalRequestController(ICabService cabService,  ICabRemovalRequestService cabRemovalRequestService, IEmailSender emailSender)
        {
            this.cabService = cabService;          
            this.emailSender = emailSender;
            this.cabRemovalRequestService = cabRemovalRequestService;
            
        }
        [HttpGet("reason-for-removing")]
        public IActionResult ReasonForRemoval(int providerid , int serviceId)
        {
            string reasonForRemoval = TempData["ReasonForRemoval"] as string ?? string.Empty;
            RemovalRequestViewModel removalRequestViewModel = new()
            {
                ProviderId = providerid,
                ServiceId = serviceId,
                RemovalReasonByCab = !string.IsNullOrEmpty(reasonForRemoval)?reasonForRemoval:string.Empty
            };
            return View(removalRequestViewModel);
        }

        [HttpPost("reason-for-removing")]
        public IActionResult SaveReasonForRemoval(RemovalRequestViewModel removalRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                TempData["ReasonForRemoval"] = removalRequestViewModel.RemovalReasonByCab; 
                return RedirectToAction("AboutToRemoveService",new { serviceId = removalRequestViewModel.ServiceId});
            }
            else
            {
                return View("ReasonForRemoval", removalRequestViewModel);
            }
        }


        [HttpGet("about-to-remove-service")]
        public async Task<IActionResult> AboutToRemoveService(int serviceId)
        {
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);
            serviceDto.RemovalReasonByCab = TempData["ReasonForRemoval"] as string;
            TempData.Keep();
            return View(serviceDto);
        }

        [HttpPost("about-to-remove-service")]
        public async Task<IActionResult> RequestServiceRemoval(int providerId, int serviceId)
        {
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0 && providerId> 0 && serviceId > 0 )
            {
               string removalReasonByCab = TempData["ReasonForRemoval"] as string;
               TempData.Clear();
               GenericResponse genericResponse =  await cabRemovalRequestService.UpdateRemovalStatus(cabId, providerId, serviceId, UserEmail, removalReasonByCab);
               if(genericResponse.Success)
                {                   
                    ServiceDto serviceDto = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);
                    //45 / CAB / Service removal requested
                    await emailSender.CabServiceRemovalRequested(UserEmail, UserEmail, serviceDto.Provider.RegisteredName, serviceDto.ServiceName, removalReasonByCab);
                    //46/DSIT/CAB service removal request
                    await emailSender.CabServiceRemovalRequestedToDSIT(serviceDto.Provider.RegisteredName, serviceDto.ServiceName, removalReasonByCab);
                    return View("ServiceRemovalRequested",serviceDto);
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
