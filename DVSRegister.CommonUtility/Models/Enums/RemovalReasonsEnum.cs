using System.ComponentModel;
using System.Reflection;

namespace DVSAdmin.CommonUtility.Models.Enums
{
    public enum RemovalReasonsEnum
    {
        // Do not change the order of the enum as the ids are used to save in the database
        // New entries should be added at the last
        [Description("The service provider service certificates have all expired")]
        RemovedByCronJob = 1,
        [Description("The service provider has requested to remove the whole provider record")]
        ProviderRequestedRemoval = 2,
        [Description("The service provider no longer exists")]
        ProviderNoLongerExists = 4,
        [Description("The service provider has failed to provide the Secretary of State with information requested in accordance with a notice")]
        FailedToProvideInformation = 5,
        [Description("The Secretary of State is satisfied that the provider is failing to comply with the trust framework")]
        FailingToComplyWithTrustFramework = 6,
        [Description("The Secretary of State is satisfied that the provider is failing to comply with the supplementary code")]
        FailingToComplyWithSupplementaryCode = 7,
        [Description("The Secretary of State considers removal necessary in the interests of national security")]
        NecessaryForNationalSecurity = 8,
        [Description("To help OfDIA maintain accurate record")]
        RemovedByOfDia = 9

    }

    public static class RemovalReasonsEnumExtensions
    {        
        
        public static string GetDescription(this RemovalReasonsEnum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}