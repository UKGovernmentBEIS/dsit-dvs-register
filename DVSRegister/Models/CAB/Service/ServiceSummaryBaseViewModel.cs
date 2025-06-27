using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Models.CAB
{
    public class ServiceSummaryBaseViewModel
    {
        public bool IsDraft { get; set; }
        public bool IsAmendment { get; set; }
        public ServiceStatusEnum ServiceStatus { get; set; }
        public bool FromSummaryPage { get; set; }
        public bool FromDetailsPage { get; set; }
        public bool IsResubmission { get; set; }
        public string? RefererURL { get; set; }
        public int SchemeId { get; set; }
        public string SchemeName { get; set; } = string.Empty;
        public bool IsSchemeEditedFromSummary { get; set; }
        public bool IsTfVersion0_4 { get; set; }
        public bool FromUnderPinningServiceSummaryPage { get; set; }
    }
}