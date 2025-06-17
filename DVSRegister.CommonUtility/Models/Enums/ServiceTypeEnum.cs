using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ServiceTypeEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("It is an underpinning service")]
        UnderPinning = 1,
        [Description("It is a white-labelled service")]
        WhiteLabelled = 2,
        [Description("Neither")]
        Neither = 3
    }
}
