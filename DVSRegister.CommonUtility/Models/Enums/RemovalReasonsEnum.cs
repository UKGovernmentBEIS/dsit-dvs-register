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
        NecessaryForNationalSecurity = 8

    }

    public static class RemovalReasonsEnumExtensions
    {
        private static readonly Dictionary<RemovalReasonsEnum, bool> RequiresAdditionalInfoMap = new Dictionary<RemovalReasonsEnum, bool>
        {
            { RemovalReasonsEnum.RemovedByCronJob, false },
            { RemovalReasonsEnum.ProviderRequestedRemoval, false },
            { RemovalReasonsEnum.ProviderNoLongerExists, false },
            { RemovalReasonsEnum.FailedToProvideInformation, false },
            { RemovalReasonsEnum.FailingToComplyWithTrustFramework, false },
            { RemovalReasonsEnum.FailingToComplyWithSupplementaryCode, false },
            { RemovalReasonsEnum.NecessaryForNationalSecurity, false }

        };

        private static readonly Dictionary<RemovalReasonsEnum, string> ContactInfoMap = new Dictionary<RemovalReasonsEnum, string>
        {
            { RemovalReasonsEnum.RemovedByCronJob, "DSIT" },
            { RemovalReasonsEnum.ProviderRequestedRemoval, "Provider" },
            { RemovalReasonsEnum.ProviderNoLongerExists, "DSIT" },
            { RemovalReasonsEnum.FailedToProvideInformation, "DSIT" },
            { RemovalReasonsEnum.FailingToComplyWithTrustFramework, "DSIT" },
            { RemovalReasonsEnum.FailingToComplyWithSupplementaryCode, "DSIT" },
            { RemovalReasonsEnum.NecessaryForNationalSecurity, "DSIT" }

        };

        public static bool RequiresAdditionalInfo(this RemovalReasonsEnum reason)
        {
            return RequiresAdditionalInfoMap.TryGetValue(reason, out var requiresAdditionalInfo) && requiresAdditionalInfo;
        }

        public static string GetContactInfo(this RemovalReasonsEnum reason)
        {
            return ContactInfoMap.TryGetValue(reason, out var contactInfo) ? contactInfo : "Unknown";
        }

        public static string GetDescription(this RemovalReasonsEnum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

