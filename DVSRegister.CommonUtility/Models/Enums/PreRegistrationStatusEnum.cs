using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum PreRegistrationStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("NA")]
        NotApplicable,
        [Description("Received")]
        Received  ,
        [Description("In Review")]
        InReview,
        [Description("Approved")]
        Approved,
        [Description("Rejected")]
        Rejected,
        [Description("Rejected - On Appeal")]
        RejectedOnAppeal
    }
}
