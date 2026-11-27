using DVSRegister.CommonUtility;

namespace DVSRegister.PublishedService;

/// <summary>Pure formatting of published service values for display.</summary>
public static class PublishedServiceDisplay
{
    public static string Text(string? value) =>
        string.IsNullOrWhiteSpace(value) ? Constants.NullFieldsDisplay : value;
}
