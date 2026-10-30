using System.Text.RegularExpressions;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Controllers;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabServiceControllerTests : ControllerTestBase<CabServiceController>
    {
        public CabServiceControllerTests()
        {
            ConfigureFakes(() => new CabServiceController(
                CabService, BucketService, UserService, ActionLogService,
                null!, Options.Create(new S3Configuration()), Logger, Mapper));
        }

        [Fact]
        public async Task SaveSupplementarySchemes_RemovingOneScheme_DropsItsSessionMappings()
        {
            var retained = new SupplementarySchemeDto { Id = 1 };
            var removed = new SupplementarySchemeDto { Id = 2 };
            var summary = new ServiceSummaryViewModel
            {
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel
                {
                    SelectedSupplementarySchemes = [retained, removed]
                },
                SchemeIdentityProfileMapping =
                [
                    new SchemeIdentityProfileMappingViewModel { SchemeId = retained.Id },
                    new SchemeIdentityProfileMappingViewModel { SchemeId = removed.Id }
                ],
                SchemeQualityLevelMapping =
                [
                    new SchemeQualityLevelMappingViewModel { SchemeId = retained.Id },
                    new SchemeQualityLevelMappingViewModel { SchemeId = removed.Id }
                ]
            };
            Session.Set("ServiceSummary", summary);
            CabService.GetSupplementarySchemes().Returns([retained, removed]);

            var result = await Controller.SaveSupplementarySchemes(
                new SupplementarySchemeViewModel { SelectedSupplementarySchemeIds = [retained.Id] }, "continue");

            Assert.IsType<RedirectToActionResult>(result);
            var stored = Session.Get<ServiceSummaryViewModel>("ServiceSummary")!;
            Assert.Equal(retained.Id, Assert.Single(stored.SupplementarySchemeViewModel!.SelectedSupplementarySchemes!).Id);
            Assert.Equal(retained.Id, Assert.Single(stored.SchemeIdentityProfileMapping!).SchemeId);
            Assert.Equal(retained.Id, Assert.Single(stored.SchemeQualityLevelMapping!).SchemeId);
        }

        [Fact]
        public async Task SaveHasSupplementarySchemesInput_AnsweringNo_ClearsSchemeSelectionsAndMappings()
        {
            var summary = CreateValidSummary();
            summary.SupplementarySchemeViewModel = new SupplementarySchemeViewModel
            {
                SelectedSupplementarySchemes = [new SupplementarySchemeDto { Id = 1 }]
            };
            summary.SchemeIdentityProfileMapping = [new SchemeIdentityProfileMappingViewModel { SchemeId = 1 }];
            summary.SchemeQualityLevelMapping = [new SchemeQualityLevelMappingViewModel { SchemeId = 1 }];
            Session.Set("ServiceSummary", summary);
            Controller.ModelState.SetModelValue("HasSupplementarySchemes", new ValueProviderResult("false"));

            var result = await Controller.SaveHasSupplementarySchemesInput(
                new ServiceSummaryViewModel { HasSupplementarySchemes = false, FromSummaryPage = true }, "continue");

            Assert.IsType<RedirectToActionResult>(result);
            var stored = Session.Get<ServiceSummaryViewModel>("ServiceSummary")!;
            Assert.Empty(stored.SupplementarySchemeViewModel!.SelectedSupplementarySchemes!);
            Assert.Empty(stored.SchemeIdentityProfileMapping!);
            Assert.Empty(stored.SchemeQualityLevelMapping!);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task SaveServiceSummary_SelectedSchemeMissingRequiredAnswer_DoesNotSave(
            bool hasIdentityAnswer, bool hasQualityAnswer)
        {
            var summary = CreateValidSummary();
            if (!hasIdentityAnswer)
            {
                summary.SchemeIdentityProfileMapping![0].IdentityProfile = null!;
            }
            if (!hasQualityAnswer)
            {
                summary.SchemeQualityLevelMapping![0].HasGPG44 = null;
            }
            Session.Set("ServiceSummary", summary);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.SaveServiceSummary());

            Assert.Equal("SummarySaveFail: All fields must be completed before submitting.", exception.Message);
            await CabService.DidNotReceive().SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<InProgressApplicationParameters?>());
        }

        [Fact]
        public async Task SaveServiceSummary_SelectedSchemeGpg44AnswerNo_DoesNotRequireQualitySelections()
        {
            var summary = CreateValidSummary();
            summary.SchemeQualityLevelMapping![0].QualityLevel = null!;

            var result = await SubmitValidSummary(summary);

            Assert.IsType<RedirectToActionResult>(result);
            await CabService.Received(1).SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<InProgressApplicationParameters?>());
        }

        [Fact]
        public async Task SaveServiceSummary_SelectedSchemeGpg44AnswerYes_RequiresBothQualitySelections()
        {
            var summary = CreateValidSummary();
            summary.SchemeQualityLevelMapping![0].HasGPG44 = true;
            summary.SchemeQualityLevelMapping[0].QualityLevel = new QualityLevelViewModel
            {
                SelectedLevelOfProtections = [],
                SelectedQualityofAuthenticators = null
            };
            Session.Set("ServiceSummary", summary);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.SaveServiceSummary());

            Assert.Equal("SummarySaveFail: All fields must be completed before submitting.", exception.Message);
            await CabService.DidNotReceive().SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<InProgressApplicationParameters?>());
        }

        [Fact]
        public async Task SaveServiceSummary_Tf03_DoesNotRequireTf04SchemeAnswers()
        {
            var summary = CreateValidSummary();
            summary.TFVersionViewModel!.SelectedTFVersion!.Version = 0.3m;
            summary.ServiceType = null;
            summary.SchemeIdentityProfileMapping = [];
            summary.SchemeQualityLevelMapping = [];
            Session.Set("ServiceSummary", summary);

            var result = await SubmitValidSummary(summary);

            Assert.IsType<RedirectToActionResult>(result);
            await CabService.Received(1).SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<InProgressApplicationParameters?>());
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("terms.pdf", null)]
        public async Task SaveServiceSummary_Tf10_RequiresTermsOfUseAndVouchingAnswer(
            string? termsOfUseFileName, bool? hasVouchingGuidance)
        {
            var summary = CreateValidSummary();
            summary.TFVersionViewModel!.SelectedTFVersion!.Version = Constants.TFVersion1_0;
            summary.TOUFileName = termsOfUseFileName;
            summary.HasVouchingGuidance = hasVouchingGuidance;
            Session.Set("ServiceSummary", summary);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.SaveServiceSummary());

            Assert.Equal("SummarySaveFail: All fields must be completed before submitting.", exception.Message);
            await CabService.DidNotReceive().SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<InProgressApplicationParameters?>());
        }

        [Fact]
        public async Task SaveServiceSummary_StaleMappingsForDeselectedScheme_AllowsSubmission()
        {
            var selected = new SupplementarySchemeDto { Id = 1 };
            var summary = new ServiceSummaryViewModel
            {
                CabId = 123,
                ServiceId = 7,
                ServiceName = "Service",
                ServiceURL = "https://example.com",
                CompanyAddress = "Address",
                IsReupload = true,
                IsResubmission = true,
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
                    SelectedSupplementarySchemes = [selected]
                },
                SchemeIdentityProfileMapping =
                [
                    new SchemeIdentityProfileMappingViewModel
                    {
                        SchemeId = selected.Id,
                        IdentityProfile = new IdentityProfileViewModel { SelectedIdentityProfiles = [] }
                    },
                    new SchemeIdentityProfileMappingViewModel { SchemeId = 2 }
                ],
                SchemeQualityLevelMapping =
                [
                    new SchemeQualityLevelMappingViewModel { SchemeId = selected.Id, HasGPG44 = false },
                    new SchemeQualityLevelMappingViewModel { SchemeId = 2, HasGPG44 = false }
                ],
                ConformityIssueDate = DateTime.Today.AddYears(-1),
                ConformityExpiryDate = DateTime.Today.AddYears(1)
            };
            Session.Set("ServiceSummary", summary);
            Mapper.Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>()).Returns(new ServiceDto());
            CabService.SaveServiceReApplication(
                    Arg.Any<ServiceDto>(), Arg.Any<string>(), true, Arg.Any<InProgressApplicationParameters?>())
                .Returns(new GenericResponse { Success = true, InstanceId = 42 });
            CabService.GetServiceDetailsWithProvider(42, 123).Returns(new ServiceDto
            {
                Id = 42,
                Provider = new ProviderProfileDto { RegisteredName = "Provider" }
            });

            var result = await Controller.SaveServiceSummary();

            Assert.IsType<RedirectToActionResult>(result);
            await CabService.Received(1).SaveServiceReApplication(
                Arg.Any<ServiceDto>(), Arg.Any<string>(), true, Arg.Any<InProgressApplicationParameters?>());
        }

        private ServiceSummaryViewModel CreateValidSummary()
        {
            var scheme = new SupplementarySchemeDto { Id = 1 };
            return new ServiceSummaryViewModel
            {
                CabId = 123,
                ServiceId = 7,
                ServiceName = "Service",
                ServiceURL = "https://example.com",
                CompanyAddress = "Address",
                IsReupload = true,
                IsResubmission = true,
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
                    SelectedSupplementarySchemes = [scheme]
                },
                SchemeIdentityProfileMapping =
                [
                    new SchemeIdentityProfileMappingViewModel
                    {
                        SchemeId = scheme.Id,
                        IdentityProfile = new IdentityProfileViewModel { SelectedIdentityProfiles = [] }
                    }
                ],
                SchemeQualityLevelMapping =
                [
                    new SchemeQualityLevelMappingViewModel { SchemeId = scheme.Id, HasGPG44 = false }
                ],
                ConformityIssueDate = DateTime.Today.AddYears(-1),
                ConformityExpiryDate = DateTime.Today.AddYears(1)
            };
        }

        private async Task<IActionResult> SubmitValidSummary(ServiceSummaryViewModel summary)
        {
            Session.Set("ServiceSummary", summary);
            Mapper.Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>()).Returns(new ServiceDto());
            CabService.SaveServiceReApplication(
                    Arg.Any<ServiceDto>(), Arg.Any<string>(), true, Arg.Any<InProgressApplicationParameters?>())
                .Returns(new GenericResponse { Success = true, InstanceId = 42 });
            CabService.GetServiceDetailsWithProvider(42, 123).Returns(new ServiceDto
            {
                Id = 42,
                Provider = new ProviderProfileDto { Id = 123, RegisteredName = "Provider" }
            });

            return await Controller.SaveServiceSummary();
        }

        [Theory]
        [InlineData("ValidString123", true)]
        [InlineData("Another Valid String", true)]
        [InlineData("Name & Address", true)]
        [InlineData("User@Name#123", true)]
        [InlineData("Test-String: Example", true)]
        [InlineData("Special_Characters: ()", true)]
        [InlineData("John Doe's Profile", true)]
        [InlineData("Example_123: Test", true)]
        [InlineData("Company Name @2023", true)]
        [InlineData("Valid-Input_Example", true)]
        [InlineData("Invalid String!", false)]
        [InlineData("Invalid#String$", false)]
        [InlineData("Another Invalid String*", false)]
        [InlineData("Invalid String?", false)]
        [InlineData("Invalid String;", false)]
        [InlineData("Invalid String: <script>", false)]
        [InlineData("Invalid String%", false)]
        [InlineData("Invalid String@#^", false)]
        [InlineData("123 Invalid String!", false)]

        public void ServiceNameAcceptedCharacters_Test(string input, bool expectedIsValid)
        {
            string pattern = @"^[A-Za-z0-9 &@#().:_'-]+$";
            var isValid = Regex.IsMatch(input, pattern);
            Assert.Equal(isValid, expectedIsValid);

        }
    }
}
