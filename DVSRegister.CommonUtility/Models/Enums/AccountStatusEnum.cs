using System.ComponentModel;
using System.Reflection;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum AccountStatusEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added after the last
        [Description("Invited")]
        Invited = 1,
        [Description("Active")]
        Active = 2,
        [Description("Suspended")]
        Suspended = 3,
        [Description("Deleted")]
        Deleted = 4,
        [Description("Cancelled")]
        Cancelled = 5
    }

    public static class AccountStatusEnumExtensions
    {
        public static string GetDescription(this AccountStatusEnum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
