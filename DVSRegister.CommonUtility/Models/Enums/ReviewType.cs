using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ReviewTypeEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Primary Check")]
        PrimaryCheck = 1,
        [Description("Secondary Check")]
        SecondaryCheck = 2
    }
}
