using DVSRegister.CommonUtility;

namespace DVSRegister.UnitTests.CommonUtility;

public class ResultTests
{
    public class OkTests
    {
        [Fact]
        public void IsSuccess_Is_True_For_Ok()
        {
            var result = Result<int>.Ok(42);

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(42, result.Value);
        }
    }

    public class FailTests
    {
        [Fact]
        public void IsFailure_Is_True_For_Fail()
        {
            var err = Error.NotFound("test");
            var result = Result<int>.Fail(err);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(err, result.Error);
        }
    }

    public class MapTests
    {
        [Fact]
        public void Maps_On_Success()
        {
            var result = Result<int>.Ok(2).Map(x => x * 5);

            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void Propagates_Error_On_Failure()
        {
            var err = Error.Validation("bad");
            var result = Result<int>.Fail(err)
                .Map(x => x * 5);

            Assert.False(result.IsSuccess);
            Assert.Equal(err, result.Error);
        }

        [Fact]
        public void Wraps_Exception_In_Error()
        {
            var result = Result<int>.Ok(1)
                .Map<string>(x => throw new DivideByZeroException());

            Assert.False(result.IsSuccess);
            Assert.Contains("Mapping failed", result.Error.Message);
        }
    }

    public class BindTests
    {
        [Fact]
        public void Binds_On_Success()
        {
            var result = Result<int>.Ok(3)
                .Bind(x => Result<string>.Ok($"v{x}"));

            Assert.True(result.IsSuccess);
            Assert.Equal("v3", result.Value);
        }

        [Fact]
        public void Returns_Failure_From_Binding_Function()
        {
            var result = Result<int>.Ok(3)
                .Bind<string>(_ => Result<string>.Fail(Error.NotFound()));

            Assert.False(result.IsSuccess);
            Assert.Equal("NOT_FOUND", result.Error.Code);
        }

        [Fact]
        public void Propagates_Error_On_Failure()
        {
            var err = Error.Conflict();
            var result = Result<int>.Fail(err)
                .Bind(x => Result<string>.Ok($"v{x}"));

            Assert.False(result.IsSuccess);
            Assert.Equal(err, result.Error);
        }
    }

    public class MatchTests
    {
        [Fact]
        public void Uses_OnSuccess_For_Success()
        {
            var result = Result<int>.Ok(10)
                .Match(
                    onSuccess: v => v * 2,
                    onFailure: _ => -1);

            Assert.Equal(20, result);
        }

        [Fact]
        public void Uses_OnFailure_For_Failure()
        {
            var result = Result<int>.Fail(Error.NotFound("nope"))
                .Match<string>(
                    onSuccess: v => v.ToString(),
                    onFailure: e => e.Code);

            Assert.Equal("NOT_FOUND", result);
        }
    }
}