using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum RequestStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3
    }
}
