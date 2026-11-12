using Microsoft.AspNetCore.Mvc.Razor;

namespace DVSRegister.PublishedService;

/// <summary>
///     Resolves co-located views as /PublishedService/{Action}.cshtml for PublishedServiceController.
/// </summary>
public sealed class PublishedServiceViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations) =>
        context.ActionContext.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller)
        && controller == "PublishedService"
            ? ["/PublishedService/{0}.cshtml", .. viewLocations]
            : viewLocations;
}
