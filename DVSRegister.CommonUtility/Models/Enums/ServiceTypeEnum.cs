using DVSAdmin.CommonUtility.Models.Enums;
using System.ComponentModel;
using System.Reflection;

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


    public static class ServiceTypeEnumExtensions
    {
        public static string GetDescription(this ServiceTypeEnum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetDisplayValue(this ServiceTypeEnum? value)
        {
            switch (value)
            {
                case ServiceTypeEnum.UnderPinning:
                    return "Underpinning service";
                case ServiceTypeEnum.WhiteLabelled:
                    return "White-labelled service";
                case ServiceTypeEnum.Neither:
                    return "Neither";
                default:
                    return "Incomplete";
            }
        }
    }
}
