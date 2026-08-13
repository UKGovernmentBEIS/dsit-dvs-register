namespace OneLogin.Authentication;

public sealed class OneLoginConfiguration
{
    public const string SectionName = "OneLogin";

    public string ClientID { get; init; } = string.Empty;

    public string PrivateKey { get; init; } = string.Empty;

    public string Environment { get; init; } = string.Empty;

    public string CallbackPath { get; init; } = string.Empty;

    public string SignedOutCallbackPath { get; init; } = string.Empty;

    public string CookieName { get; init; } = string.Empty;

    public string UserAgent { get; init; } = string.Empty;

    //If only one public key is registered directly, KeyId can remain null, and the client assertion will omit kid.
    //It is kept as an optional variable to support JWKS-based registration and future key rotation.It is not required for ID-token validation.
    public string? KeyId { get; init; }
}
