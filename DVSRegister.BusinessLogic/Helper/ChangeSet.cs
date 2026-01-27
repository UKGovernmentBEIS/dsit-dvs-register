namespace DVSRegister.CommonUtility.Models
{
    public sealed record ChangeSet
    (
       Dictionary<string, List<string>> Current,
       Dictionary<string, List<string>> Previous
    );
}
