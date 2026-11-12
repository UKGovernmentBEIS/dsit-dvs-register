using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.PublishedService;

public static class PublishedServiceActionResult
{
    public static IActionResult FromError(Error error) =>
        error.Code == PublishedServiceErrorCodes.NotFound
            ? new RedirectToActionResult("RegisterPageNotFound", "Error", null)
            : throw new InvalidOperationException($"Published service lookup failed ({error.Code}): {error.Message}");
}
