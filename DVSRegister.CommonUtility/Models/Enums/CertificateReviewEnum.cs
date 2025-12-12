using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum CertificateReviewEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last      
      
        [Description("Passed")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,  
        [Description("Amendments needed")]
        AmendmentsRequired = 5,
        [Description("Passed")]
        DeclinedByProvider = 6,
        [Description("Invitation cancelled")]
        InvitationCancelled = 7,
        [Description("Application restored")]
        Restored = 8,
        [Description("Sent back from Primary check")]
        SentBackFromPrimaryCheck = 9,
        [Description("Sent back from secondary check")]
        SentBackFromSecondaryCheck = 10
    }
}
