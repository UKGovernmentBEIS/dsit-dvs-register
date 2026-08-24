using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers;

public abstract class ResultControllerBase : Controller
{
    protected IActionResult FromResult<T>(Result<T> result, Func<T, IActionResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : FromError(result.Error);

    protected IActionResult FromError(Error error)
    {
        return error.Code switch
        {
            "NOT_FOUND" => NotFoundView(),
            "VALIDATION" => StatusCode(StatusCodes.Status400BadRequest, error.Message),
            "CONFLICT" => StatusCode(StatusCodes.Status429TooManyRequests, error.Message),
            _ => ErrorView()
        };
    }

    private IActionResult NotFoundView()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("RegisterPageNotFound");
    }

    private IActionResult ErrorView()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View("Error");
    }
}