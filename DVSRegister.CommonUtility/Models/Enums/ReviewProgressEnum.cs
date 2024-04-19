using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ReviewProgressEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Primary Check Draft")]
        PrimaryCheckDraft,
        [Description("Primary Check Completed")]
        PrimaryCheckCompleted,
        [Description("Secondary Check Completed")]
        SecondaryCheckCompleted
    }
}
