using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

public class NavigationTrackingFilter : IActionFilter
{
    private const string SessionKey = "NavStack";

    public void OnActionExecuted(ActionExecutedContext context)
    {
        const int maxStackSize = 30;

        var http = context.HttpContext;
        var request = http.Request;
        var url = $"{request.Path}{request.QueryString}";

        if (request.Method != "GET" || url == "/previous-page" || request.Path.StartsWithSegments("/register") || request.Path == "/cab-service/re-application/resume-submission")
            return;

        var session = http.Session;
        var json = session.GetString(SessionKey);
        var stack = string.IsNullOrEmpty(json) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(json);

        if (request.Path.StartsWithSegments("/cab-service/home"))
        {
            stack.Clear(); 
        }
        else if (stack.Count == 0 || stack.Last() != url)
        {
            stack.Add(url);
        }
        if (stack.Count > maxStackSize)
        {
            stack.RemoveAt(0); 
        }

        session.SetString(SessionKey, JsonSerializer.Serialize(stack));
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        // nothing needed here
    }
}

