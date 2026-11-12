using DVSRegister.CommonUtility;

namespace DVSRegister.PublishedService;

public interface IGetPublishedService
{
    Task<Result<PublishedServiceViewModel>> HandleAsync(Guid providerGuid, Guid serviceGuid, CancellationToken ct);
}
