using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum CertificateInfoStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Received")]
        Received = 1,
        [Description("In review")]
        InReview = 2,
        [Description("Approved - sent for provider consent")]
        Approved = 3,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Expired")]
        Expired = 5,
        [Description("Ready to publish")]
        ReadyToPublish = 6,
        [Description("Published")]
        Published = 7,
        [Description("Removed")]
        Removed = 8
    }
}
