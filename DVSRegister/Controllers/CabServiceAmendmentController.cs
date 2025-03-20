using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Extensions;

namespace DVSRegister.Controllers
{
   

    [Route("cab-service/amend")]

    public class CabServiceAmendmentController(ICabService cabService, IBucketService bucketService, IUserService userService, ILogger<CabServiceAmendmentController> logger) : BaseController(logger)
    {

        private readonly ICabService cabService = cabService;
        private readonly IBucketService bucketService = bucketService;
        private readonly IUserService userService = userService;


        #region Amendments

        [HttpGet("service-amendments")]
        public async Task<IActionResult> ServiceAmendments(int serviceId)
        {


            SetRefererURL();

            ServiceDto service = await cabService.GetServiceDetails(serviceId, CabId);
            SetServiceDataToSession(CabId, service);
            AmendmentViewModel amendmentViewModel = new AmendmentViewModel
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
        #endregion
    }
}