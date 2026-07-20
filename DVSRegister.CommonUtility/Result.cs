namespace DVSRegister.CommonUtility;

public class Error
{
    public string Code { get; }
    public string Message { get; }

    private Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error NotFound(string message = "Resource not found") =>
        new Error("NOT_FOUND", message);

    public static Error Validation(string message) =>
        new Error("VALIDATION", message);

    public static Error Conflict(string message = "Operation could not be completed due to a conflict.") =>
        new Error("CONFLICT", message);

    public static Error Unexpected(string message) =>
        new Error("UNEXPECTED", message);

    public static Error FromException(Exception ex, string context = "") =>
        Unexpected(
            string.IsNullOrWhiteSpace(context)
                ? ex.Message
                : $"{context}: {ex.Message}");
}

public class Result<T>
{
    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value!;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }

    public static Result<T> Ok(T value) => new Result<T>(true, value, default!);
    public static Result<T> Fail(Error error) => new Result<T>(false, default, error);

    public Result<U> Map<U>(Func<T, U> fn)
    {
        if (!IsSuccess) return Result<U>.Fail(Error);
        try
        {
            return Result<U>.Ok(fn(Value));
        }
        catch (Exception ex)
        {
            return Result<U>.Fail(Error.Unexpected($"Mapping failed: {ex.Message}"));
        }
    }

    public Result<U> Bind<U>(Func<T, Result<U>> fn)
    {
        if (!IsSuccess) return Result<U>.Fail(Error);
        return fn(Value);
    }

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}