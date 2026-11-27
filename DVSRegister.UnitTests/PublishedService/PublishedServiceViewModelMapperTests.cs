using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.PublishedService;
using DVSRegister.UnitTests.Helpers;

namespace DVSRegister.UnitTests.PublishedService
{
    public class PublishedServiceViewModelMapperTests
    {
        private static ServiceDto CreateDto(Guid providerGuid, Guid serviceGuid) =>
            TestDataFactory.CreatePublishedServiceDto(providerGuid, "Provider Ltd", 10, 20, serviceGuid, "Service");

        [Fact]
        public void Map_BasicService_CopiesStableGuidsAndProviderDetails()
        {
            var providerGuid = Guid.NewGuid();
            var serviceGuid = Guid.NewGuid();
            var dto = CreateDto(providerGuid, serviceGuid);
            dto.ServiceRoleMapping = [new() { RoleId = (int)RoleEnum.Identity }, new() { RoleId = (int)RoleEnum.Holder }];

            var model = PublishedServiceViewModelMapper.Map(dto);

            Assert.Equal(providerGuid, model.ProviderGuid);
            Assert.Equal(serviceGuid, model.ServiceGuid);
            Assert.Equal("Provider Ltd", model.ProviderRegisteredName);
            Assert.Equal("Test CAB", model.CabName);
            Assert.Equal(["Identity", "Holder"], model.Roles);
            Assert.Null(model.UnderpinningService);
            Assert.Null(model.ManualUnderpinningService);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Map_MissingOptionalProviderFields_ShowNotApplicable(string? missing)
        {
            var dto = CreateDto(Guid.NewGuid(), Guid.NewGuid());
            dto.Provider.TradingName = missing;
            dto.Provider.PublicContactEmail = missing;
            dto.Provider.ProviderTelephoneNumber = missing;

            var model = PublishedServiceViewModelMapper.Map(dto);

            Assert.Equal(Constants.NullFieldsDisplay, model.ProviderTradingName);
            Assert.Equal(Constants.NullFieldsDisplay, model.ProviderPublicContactEmail);
            Assert.Equal(Constants.NullFieldsDisplay, model.ProviderTelephoneNumber);
        }

        [Fact]
        public void Map_IdentityProfilesAndLevels_AreGroupedByTypeAndQualityType()
        {
            var dto = CreateDto(Guid.NewGuid(), Guid.NewGuid());
            dto.ServiceIdentityProfileMapping =
            [
                new() { IdentityProfile = new() { IdentityProfileName = "M1B", IdentityProfileType = IdentityProfileTypeEnum.Medium } },
                new() { IdentityProfile = new() { IdentityProfileName = "L1A", IdentityProfileType = IdentityProfileTypeEnum.Low } },
                new() { IdentityProfile = new() { IdentityProfileName = "M1A", IdentityProfileType = IdentityProfileTypeEnum.Medium } }
            ];
            dto.ServiceQualityLevelMapping =
            [
                new() { QualityLevel = new() { Level = "Low", QualityType = QualityTypeEnum.Authentication } },
                new() { QualityLevel = new() { Level = "Enhanced", QualityType = QualityTypeEnum.Protection } }
            ];

            var model = PublishedServiceViewModelMapper.Map(dto);

            Assert.Equal(["Low", "Medium"], model.IdentityProfiles.Select(g => g.Type));
            Assert.Equal(["M1A", "M1B"], model.IdentityProfiles[1].Names);
            Assert.Equal(["Low"], model.AuthenticationLevels);
            Assert.Equal(["Enhanced"], model.ProtectionLevels);
        }

        [Fact]
        public void Map_UnderpinningServices_UseStableGuids()
        {
            var underpinningProviderGuid = Guid.NewGuid();
            var underpinningServiceGuid = Guid.NewGuid();
            var dto = CreateDto(Guid.NewGuid(), Guid.NewGuid());
            dto.UnderPinningService = CreateDto(underpinningProviderGuid, underpinningServiceGuid);
            dto.ManualUnderPinningService = new ManualUnderPinningServiceDto
            {
                ServiceName = "Manual service",
                ProviderName = "Manual provider",
                Cab = new CabDto { CabName = "Manual CAB" }
            };

            var model = PublishedServiceViewModelMapper.Map(dto);

            Assert.Equal(underpinningProviderGuid, model.UnderpinningService!.ProviderGuid);
            Assert.Equal(underpinningServiceGuid, model.UnderpinningService.ServiceGuid);
            Assert.Equal("Manual CAB", model.ManualUnderpinningService!.CabName);
        }
    }
}
