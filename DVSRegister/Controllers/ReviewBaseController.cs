using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    public class ReviewBaseController : Controller
    {
        private readonly IActionLogService actionLogService;

        public ReviewBaseController (IActionLogService actionLogService )
        {            
            this.actionLogService = actionLogService;
        }

        protected Task AddActionLog(ServiceDto serviceDto, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string? displayMessageAdmin = null)
        {

            if (actionLogService is null)
                return Task.CompletedTask;

            ArgumentNullException.ThrowIfNull(serviceDto, nameof(serviceDto));
            ArgumentNullException.ThrowIfNull(serviceDto.Provider, $"{nameof(serviceDto)}.{nameof(serviceDto.Provider)}");

            var actionLogsDto = new ActionLogsDto
            {              
                ActionCategoryEnum = actionCategory,
                ActionDetailsEnum = actionDetails,
                ServiceId = serviceDto.Id,
                ServiceName = serviceDto.ServiceName,
                ServiceStatus = serviceDto.ServiceStatus,
                ProviderId = serviceDto.Provider.Id,
                ProviderName = serviceDto.Provider.RegisteredName ?? string.Empty,
                PublicInterestCheckId = serviceDto.PublicInterestCheck?
                                           .FirstOrDefault(x => x.IsLatestReviewVersion)?.Id,
                CertificateReviewId = serviceDto.CertificateReview?
                                           .FirstOrDefault(x => x.IsLatestReviewVersion)?.Id,
                CabTransferRequestId = serviceDto.CabTransferRequestId,
                ServiceRemovalRequestId = serviceDto.ServiceRemovalRequestId,
                DisplayMessageAdmin = displayMessageAdmin
            };


            return actionLogService.SaveActionLogs(actionLogsDto);
        }
    }
}
