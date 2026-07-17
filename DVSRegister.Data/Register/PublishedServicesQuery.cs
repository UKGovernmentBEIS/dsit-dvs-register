using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Register;

public sealed class PublishedServicesQuery(DVSRegisterDbContext context) : IPublishedServicesQuery
{
    public async Task<IReadOnlyList<PublishedServiceForContactsReport>> GetAsync(CancellationToken ct)
    {
        var query = context.Service
            .AsNoTracking()
            .Where(s => s.IsInRegister == true)
            .OrderBy(s => s.Provider.RegisteredName);

        var services = await query
            .Select(s => new
            {
                Service = s,
                Provider = s.Provider,
                CabName = s.CabUser.Cab != null
                    ? s.CabUser.Cab.CabName
                    : (string?)null,
                TrustFrameworkVersionName = s.TrustFrameworkVersion.TrustFrameworkName,
                Schemes = s.ServiceSupSchemeMapping
                    .Where(ssm =>
                        ssm.ServiceSupSchemeCustomDisplayId == null
                        && ssm.SupplementaryScheme != null)
                    .Select(ssm => ssm.SupplementaryScheme!.SchemeName)
                    .ToList()
            })
            .ToListAsync(ct);

        var result = services
            .Select(s => new PublishedServiceForContactsReport(
                ProviderRegisteredName: s.Provider.RegisteredName,
                ServiceName: s.Service.ServiceName ?? string.Empty,
                CabName: s.CabName ?? string.Empty,
                TrustFrameworkVersionName: s.TrustFrameworkVersionName ?? string.Empty,
                SchemeNames: s.Schemes,
                PrimaryContactFullName: s.Provider.PrimaryContactFullName,
                PrimaryContactEmail: s.Provider.PrimaryContactEmail,
                SecondaryContactFullName: s.Provider.SecondaryContactFullName,
                SecondaryContactEmail: s.Provider.SecondaryContactEmail
            ))
            .ToArray();

        return result;
    }
}