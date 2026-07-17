using DVSRegister.CommonUtility;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers;

public abstract class ResultControllerBase : Controller
{
    protected IActionResult FromResult<T>(Result<T> result, Func<T, IActionResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : FromError(result.Error);

    private IActionResult FromError(Error error) => error.Code switch
    {
        "NOT_FOUND" => RedirectToAction("RegisterPageNotFound", "Error"),
        "VALIDATION" => StatusCode(StatusCodes.Status400BadRequest, error.Message),
        "CONFLICT" => StatusCode(StatusCodes.Status429TooManyRequests, error.Message),
        _ => throw new InvalidOperationException(error.Message)
    };
}