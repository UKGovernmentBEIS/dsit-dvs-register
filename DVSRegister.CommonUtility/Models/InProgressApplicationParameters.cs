namespace DVSRegister.CommonUtility.Models
{
    public class InProgressApplicationParameters
    {
        public bool HasInProgressApplication { get; set; }
        public int InProgressApplicationId { get; set; }
        public bool HasActiveRemovalRequest { get; set; }
        public int InProgressRemovalRequestServiceId { get; set; }
        public bool HasActiveReassignmentRequest { get; set; }
        public int InProgressReassignmentRequestServiceId { get; set; }

        public bool HasActiveUpdateRequest { get; set; }
        public bool LatestVersionInProgressAndUpdateRequested { get; set; }
        public int LatestVersionInProgressAndUpdateRequestedId { get; set; }
        public List<int>? InProgressUpdateRequestServiceIds { get; set; }
    }
}
