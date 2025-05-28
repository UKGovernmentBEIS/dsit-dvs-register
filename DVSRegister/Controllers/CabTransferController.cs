using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CabTransfer;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CabTransfer;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Controllers
{
    [Route("cab-transfer")]
    public class CabTransferController(ICabTransferService cabTransferService, ILogger<CabTransferController> logger) : BaseController(logger)
    {
        private readonly ICabTransferService cabTransferService = cabTransferService;        

        [HttpGet("service-management-requests")]
        public async Task<IActionResult> ServiceManagementRequests()
        {
            var list = await cabTransferService.GetServiceTransferRequests(CabId);

            ServiceManagementRequestsViewModel serviceManagementRequestsViewModel = new()
            {
                PendingRequests = list.Where(x => x.RequestManagement != null && x.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                && (x.RequestManagement.RequestStatus == RequestStatusEnum.Pending)).ToList(),
                CompletedRequests = list.Where(x => x.RequestManagement != null && x.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                && (x.RequestManagement.RequestStatus == RequestStatusEnum.Approved || x.RequestManagement.RequestStatus == RequestStatusEnum.Rejected)).ToList()
            };

            return View(serviceManagementRequestsViewModel);
        }
      

        [HttpGet("service-details")]
        public async Task<IActionResult> ServiceDetails(int serviceId, int fromCabId)
        {
            ServiceDto serviceDto = await cabTransferService.GetServiceDetailsWithCabTransferDetails(serviceId, fromCabId);
            if (serviceDto.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || serviceDto.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
                return View(serviceDto);
            else throw new InvalidOperationException("Invalid service status for reassign");
           
        }

        [HttpPost("save-reassignment")]
        public async Task<IActionResult> SaveOrCancelReassignment(int requestId, string saveReassignment)
        {
            //to do
            return View();
           
        }
        [HttpGet("reassignment-success")]
        public IActionResult ReAssignmentSuccess()
        {
            return View();
        }
    }
}
