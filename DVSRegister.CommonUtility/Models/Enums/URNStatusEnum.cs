using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum URNStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Created")]
        Created = 1,
        [Description("Rejected")]
        Rejected = 2,
        [Description("Approved - CAB Validation pending")]
        Approved = 3,
        [Description("Validated by CAB")]
        ValidatedByCAB = 4,
        [Description("Expired")]
        Expired = 5
    }
}
