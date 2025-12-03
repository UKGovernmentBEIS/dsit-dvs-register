namespace DVSRegister.CommonUtility.Models
{
    public class InProgressApplicationParameters
    {
        public int ServiceId { get; set; }
        public bool HasInProgressApplication { get; set; }
        public int InProgressApplicationId { get; set; }
        public bool HasActiveRemovalRequest { get; set; }
        public int InProgressRemovalRequestServiceId { get; set; }
        public bool HasActiveReassignmentRequest { get; set; }
        public int InProgressReassignmentRequestServiceId { get; set; }

        public bool HasActiveUpdateRequest { get; set; }
        public bool InProgressAndUpdateRequested { get; set; }
        public int InProgressAndUpdateRequestedId { get; set; }
        public List<int>? InProgressUpdateRequestServiceIds { get; set; }
    }
}
