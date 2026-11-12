using DVSRegister.CommonUtility;

namespace DVSRegister.PublishedService;

public interface IResolveLegacyServiceLink
{
    Task<Result<string>> HandleAsync(int id, CancellationToken ct);
}
