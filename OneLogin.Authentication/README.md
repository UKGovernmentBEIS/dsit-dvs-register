# Reusable GOV.UK One Login authentication

`OneLogin.Authentication` contains application-neutral GOV.UK One Login authentication registration for .NET 8 applications.

## Register authentication

```csharp
builder.Services.AddGovUkOneLoginAuthentication(builder.Configuration);
```

The request pipeline must include authentication before authorization:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## Application configuration

Each consuming application must provide its own non-secret settings:

```json
{
  "OneLogin": {
    "Environment": "Integration",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc",
    "CookieName": "ApplicationName.Authentication",
    "UserAgent": "ApplicationName/1.0"
  }
}
```

Use a unique `CookieName` and `UserAgent` for every application. The supported environments are `Integration` and `Production`.

If an application needs a differently named configuration section, pass its name explicitly:

```csharp
builder.Services.AddGovUkOneLoginAuthentication(builder.Configuration, "ApplicationOneLogin");
```

## Secrets

Each application must obtain its own GOV.UK One Login client registration and key pair. Supply these values through .NET user secrets locally and the deployment secret manager in hosted environments:

- `OneLogin:ClientID`
- `OneLogin:PrivateKey`

Environment-variable equivalents are `OneLogin__ClientID` and `OneLogin__PrivateKey`.

Do not commit client IDs or private keys. Register only the corresponding public key with GOV.UK One Login.

`OneLogin:KeyId` is optional when the client has a single public key registered directly. If a service publishes a JWKS endpoint or has multiple client keys, configure it with the `kid` of this application's registered public key. This value is added only to the client assertion signed by the application.

The ID token returned by One Login has a separate `kid` selected by One Login. The underlying OpenID Connect middleware reads One Login's discovery metadata, downloads the keys from its `jwks_uri`, and matches the ID-token header's `kid` to the corresponding JWK. Do not configure One Login's ID-token signing-key ID as `OneLogin:KeyId`.

## Sign out

Sign out through both the local cookie scheme and the One Login scheme so that the application cookie is deleted and the browser is redirected through One Login's end-session endpoint:

```csharp
return SignOut(
    new AuthenticationProperties { RedirectUri = "/" },
    CookieAuthenticationDefaults.AuthenticationScheme,
    OneLoginDefaults.AuthenticationScheme);
```

`HttpContext.Session.Clear()` is not an authentication logout mechanism and cannot delete cookies owned by the One Login domain.
