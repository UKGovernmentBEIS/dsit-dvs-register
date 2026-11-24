using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.PublishedService;

public sealed class ResolveLegacyServiceLink(DVSRegisterDbContext dbContext) : IResolveLegacyServiceLink
{
    public async Task<Result<string>> HandleAsync(int id, CancellationToken ct)
    {
        // Only published rows (or removed ones, which are no longer in the register) may redirect,
        // so a never-published version (for example a submitted v3) does not reveal that it exists
        var row = await dbContext.Service.AsNoTracking()
            .Where(s => s.Id == id && (s.IsInRegister || s.ServiceStatus == ServiceStatusEnum.Removed))
            .Select(s => new { s.ServiceGuid, s.ProviderProfileId, ProviderGuid = s.Provider.Guid })
            .SingleOrDefaultAsync(ct);

        if (row is null)
            return NotFound();

        // Only redirect when exactly one published row exists for this service
        var publishedCount = await dbContext.Service.AsNoTracking()
            .CountAsync(s => s.IsInRegister && s.ServiceGuid == row.ServiceGuid
                          && s.ProviderProfileId == row.ProviderProfileId, ct);

        return publishedCount == 1
            ? Result<string>.Ok(ServiceLinks.Latest(row.ProviderGuid, row.ServiceGuid))
            : NotFound();
    }

    private static Result<string> NotFound() =>
        Result<string>.Fail(Error.Custom(PublishedServiceErrorCodes.NotFound, "The service could not be found."));
}
