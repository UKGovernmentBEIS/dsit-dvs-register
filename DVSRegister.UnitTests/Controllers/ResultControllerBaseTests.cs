using DVSRegister.CommonUtility;
using DVSRegister.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.UnitTests.Controllers;

public class ResultControllerBaseTests
{
    private sealed class TestController : ResultControllerBase
    {
        public IActionResult TestSuccess(int value)
        {
            var result = Result<int>.Ok(value);
            return FromResult(result, v => new OkObjectResult(v));
        }

        public IActionResult TestNotFound()
        {
            var result = Result<int>.Fail(Error.NotFound());
            return FromResult(result, _ => (IActionResult)new OkResult());
        }

        public IActionResult TestValidation()
        {
            var result = Result<int>.Fail(Error.Validation("invalid"));
            return FromResult(result, _ => (IActionResult)new OkResult());
        }

        public IActionResult TestConflict()
        {
            var result = Result<int>.Fail(Error.Conflict());
            return FromResult(result, _ => (IActionResult)new OkResult());
        }

        public IActionResult TestUnexpected()
        {
            var result = Result<int>.Fail(Error.Unexpected("boom"));
            return FromResult(result, _ => (IActionResult)new OkResult());
        }
    }

    private TestController CreateController()
    {
        return new TestController();
    }

    [Fact]
    public void FromResult_Returns_Success_Action_When_Result_Is_Success()
    {
        var ctrl = CreateController();
        var result = ctrl.TestSuccess(42);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void FromResult_Returns_Redirect_For_NOT_FOUND()
    {
        var ctrl = CreateController();
        var result = ctrl.TestNotFound();

        var redirect = Assert.IsType<RedirectToRouteResult>(result);
        Assert.Equal("RegisterPageNotFound", redirect.RouteName);
    }

    [Fact]
    public void FromResult_Returns_400_For_VALIDATION()
    {
        var ctrl = CreateController();
        var result = ctrl.TestValidation();

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, status.StatusCode);
    }

    [Fact]
    public void FromResult_Returns_429_For_CONFLICT()
    {
        var ctrl = CreateController();
        var result = ctrl.TestConflict();

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, status.StatusCode);
    }

    [Fact]
    public void FromResult_Throws_For_UNKNOWN_Error()
    {
        var ctrl = CreateController();

        Assert.Throws<InvalidOperationException>(() => ctrl.TestUnexpected());
    }
}