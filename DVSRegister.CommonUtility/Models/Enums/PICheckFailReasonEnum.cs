using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum PICheckFailReasonEnum
    {
        [Description("This application failed primary review")]
        FailedPrimaryReview = 1,
        [Description("This application needs to be sent back to certificate review")]
        SendBackToCertificateReview = 2
    }
}
