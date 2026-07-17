using DVSRegister.Data.Register;

namespace DVSRegister.BusinessLogic.Reports.CurrentRegister;

public static class CurrentRegisterWithContactsProjector
{
    public static IReadOnlyList<CurrentRegisterWithContactsRow> Project(
        IEnumerable<PublishedServiceForContactsReport> services)
    {
        return services.Select(ToRow).ToArray();
    }

    private static CurrentRegisterWithContactsRow ToRow(PublishedServiceForContactsReport s)
    {
        return new(
            Provider: s.ProviderRegisteredName,
            ServiceName: s.ServiceName,
            Cab: s.CabName,
            TrustFrameworkVersion: s.TrustFrameworkVersionName,
            SupplementaryCodes: JoinSchemes(s.SchemeNames),
            PrimaryContactFullName: s.PrimaryContactFullName,
            PrimaryContactEmail: s.PrimaryContactEmail,
            SecondaryContactFullName: s.SecondaryContactFullName,
            SecondaryContactEmail: s.SecondaryContactEmail);
    }

    private static string? JoinSchemes(IReadOnlyList<string> names)
    {
        return names is { Count: > 0 } ? string.Join(", ", names) : null;
    }
}