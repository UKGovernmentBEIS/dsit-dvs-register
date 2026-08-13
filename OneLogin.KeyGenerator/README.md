# GOV.UK One Login key generator

This .NET 8 console application generates a 2048-bit RSA key pair without additional NuGet packages.

By default, keys are written outside the repository to `%USERPROFILE%\.dvsregister-secrets` on Windows.

```powershell
dotnet run --project .\OneLogin.KeyGenerator
```

To select another output directory:

```powershell
dotnet run --project .\OneLogin.KeyGenerator -- "C:\secure\onelogin"
```

The generator refuses to replace an existing key pair. When rotating keys, generate into a new directory so the active private key remains available during the registration transition:

```powershell
dotnet run --project .\OneLogin.KeyGenerator -- "$env:USERPROFILE\.dvsregister-secrets\rotation-$(Get-Date -Format 'yyyyMMdd')"
```

Use `--force` only when an intentional replacement of files in the selected directory is required. Replacing the active private key before registering its new public key will prevent client authentication.

Register `onelogin-public-key.pem` with GOV.UK One Login. Store `onelogin-private-key.pem` in .NET user secrets for local development and in the deployment secret manager for hosted environments. Never commit or share the private key.
