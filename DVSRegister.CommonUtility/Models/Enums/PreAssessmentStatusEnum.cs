using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum PreAssessmentStatusEnum
    {

        [Description("RECEIVED")]
        Received,
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
