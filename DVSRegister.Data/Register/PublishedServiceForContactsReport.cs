namespace DVSRegister.Data.Register;

public sealed record PublishedServiceForContactsReport(
    string ProviderRegisteredName,
    string ServiceName,
    string CabName,
    string TrustFrameworkVersionName,
    IReadOnlyList<string> SchemeNames,
    string? PrimaryContactFullName,
    string? PrimaryContactEmail,
    string? SecondaryContactFullName,
    string? SecondaryContactEmail);