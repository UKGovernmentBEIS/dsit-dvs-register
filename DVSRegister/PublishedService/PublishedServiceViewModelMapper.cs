using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;

namespace DVSRegister.PublishedService;

public static class PublishedServiceViewModelMapper
{
    public static PublishedServiceViewModel Map(ServiceDto service) => new(
        ProviderGuid: service.Provider.Guid,
        ServiceGuid: service.ServiceGuid,
        ProviderProfileId: service.ProviderProfileId,
        ServiceName: service.ServiceName,
        LastUpdated: service.LastUpdated,
        ProviderRegisteredName: service.Provider.RegisteredName ?? string.Empty,
        ProviderTradingName: service.Provider.TradingName,
        ProviderPublicContactEmail: service.Provider.PublicContactEmail,
        ProviderTelephoneNumber: service.Provider.ProviderTelephoneNumber,
        ProviderWebsiteAddress: service.Provider.ProviderWebsiteAddress,
        TrustmarkNumber: service.TrustmarkNumber?.TrustMarkNumber,
        TrustmarkLogoLink: service.TrustmarkNumber?.SvgLogoLink,
        TrustFrameworkName: service.TrustFrameworkVersion.TrustFrameworkName,
        TrustFrameworkVersion: service.TrustFrameworkVersion.Version,
        CabName: service.CabUser.Cab.CabName,
        CompanyAddress: service.CompanyAddress,
        Roles: service.ServiceRoleMapping.Select(r => Enum.GetName(typeof(RoleEnum), r.RoleId) ?? string.Empty).ToList(),
        ServiceTypeDisplay: ViewModelHelper.GetServiceType(service.ServiceType),
        IdentityProfiles: GroupProfiles(service.ServiceIdentityProfileMapping?.Select(m => m.IdentityProfile)),
        AuthenticationLevels: Levels(service.ServiceQualityLevelMapping?.Select(m => m.QualityLevel), QualityTypeEnum.Authentication),
        ProtectionLevels: Levels(service.ServiceQualityLevelMapping?.Select(m => m.QualityLevel), QualityTypeEnum.Protection),
        SupplementarySchemes: (service.ServiceSupSchemeMapping ?? []).Select(MapScheme).ToList(),
        ConformityIssueDate: service.ConformityIssueDate,
        ConformityExpiryDate: service.ConformityExpiryDate,
        HasVouchingGuidance: service.HasVouchingGuidance == true,
        UnderpinningService: MapUnderpinning(service.UnderPinningService),
        ManualUnderpinningService: MapManual(service.ManualUnderPinningService));

    private static SupplementarySchemeViewModel MapScheme(ServiceSupSchemeMappingDto mapping)
    {
        var gpg44 = mapping.SchemeGPG44Mapping;
        return new SupplementarySchemeViewModel(
            mapping.SupplementaryScheme.SchemeName,
            GroupProfiles(mapping.SchemeGPG45Mapping?.Select(m => m.IdentityProfile)),
            gpg44 is { Count: > 0 },
            Levels(gpg44?.Select(m => m.QualityLevel), QualityTypeEnum.Authentication),
            Levels(gpg44?.Select(m => m.QualityLevel), QualityTypeEnum.Protection));
    }

    private static List<IdentityProfileGroupViewModel> GroupProfiles(IEnumerable<IdentityProfileDto>? profiles) =>
        (profiles ?? [])
            .GroupBy(p => p.IdentityProfileType)
            .OrderBy(g => g.Key)
            .Select(g => new IdentityProfileGroupViewModel(
                g.Key.ToString(),
                g.Select(p => p.IdentityProfileName).OrderBy(n => n).ToList()))
            .ToList();

    private static List<string> Levels(IEnumerable<QualityLevelDto>? levels, QualityTypeEnum type) =>
        (levels ?? []).Where(l => l.QualityType == type).Select(l => l.Level).ToList();

    private static UnderpinningServiceViewModel? MapUnderpinning(ServiceDto? service) =>
        service is null
            ? null
            : new UnderpinningServiceViewModel(
                service.Provider.Guid,
                service.ServiceGuid,
                service.ServiceName,
                service.Provider.RegisteredName ?? string.Empty,
                service.CabUser.Cab.CabName,
                service.ConformityExpiryDate);

    private static ManualUnderpinningServiceViewModel? MapManual(ManualUnderPinningServiceDto? service) =>
        service is null
            ? null
            : new ManualUnderpinningServiceViewModel(
                service.ServiceName,
                service.ProviderName,
                service.Cab?.CabName,
                service.CertificateExpiryDate);
}
