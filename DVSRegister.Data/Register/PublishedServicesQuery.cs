using DVSRegister.CommonUtility;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Register;

public sealed class PublishedServicesQuery(DVSRegisterDbContext context) : IPublishedServicesQuery
{
    public async Task<Result<IReadOnlyList<PublishedServiceForContactsReport>>> GetAsync(CancellationToken ct)
    {
        try
        {
            var query = context.Service
                .AsNoTracking()
                .Where(s => s.IsInRegister)
                .OrderBy(s => s.Provider.RegisteredName);

            var services = await query
                .Select(s => new
                {
                    Service = s,
                    s.Provider,
                    CabName = s.CabUser.Cab != null
                        ? s.CabUser.Cab.CabName
                        : null,
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

            return Result<IReadOnlyList<PublishedServiceForContactsReport>>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<PublishedServiceForContactsReport>>.Fail(
                Error.FromException(ex, "PublishedServicesQuery failed"));
        }
    }
}