using DVSRegister.CommonUtility;

namespace DVSRegister.PublishedService;

public static class PublishedServiceNavigation
{
    // Same-host referer that is another published service page, used for the Back link
    public static string? GetPreviousServiceUrl(HttpRequest request)
    {
        if (!Uri.TryCreate(request.Headers.Referer.ToString(), UriKind.Absolute, out var referer))
            return null;

        var sameHost = string.Equals(referer.Authority, request.Host.Value, StringComparison.OrdinalIgnoreCase);
        var isServicePage = referer.AbsolutePath.StartsWith("/organisation/", StringComparison.OrdinalIgnoreCase)
                            && referer.AbsolutePath.EndsWith("/version/latest", StringComparison.OrdinalIgnoreCase);
        var isSamePage = string.Equals(referer.AbsolutePath, request.Path.Value, StringComparison.OrdinalIgnoreCase);

        return sameHost && isServicePage && !isSamePage ? referer.AbsolutePath : null;
    }

    public static string? GetBackUrl(HttpRequest request, UnderpinningServiceViewModel? underpinning)
    {
        var previous = GetPreviousServiceUrl(request);
        if (previous is null || underpinning is null)
            return previous;

        // Came back from our own underpinning service, so don't link forward again
        var underpinningPath = ServiceLinks.Latest(underpinning.ProviderGuid, underpinning.ServiceGuid);
        return string.Equals(previous, underpinningPath, StringComparison.OrdinalIgnoreCase) ? null : previous;
    }
}
