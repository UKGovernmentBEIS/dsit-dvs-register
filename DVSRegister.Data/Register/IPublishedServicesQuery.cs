using DVSRegister.CommonUtility;

namespace DVSRegister.Data.Register;

public interface IPublishedServicesQuery
{
    Task<Result<IReadOnlyList<PublishedServiceForContactsReport>>> GetAsync(CancellationToken ct);
}