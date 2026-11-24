using DVSRegister.BusinessLogic.Services.Register;
using DVSRegister.CommonUtility;
using DVSRegister.Data;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.PublishedService;

public sealed class GetPublishedService(DVSRegisterDbContext dbContext, IRegisterService registerService)
    : IGetPublishedService
{
    public async Task<Result<PublishedServiceViewModel>> HandleAsync(
        Guid providerGuid, Guid serviceGuid, CancellationToken ct)
    {
        // More than one published row means the data is ambiguous, so treat it as not found
        var ids = await dbContext.Service.AsNoTracking()
            .Where(s => s.IsInRegister && s.ServiceGuid == serviceGuid && s.Provider.Guid == providerGuid)
            .Select(s => s.Id)
            .Take(2)
            .ToListAsync(ct);

        if (ids.Count != 1)
            return Result<PublishedServiceViewModel>.Fail(NotFound());

        var service = await registerService.GetServiceDetails(ids[0]);
        return service is null
            ? Result<PublishedServiceViewModel>.Fail(NotFound())
            : Result<PublishedServiceViewModel>.Ok(PublishedServiceViewModelMapper.Map(service));
    }

    private static Error NotFound() =>
        Error.Custom(PublishedServiceErrorCodes.NotFound, "The service could not be found.");
}
