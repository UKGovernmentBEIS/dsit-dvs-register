using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.PublishedService;

public sealed class PublishedServiceController(IGetPublishedService getPublishedService) : Controller
{
    [HttpGet("~/organisation/{providerGuid:guid}/service/{serviceGuid:guid}")]
    public IActionResult Bare(Guid providerGuid, Guid serviceGuid) =>
        Redirect(ServiceLinks.Latest(providerGuid, serviceGuid));

    [HttpGet("~/organisation/{providerGuid:guid}/service/{serviceGuid:guid}/version/latest")]
    public async Task<IActionResult> Latest(Guid providerGuid, Guid serviceGuid, CancellationToken ct)
    {
        var result = await getPublishedService.HandleAsync(providerGuid, serviceGuid, ct);

        return result.Match(
            model => View("Index", model with
            {
                PreviousServiceUrl = PublishedServiceNavigation.GetBackUrl(Request, model.UnderpinningService)
            }),
            PublishedServiceActionResult.FromError);
    }
}
