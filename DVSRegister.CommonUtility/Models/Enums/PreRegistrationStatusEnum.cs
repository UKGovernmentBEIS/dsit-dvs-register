using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum PreRegistrationStatusEnum
    {
        [Description("NA")]
        NotApplicable,
        [Description("RECEIVED")]
        Received  ,
        [Description("IN REVIEW")]
        InReview,
        [Description("APPROVED")]
        Approved,
        [Description("REJECTED")]
        Rejected,
        [Description("REJECTED - ON APPEAL")]
        RejectedOnAppeal
    }
}
