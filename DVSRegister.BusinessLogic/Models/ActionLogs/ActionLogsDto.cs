using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models
{
    public class ActionLogsDto
    {
        public ActionCategoryEnum ActionCategoryEnum { get; set; }
        public ActionDetailsEnum ActionDetailsEnum { get; set; }
        public ActionDetailsDto ActionDetails { get; set; }
        public ProviderProfileDto ProviderProfile { get; set; }
        public ServiceDto Service { get; set; }
        public int RequestedUserId { get; set; }
        public string LoggedInUserEmail { get; set; }
        public int? ServiceId { get; set; }
        public int ProviderId { get; set; }
        public string ServiceName { get; set; }
        public string ProviderName { get; set; }
        public Dictionary<string, List<string>>? PreviousData { get; set; }
        public Dictionary<string, List<string>>? UpdatedData { get; set; }
        public DateTime? UpdateRequestedTime { get; set; }
        public DateTime LogDate { get; set; } // Date only
        public DateTime LoggedTime { get; set; } // Date + time
        public string DisplayMessage { get; set; }
        public string? DisplayMessageAdmin { get; set; }
        public bool ShowInRegisterUpdates { get; set; }
        public int? CertificateReviewId { get; set; }
        public CertificateReviewDto? CertificateReview { get; set; }
        public bool IsProviderPreviouslyPublished { get; set; }
        public int? CabTransferRequestId { get; set; }
    }
}
