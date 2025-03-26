using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{


    [Route("cab-service/amend")]

    public class CabServiceAmendmentController(ICabService cabService, ILogger<CabServiceAmendmentController> logger, IMapper mapper) : BaseController(logger)
    {

        private readonly ICabService cabService = cabService;    
        private readonly IMapper mapper = mapper;


        #region Amendments

        [HttpGet("service-amendments")]
        public async Task<IActionResult> ServiceAmendments(int serviceId)
        {
            SetRefererURL();
            ServiceDto service = await cabService.GetServiceDetails(serviceId, CabId);
            SetServiceDataToSession(CabId, service);
            AmendmentViewModel amendmentViewModel = new()
            {
                CertificateReview = service.CertificateReview,
                ServiceSummary = GetServiceSummary()
            };

            HttpContext?.Session.Set("CertificateReviewDetails", service.CertificateReview);
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
        public async Task<IActionResult> SaveServiceAmendmentsSummary()
        {
            ServiceSummaryViewModel summaryViewModel = GetServiceSummary();
            summaryViewModel.ServiceStatus = ServiceStatusEnum.Resubmitted;
            ServiceDto serviceDto = mapper.Map<ServiceDto>(summaryViewModel);

        

            if (serviceDto.CabUserId < 0) throw new InvalidDataException("Invalid CabUserId");

            GenericResponse genericResponse = new();
                if (summaryViewModel.IsAmendment)
                {
                    genericResponse = await cabService.SaveServiceAmendments(serviceDto, UserEmail);
                }                

                if (genericResponse.Success)
                {
                    return RedirectToAction("InformationSubmitted","CabService");
                }
                else
                {
                 throw new InvalidOperationException("SaveServiceAmendmentsSummary: Failed to save service amendments");
                }          

        }
        #endregion
    }
}