namespace OneLogin.Poc.Models;

public sealed class OneLoginClaimsViewModel
{
    public string Subject { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string VectorOfTrust { get; init; } = string.Empty;

    public string SessionId { get; init; } = string.Empty;
}
