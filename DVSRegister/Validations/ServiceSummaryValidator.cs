using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CAB;

namespace DVSRegister.Validations
{
    public static class ServiceSummaryValidator
    {
        public static bool HasAllRequiredData(ServiceSummaryViewModel? summary)
        {
            if (summary?.TFVersionViewModel?.SelectedTFVersion is not { } selectedVersion)
                return false;

            return HasSharedRequiredData(summary)
                && HasTrustFramework04Data(summary, selectedVersion.Version)
                && HasTrustFramework10Data(summary, selectedVersion.Version);
        }

        private static bool HasSharedRequiredData(ServiceSummaryViewModel summary)
        {
            return !string.IsNullOrWhiteSpace(summary.ServiceName)
                && !string.IsNullOrWhiteSpace(summary.ServiceURL)
                && !string.IsNullOrWhiteSpace(summary.CompanyAddress)
                && summary.RoleViewModel?.SelectedRoles != null
                && summary.ConformityIssueDate != null
                && summary.ConformityExpiryDate != null
                && HasGpg44Data(summary)
                && HasGpg45Data(summary)
                && HasSupplementarySchemeAnswer(summary);
        }

        private static bool HasGpg44Data(ServiceSummaryViewModel summary)
        {
            return summary.HasGPG44.HasValue
                && (!summary.HasGPG44.Value
                    || (summary.QualityLevelViewModel?.SelectedLevelOfProtections != null
                        && summary.QualityLevelViewModel.SelectedQualityofAuthenticators != null));
        }

        private static bool HasGpg45Data(ServiceSummaryViewModel summary)
        {
            return summary.HasGPG45.HasValue
                && (!summary.HasGPG45.Value
                    || summary.IdentityProfileViewModel?.SelectedIdentityProfiles != null);
        }

        private static bool HasSupplementarySchemeAnswer(ServiceSummaryViewModel summary)
        {
            return summary.HasSupplementarySchemes.HasValue
                && (!summary.HasSupplementarySchemes.Value
                    || summary.SupplementarySchemeViewModel?.SelectedSupplementarySchemes != null);
        }

        private static bool HasTrustFramework04Data(ServiceSummaryViewModel summary, decimal version)
        {
            if (version < Constants.TFVersion0_4)
                return true;

            return HasServiceTypeData(summary) && HasSelectedSchemeAnswers(summary);
        }

        private static bool HasServiceTypeData(ServiceSummaryViewModel summary)
        {
            return summary.ServiceType.HasValue
                && (summary.ServiceType != ServiceTypeEnum.WhiteLabelled
                    || (summary.IsUnderpinningServicePublished == true
                        && summary.SelectedUnderPinningServiceId != null)
                    || (summary.IsUnderpinningServicePublished == false
                        && summary.SelectedManualUnderPinningServiceId != null)
                    || (summary.IsUnderpinningServicePublished == false
                        && !string.IsNullOrWhiteSpace(summary.UnderPinningProviderName)
                        && !string.IsNullOrWhiteSpace(summary.UnderPinningServiceName)
                        && summary.UnderPinningServiceExpiryDate != null
                        && !string.IsNullOrWhiteSpace(summary.SelectCabViewModel?.SelectedCabName)));
        }

        private static bool HasSelectedSchemeAnswers(ServiceSummaryViewModel summary)
        {
            if (!summary.HasSupplementarySchemes.GetValueOrDefault())
                return true;

            var selectedSchemes = summary.SupplementarySchemeViewModel?.SelectedSupplementarySchemes;
            return selectedSchemes != null
                && selectedSchemes.All(scheme => HasSchemeIdentityProfileAnswer(summary, scheme.Id)
                    && HasSchemeQualityLevelAnswer(summary, scheme.Id));
        }

        private static bool HasSchemeIdentityProfileAnswer(ServiceSummaryViewModel summary, int schemeId)
        {
            return summary.SchemeIdentityProfileMapping?.Any(mapping =>
                mapping.SchemeId == schemeId
                && mapping.IdentityProfile?.SelectedIdentityProfiles != null) == true;
        }

        private static bool HasSchemeQualityLevelAnswer(ServiceSummaryViewModel summary, int schemeId)
        {
            return summary.SchemeQualityLevelMapping?.Any(mapping =>
                mapping.SchemeId == schemeId
                && mapping.HasGPG44.HasValue
                && (!mapping.HasGPG44.Value
                    || (mapping.QualityLevel?.SelectedLevelOfProtections != null
                        && mapping.QualityLevel.SelectedQualityofAuthenticators != null))) == true;
        }

        private static bool HasTrustFramework10Data(ServiceSummaryViewModel summary, decimal version)
        {
            return version != Constants.TFVersion1_0
                || (summary.TOUFileName != null && summary.HasVouchingGuidance.HasValue);
        }
    }
}
