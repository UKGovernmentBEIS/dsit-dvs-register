using System.ComponentModel;
using System.Reflection;

namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum CabUserRemovalReasonEnum
    {
        //Donot change order of the enum as the ids are used to save in database
        //New entries should be added before "Other" but after the last specific reason
        [Description("This CAB user is leaving their CAB")]
        LeavingCAB = 1,
        [Description("This CAB user no longer needs access")]
        NoLongerNeedsAccess = 2,
        [Description("This user’s CAB is no longer accredited")]
        CABNoLongerAccredited = 3,
        [Description("This user’s CAB is no longer approved")]
        CABNoLongerApproved = 4,


        [Description("Other")]
        Other = 100
    }

    public static class CabUserRemovalReasonEnumExtensions
    {
        public static string GetDescription(this CabUserRemovalReasonEnum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static string GetDescription(this CabUserRemovalReasonEnum? value)
        {
            if (!value.HasValue)
                return "Not specified"; // fallback for null

            return value.Value.GetDescription(); // calls the non-nullable version
        }
    }
}
