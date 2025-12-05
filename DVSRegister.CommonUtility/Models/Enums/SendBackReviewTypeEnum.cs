using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum SendBackReviewTypeEnum
    {
        [Description("Send back to primary review")]
        ToPrimaryReview = 1,
        [Description("This application needs to be sent back to certificate review")]
        ToCertificateReview = 2
    }
}
