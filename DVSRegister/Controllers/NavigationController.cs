using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class NavigationController : Controller
{
    private const string SessionKey = "NavStack";

    [HttpGet("previous-page")]
    public IActionResult Back()
    {
        var session = HttpContext.Session;

        var json = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
            return RedirectToAction("DraftApplications", "Home");

        var stack = JsonSerializer.Deserialize<List<string>>(json);

        if (stack.Count <= 1)
            return RedirectToAction("DraftApplications", "Home");

        stack.RemoveAt(stack.Count - 1);

        var previous = stack.Last();

        session.SetString(SessionKey, JsonSerializer.Serialize(stack));

        return Redirect(previous);
    }
}
