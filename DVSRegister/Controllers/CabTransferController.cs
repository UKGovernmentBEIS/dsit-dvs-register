﻿using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CabTransfer;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CabTransfer;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-transfer")]
    public class CabTransferController(ICabTransferService cabTransferService, IConfiguration configuration, ILogger<CabTransferController> logger) : BaseController(logger)
    {
        private readonly ICabTransferService cabTransferService = cabTransferService;
        private readonly IConfiguration configuration = configuration;        
        
        [HttpGet("service-management-requests")]
        public async Task<IActionResult> ServiceManagementRequests(string previousPage)
        {
            var list = await cabTransferService.GetServiceTransferRequests(CabId);

            ServiceManagementRequestsViewModel serviceManagementRequestsViewModel = new()
            {
                PendingRequests = list.Where(x => x.RequestManagement != null && x.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                && (x.RequestManagement.RequestStatus == RequestStatusEnum.Pending)).ToList(),
                CompletedRequests = list.Where(x => x.RequestManagement != null && x.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                && (x.RequestManagement.RequestStatus == RequestStatusEnum.Approved)).ToList()
            };
            ViewBag.previousPage = previousPage;
            return View(serviceManagementRequestsViewModel);
        }
      

        [HttpGet("service-details")]
        public async Task<IActionResult> ServiceDetails(int serviceId, int fromCabId)
        {
            ServiceDto serviceDto = await cabTransferService.GetServiceDetailsWithCabTransferDetails(serviceId, fromCabId);
            if (serviceDto.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || serviceDto.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
            {
                serviceDto.CabTransferRequestId = serviceDto.CabTransferRequest.Where(x=>x.ServiceId == serviceDto.Id && x.CertificateUploaded== false && x.RequestManagement!=null 
                && x.RequestManagement.RequestStatus == RequestStatusEnum.Pending && x.RequestManagement.RequestType== RequestTypeEnum.CabTransfer).Select(x=>x.Id).FirstOrDefault();
                return View(serviceDto);
            }
                
            else throw new InvalidOperationException("Invalid service status for reassign");           
        }
      

        [HttpGet("about-to-approve")]
        public async Task<IActionResult> AboutToApproveReAssignment(int requestId)
        {           
            var cabTransferRequest = await cabTransferService.GetCabTransferRequestDetails(requestId);
            TempData["ServiceName"] = cabTransferRequest.Service.ServiceName;
            TempData["ProviderName"] = cabTransferRequest.Service.Provider.RegisteredName;
            return View(cabTransferRequest);
        }

        [HttpPost("about-to-approve")]
        public async Task<IActionResult> ApproveReAssignment(int requestId, int providerProfileId)
        {
            TempData.Keep();
            GenericResponse genericResponse = await cabTransferService.ApproveOrCancelTransferRequest(true,requestId, providerProfileId ,UserEmail);
            if (genericResponse.Success)
                return RedirectToAction("ReAssignmentSuccess");
            else throw new InvalidOperationException("ApproveReAssignment failed");           
        }

        [HttpGet("reassignment-success")]
        public IActionResult ReAssignmentSuccess()
        {
            TempData["CabURL"] = configuration.GetValue<string>("GovUkNotify:CabLoginLink") ?? string.Empty;
            return View();
        }

        [HttpGet("about-to-reject")]
        public async Task<IActionResult> AboutToRejectReAssignment(int requestId)
        {
            var cabTransferRequest = await cabTransferService.GetCabTransferRequestDetails(requestId);
            TempData["ServiceName"] = cabTransferRequest.Service.ServiceName;
            TempData["ProviderName"] = cabTransferRequest.Service.Provider.RegisteredName;         
            return View(cabTransferRequest);
        }

        [HttpPost("about-to-reject")]
        public async Task<IActionResult> RejectReAssignment(int requestId, int providerProfileId)
        {
            TempData.Keep();
            GenericResponse genericResponse = await cabTransferService.ApproveOrCancelTransferRequest(false, requestId, providerProfileId, UserEmail);
            if (genericResponse.Success)
                return RedirectToAction("ReAssignmentRejected");
            else throw new InvalidOperationException("ApproveReAssignment failed");
        }

        [HttpGet("reassignment-rejected")]
        public IActionResult ReAssignmentRejected()
        {
            TempData["CabURL"] = configuration.GetValue<string>("GovUkNotify:CabLoginLink") ?? string.Empty; 
            return View();
        }
    }
}
