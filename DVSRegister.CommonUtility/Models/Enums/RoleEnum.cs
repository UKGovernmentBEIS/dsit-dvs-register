using System.ComponentModel;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum RoleEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("Identity")]
        Identity = 1,
        [Description("Attribute")]
        Attribute = 2,
        [Description("Orchestration")]
        Orchestration = 3,
        [Description("Holder")]
        Holder = 4,
        [Description("Component")]
        Component = 5

    }
}
