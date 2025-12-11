using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service")]   
    public class CabController(ICabService cabService,ILogger<CabController> logger) : BaseController(logger)
    {
    
        private readonly ICabService cabService = cabService;
        [HttpGet("service-details/{serviceKey}/{fromOpenTasks?}")]
        public async Task<IActionResult> ProviderServiceDetails(int serviceKey, bool? fromOpenTasks)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            ServiceVersionViewModel serviceVersions = new();
            var serviceList = await cabService.GetServiceList(serviceKey, CabId);
            ServiceDto latestSubmission = serviceList?.Where(s => !(s.ServiceStatus == ServiceStatusEnum.SavedAsDraft  || s.ServiceStatus == ServiceStatusEnum.AmendmentsRequired))
                .OrderByDescending(s=> s.ServiceVersion).FirstOrDefault() ?? new ServiceDto();
            ServiceDto currentServiceVersion = await cabService.GetServiceDetails(latestSubmission.Id, CabId);
            serviceVersions.CurrentServiceVersion = currentServiceVersion;
            serviceVersions.ServiceHistoryVersions = serviceList?.Where(x => x.ServiceVersion < currentServiceVersion.ServiceVersion).OrderByDescending(x=> x.PublishedTime).ToList()?? new ();
            serviceVersions.ProviderProfileId = currentServiceVersion.ProviderProfileId;
            serviceVersions.Provider = currentServiceVersion.Provider;
            ViewBag.FromOpenTasks = fromOpenTasks;
            var latestCabTransferRequest = currentServiceVersion?.CabTransferRequest?.OrderByDescending(c => c.Id).FirstOrDefault();
            currentServiceVersion.CertificateUploadRequired = latestCabTransferRequest != null && latestCabTransferRequest.RequestManagement != null
                       && latestCabTransferRequest.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                       && latestCabTransferRequest.RequestManagement.RequestStatus == RequestStatusEnum.Approved
                       && latestCabTransferRequest.CertificateUploaded == false;
           

            if (serviceList != null && !serviceList.Any(x=>x.ServiceStatus == ServiceStatusEnum.SavedAsDraft)
                 && (serviceList.Any(x=>x.IsInRegister == true) 
                || serviceList.Any(x => x.ServiceStatus == ServiceStatusEnum.Removed)) )
            {
                serviceVersions.CurrentServiceVersion.EnableResubmission = true;
            }

            serviceVersions.CanRemoveService = serviceList?.Any(x => x.ServiceStatus == ServiceStatusEnum.Published) ?? false;
            serviceVersions.CanCancelRemovalRequest = serviceList?.Any(x => x.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation) ?? false;
            if (serviceVersions.CanRemoveService)
            {
                var publishedService = serviceList?.Where(s => s.ServiceStatus == ServiceStatusEnum.Published).FirstOrDefault();
                serviceVersions.PublishedServiceId = publishedService?.Id ?? 0;
            }
            else if (serviceVersions.CanCancelRemovalRequest)
            {
                var removalRequestedService = serviceList?.Where(s => s.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation).FirstOrDefault();
                serviceVersions.PublishedServiceId = removalRequestedService?.Id ?? 0; // the service is still published
            }            
          
             return View(serviceVersions);
        }

        [HttpGet("service-version-details/{serviceId}")]
        public async Task<IActionResult> ServiceVersionDetails(int serviceId)
        {
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            var serviceVersion = await cabService.GetServiceDetails(serviceId, CabId);
            
            if (serviceVersion == null || serviceVersion.Id == 0)
            {
                return NotFound();
            }
          
            return View(serviceVersion);
        }

     
    }
}