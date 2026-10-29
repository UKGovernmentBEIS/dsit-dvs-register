using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Validations;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class ServiceSummaryValidatorTests
    {
        [Fact]
        public void HasAllRequiredData_NullSummary_ReturnsFalse()
        {
            Assert.False(ServiceSummaryValidator.HasAllRequiredData(null));
        }

        [Fact]
        public void HasAllRequiredData_MissingTrustFrameworkVersion_ReturnsFalse()
        {
            var summary = CreateValidSummary();
            summary.TFVersionViewModel = null;

            Assert.False(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_OrphanedMappingsForDeselectedSchemes_AreIgnored()
        {
            var summary = CreateValidSummary();
            summary.SchemeIdentityProfileMapping!.Add(
                new SchemeIdentityProfileMappingViewModel { SchemeId = 2 });
            summary.SchemeQualityLevelMapping!.Add(
                new SchemeQualityLevelMappingViewModel { SchemeId = 2 });

            Assert.True(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_MappingsOnlyForDeselectedScheme_ReturnsFalse()
        {
            var summary = CreateValidSummary();
            summary.SchemeIdentityProfileMapping =
            [
                new SchemeIdentityProfileMappingViewModel
                {
                    SchemeId = 2,
                    IdentityProfile = new IdentityProfileViewModel { SelectedIdentityProfiles = [] }
                }
            ];
            summary.SchemeQualityLevelMapping =
            [
                new SchemeQualityLevelMappingViewModel { SchemeId = 2, HasGPG44 = false }
            ];

            Assert.False(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void HasAllRequiredData_SelectedSchemeMissingAnAnswer_ReturnsFalse(
            bool hasIdentityAnswer, bool hasQualityAnswer)
        {
            var summary = CreateValidSummary();
            if (!hasIdentityAnswer)
                summary.SchemeIdentityProfileMapping![0].IdentityProfile = null!;
            if (!hasQualityAnswer)
                summary.SchemeQualityLevelMapping![0].HasGPG44 = null;

            Assert.False(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_SchemeGpg44No_DoesNotRequireQualitySelections()
        {
            var summary = CreateValidSummary();
            summary.SchemeQualityLevelMapping![0].QualityLevel = null!;

            Assert.True(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_SchemeGpg44Yes_RequiresBothQualitySelections()
        {
            var summary = CreateValidSummary();
            summary.SchemeQualityLevelMapping![0].HasGPG44 = true;
            summary.SchemeQualityLevelMapping[0].QualityLevel = new QualityLevelViewModel
            {
                SelectedLevelOfProtections = [],
                SelectedQualityofAuthenticators = null
            };

            Assert.False(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_SchemeGpg44Yes_WithBothQualitySelections_ReturnsTrue()
        {
            var summary = CreateValidSummary();
            summary.SchemeQualityLevelMapping![0].HasGPG44 = true;
            summary.SchemeQualityLevelMapping[0].QualityLevel = new QualityLevelViewModel
            {
                SelectedLevelOfProtections = [],
                SelectedQualityofAuthenticators = []
            };

            Assert.True(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Fact]
        public void HasAllRequiredData_Tf03_DoesNotRequireTf04ServiceOrSchemeData()
        {
            var summary = CreateValidSummary();
            summary.TFVersionViewModel!.SelectedTFVersion!.Version = 0.3m;
            summary.ServiceType = null;
            summary.SchemeIdentityProfileMapping = [];
            summary.SchemeQualityLevelMapping = [];

            Assert.True(ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("terms.pdf", null)]
        [InlineData("terms.pdf", false)]
        public void HasAllRequiredData_Tf10_RequiresTermsAndVouchingAnswer(
            string? termsOfUseFileName, bool? hasVouchingGuidance)
        {
            var summary = CreateValidSummary();
            summary.TFVersionViewModel!.SelectedTFVersion!.Version = Constants.TFVersion1_0;
            summary.TOUFileName = termsOfUseFileName;
            summary.HasVouchingGuidance = hasVouchingGuidance;

            Assert.Equal(
                termsOfUseFileName != null && hasVouchingGuidance.HasValue,
                ServiceSummaryValidator.HasAllRequiredData(summary));
        }

        private static ServiceSummaryViewModel CreateValidSummary()
        {
            const int schemeId = 1;
            return new ServiceSummaryViewModel
            {
                ServiceName = "Service",
                ServiceURL = "https://example.com",
                CompanyAddress = "Address",
                RoleViewModel = new RoleViewModel { SelectedRoles = [] },
                TFVersionViewModel = new TFVersionViewModel
                {
                    SelectedTFVersion = new TrustFrameworkVersionDto { Version = Constants.TFVersion0_4 }
                },
                ServiceType = ServiceTypeEnum.Neither,
                HasGPG44 = false,
                HasGPG45 = false,
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = [] },
                HasSupplementarySchemes = true,
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel
                {
                    SelectedSupplementarySchemes = [new SupplementarySchemeDto { Id = schemeId }]
                },
                SchemeIdentityProfileMapping =
                [
                    new SchemeIdentityProfileMappingViewModel
                    {
                        SchemeId = schemeId,
                        IdentityProfile = new IdentityProfileViewModel { SelectedIdentityProfiles = [] }
                    }
                ],
                SchemeQualityLevelMapping =
                [
                    new SchemeQualityLevelMappingViewModel { SchemeId = schemeId, HasGPG44 = false }
                ],
                ConformityIssueDate = DateTime.Today.AddYears(-1),
                ConformityExpiryDate = DateTime.Today.AddYears(1)
            };
        }
    }
}
