namespace DVSRegister.BusinessLogic.Reports.CurrentRegister;

public sealed record CurrentRegisterWithContactsRow(
    string Provider,
    string ServiceName,
    string Cab,
    string TrustFrameworkVersion,
    string? SupplementaryCodes,
    string? PrimaryContactFullName,
    string? PrimaryContactEmail,
    string? SecondaryContactFullName,
    string? SecondaryContactEmail);