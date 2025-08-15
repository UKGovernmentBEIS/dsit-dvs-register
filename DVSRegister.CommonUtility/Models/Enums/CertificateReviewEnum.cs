using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum CertificateReviewEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
      
        [Description("In review")]
        InReview = 1,
        [Description("Passed")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Expired")]
        Expired = 4,
        [Description("Amendments needed")]
        AmendmentsRequired = 5,
        [Description("Declined by Provider")]
        DeclinedByProvider = 6
    }
}
