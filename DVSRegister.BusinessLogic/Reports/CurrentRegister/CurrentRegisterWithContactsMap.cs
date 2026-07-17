using CsvHelper.Configuration;

namespace DVSRegister.BusinessLogic.Reports.CurrentRegister;

public sealed class CurrentRegisterWithContactsMap : ClassMap<CurrentRegisterWithContactsRow>
{
    public CurrentRegisterWithContactsMap()
    {
        Map(r => r.Provider).Name("Provider");
        Map(r => r.ServiceName).Name("Service name");
        Map(r => r.Cab).Name("CAB");
        Map(r => r.TrustFrameworkVersion).Name("Trust framework version");
        Map(r => r.SupplementaryCodes).Name("Supplementary codes");

        Map(r => r.PrimaryContactFullName).Name("Primary contact full name");
        Map(r => r.PrimaryContactEmail).Name("Primary contact email address");

        Map(r => r.SecondaryContactFullName).Name("Secondary contact full name");
        Map(r => r.SecondaryContactEmail).Name("Secondary contact email address");
    }
}