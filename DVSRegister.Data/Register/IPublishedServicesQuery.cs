namespace DVSRegister.Data.Register;

public interface IPublishedServicesQuery
{
    Task<IReadOnlyList<PublishedServiceForContactsReport>> GetAsync(CancellationToken ct);
}