using System.ComponentModel;
using System.Reflection;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum OfDIAUserRemovalReasonEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added at the last
        [Description("This OfDIA user is leaving OfDIA")]
        LeavingOfDIA = 1,
        [Description("This OfDIA user is leaving DSIT")]
        LeavingDSIT = 2,
        [Description("This OfDIA user no longer needs access")]
        AccessNoLongerNeeded = 3,
        [Description("Other")]
        Other = 4
    }
    public static class OfDIAUserRemovalReasonEnumExtensions
    {
        public static string GetDescription(this OfDIAUserRemovalReasonEnum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

