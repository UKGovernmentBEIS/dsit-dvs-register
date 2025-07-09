using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum IdentityProfileTypeEnum
    {

        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Low")]
        Low = 1,
        [Description("Medium")]
        Medium = 2,
        [Description("High")]
        High = 3,
        [Description("Very High")]
        VeryHigh = 4
    }
}
