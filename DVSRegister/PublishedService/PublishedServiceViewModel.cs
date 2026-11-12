namespace DVSRegister.PublishedService;

public sealed record PublishedServiceViewModel(
    Guid ProviderGuid,
    Guid ServiceGuid,
    int ProviderProfileId,
    string ServiceName,
    DateTime? LastUpdated,
    string ProviderRegisteredName,
    string? ProviderTradingName,
    string? ProviderPublicContactEmail,
    string? ProviderTelephoneNumber,
    string? ProviderWebsiteAddress,
    string? TrustmarkNumber,
    string? TrustmarkLogoLink,
    string TrustFrameworkName,
    decimal TrustFrameworkVersion,
    string CabName,
    string? CompanyAddress,
    IReadOnlyList<string> Roles,
    string ServiceTypeDisplay,
    IReadOnlyList<IdentityProfileGroupViewModel> IdentityProfiles,
    IReadOnlyList<string> AuthenticationLevels,
    IReadOnlyList<string> ProtectionLevels,
    IReadOnlyList<SupplementarySchemeViewModel> SupplementarySchemes,
    DateTime ConformityIssueDate,
    DateTime ConformityExpiryDate,
    bool HasVouchingGuidance,
    UnderpinningServiceViewModel? UnderpinningService,
    ManualUnderpinningServiceViewModel? ManualUnderpinningService)
{
    public string? PreviousServiceUrl { get; init; }
}

public sealed record IdentityProfileGroupViewModel(string Type, IReadOnlyList<string> Names);

public sealed record SupplementarySchemeViewModel(
    string Name,
    IReadOnlyList<IdentityProfileGroupViewModel> IdentityProfiles,
    bool HasGpg44,
    IReadOnlyList<string> AuthenticationLevels,
    IReadOnlyList<string> ProtectionLevels);

public sealed record UnderpinningServiceViewModel(
    Guid ProviderGuid,
    Guid ServiceGuid,
    string ServiceName,
    string ProviderName,
    string CabName,
    DateTime ConformityExpiryDate);

public sealed record ManualUnderpinningServiceViewModel(
    string? ServiceName,
    string? ProviderName,
    string? CabName,
    DateTime? CertificateExpiryDate);
