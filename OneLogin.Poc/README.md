# GOV.UK One Login proof of concept

This standalone .NET 8 MVC application with Razor Views uses the reusable `OneLogin.Authentication` project. It does not reference or modify DVS Register.

## Register the local service

1. Sign in to the GOV.UK One Login admin tool using a DSIT email address.
2. Create a service and retain its client ID.
3. Register `https://localhost:7217/signin-oidc` as a redirect URI.
4. Register `https://localhost:7217/signout-callback-oidc` as a post-logout redirect URI.
5. Select `private_key_jwt` as the token endpoint authentication method.

## Generate and register a key

Run these commands outside the repository, or rely on the repository PEM ignore rules:

```powershell
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 -out onelogin-private-key.pem
openssl rsa -pubout -in onelogin-private-key.pem -out onelogin-public-key.pem
```

If OpenSSL is not installed or the command is unavailable on Windows, run the .NET 8 key generator from the repository root:

```powershell
dotnet run --project .\OneLogin.KeyGenerator
```

By default, the generated keys are written outside the repository:

```text
%USERPROFILE%\.dvsregister-secrets\onelogin-private-key.pem
%USERPROFILE%\.dvsregister-secrets\onelogin-public-key.pem
```

The generator will not replace an existing key pair unless `--force` is supplied. Generate into a new directory when rotating keys so the existing key remains available during the transition:

```powershell
dotnet run --project .\OneLogin.KeyGenerator -- "$env:USERPROFILE\.dvsregister-secrets\rotation-$(Get-Date -Format 'yyyyMMdd')"
```

Provide only `onelogin-public-key.pem` to GOV.UK One Login. Never share or commit the private key.

## Configure local secrets

If OpenSSL was used to generate the private key in the repository root, run:

```powershell
dotnet user-secrets set "OneLogin:ClientID" "YOUR_CLIENT_ID" --project OneLogin.Poc
dotnet user-secrets set "OneLogin:PrivateKey" "$(Get-Content onelogin-private-key.pem -Raw)" --project OneLogin.Poc
```

If the .NET key generator was used with its default output directory, run:

```powershell
dotnet user-secrets set "OneLogin:ClientID" "YOUR_CLIENT_ID" --project OneLogin.Poc
dotnet user-secrets set "OneLogin:PrivateKey" "$(Get-Content "$env:USERPROFILE\.dvsregister-secrets\onelogin-private-key.pem" -Raw)" --project OneLogin.Poc
```

The equivalent environment variable names are `OneLogin__ClientID` and `OneLogin__PrivateKey`. If the One Login client registration uses a JWKS endpoint or multiple client keys, also configure `OneLogin:KeyId` (`OneLogin__KeyId`) with the `kid` of this application's registered public key.

## Run and verify

```powershell
dotnet run --project OneLogin.Poc --launch-profile https
```

Open `https://localhost:7217`. The protected page triggers a challenge to the integration environment. After authenticating with a test user, the page displays `sub`, `email`, `phone_number`, `vot`, and `sid`.

The integration requests only the `Cl.Cm` vector of trust and the `openid`, `email`, and `phone` scopes. The NuGet middleware generates the five-minute `private_key_jwt` client assertion with `aud`, `iss`, `sub`, and a unique `jti`; validates state and nonce; exchanges the authorization code; and validates the ID token against One Login metadata/JWKS. The reusable module also supplies a `User-Agent` header on OIDC backchannel requests.

The client assertion and ID token use different signing keys:

- The POC signs the client assertion with `OneLogin:PrivateKey`. Its optional JWT-header `kid` comes from `OneLogin:KeyId`, and One Login validates it using the public key registered for this client.
- One Login signs the ID token and supplies its own `kid` in the ID-token header. The middleware obtains One Login's keys from the discovery document's `jwks_uri`, selects the JWK matching that `kid`, and validates the signature, issuer, audience, lifetime, and nonce. One Login's ID-token `kid` is not configured in the POC.

## Sign out

The page posts to the MVC `Logout` action. It signs out both the local cookie scheme and the One Login scheme, which invokes One Login's end-session endpoint. Because `/` is protected, returning to `/` after logout immediately starts a new authentication challenge. The POC does not use `HttpContext.Session` for authentication, and clearing ASP.NET session state would not remove either authentication session.
