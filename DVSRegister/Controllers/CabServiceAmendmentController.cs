using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{


    [Route("cab-service/amend")]

    public class CabServiceAmendmentController(ICabService cabService, IActionLogService actionLogService, 
        ILogger<CabServiceAmendmentController> logger, IMapper mapper) : BaseController(logger)
    {

        private readonly ICabService cabService = cabService;    
        private readonly IMapper mapper = mapper;
        private readonly IActionLogService actionLogService = actionLogService;
       


        #region Amendments

        [HttpGet("service-amendments")]
        public async Task<IActionResult> ServiceAmendments(int serviceId)
        {
            SetRefererURL();
            ServiceDto service = await cabService.GetServiceDetails(serviceId, CabId);
            var latestReview = service.CertificateReview.SingleOrDefault(x => x.IsLatestReviewVersion)!;
            bool isAmendment = service.ServiceStatus == ServiceStatusEnum.AmendmentsRequired;
            SetServiceDataToSession(CabId, service, isAmendment);
            AmendmentViewModel amendmentViewModel = new()
            {
                CertificateReview = latestReview,
                ServiceSummary = GetServiceSummary()
            };

            HttpContext?.Session.Set("CertificateReviewDetails", latestReview);
            return View(amendmentViewModel);

            }


            [HttpGet("service-amendments-summary")]
        public IActionResult ServiceAmendmentsSummary()
        {
            SetRefererURL();

            AmendmentViewModel amendmentViewModel = new AmendmentViewModel();
            amendmentViewModel.CertificateReview = HttpContext?.Session.Get<CertificateReviewDto>("CertificateReviewDetails") ?? new CertificateReviewDto();
            amendmentViewModel.ServiceSummary = GetServiceSummary();
            return View(amendmentViewModel);

        }

        [HttpPost("service-amendments-summary")]
        public async Task<IActionResult> SaveServiceAmendmentsSummary(string action)
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.ServiceStatus = ServiceStatusEnum.Resubmitted;
            ServiceDto serviceDto = mapper.Map<ServiceDto>(summaryViewModel);
            ViewModelHelper.MapTFVersion0_4Fields(summaryViewModel, serviceDto);
            var existingService = await cabService.GetServiceDetails(summaryViewModel.ServiceId,CabId);

            if (action == "save")
            {
                GenericResponse genericResponse = new();
                if (summaryViewModel.IsAmendment)
                {
                    genericResponse = await cabService.SaveServiceAmendments(serviceDto, existingService.FileLink, existingService.CabUser.CabId, CabId, UserEmail);
                }

                if (genericResponse.Success)
                {
                    ServiceDto submittedService = await cabService.GetServiceDetailsWithProvider(genericResponse.InstanceId, CabId);
                    string providerName = submittedService.Provider?.RegisteredName ?? string.Empty;                

                    await actionLogService.AddActionLog(submittedService, ActionCategoryEnum.CR, ActionDetailsEnum.CR_Submitted,UserEmail);
                    return RedirectToAction("InformationSubmitted", "CabService", new { providerName, serviceName = submittedService.ServiceName });
                }
                else
                {
                    throw new InvalidOperationException("Failed: SaveServiceAmendmentsSummary");
                }
            }
            else if (action == "discard")
            {
                if(cabService.CanDeleteCertificate(serviceDto.FileLink, existingService.FileLink, existingService.CabUser.CabId,CabId))
                {
                    //TODO: uncomment after S3 changes
                   // await bucketService.DeleteFromS3Bucket(serviceDto.FileLink);
                }                
                return RedirectToAction("ServiceAmendments", new { serviceId = summaryViewModel.ServiceId });
            
            }
            else
            {
                throw new ArgumentException("Invalid action parameter");
            }

        }
        #endregion


     
    }
}