using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;

namespace OneLogin.Authentication;

public static class OneLoginAuthenticationExtensions
{
    public static IServiceCollection AddGovUkOneLoginAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = OneLoginConfiguration.SectionName)
    {
        var oneLoginConfiguration = configuration
            .GetRequiredSection(sectionName)
            .Get<OneLoginConfiguration>()
            ?? throw new InvalidOperationException($"The {sectionName} configuration section is missing.");

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.ClientID))
        {
            throw new InvalidOperationException($"{sectionName}:ClientID must be configured using a secure configuration provider.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.PrivateKey))
        {
            throw new InvalidOperationException($"{sectionName}:PrivateKey must be configured using a secure configuration provider.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.Environment))
        {
            throw new InvalidOperationException($"{sectionName}:Environment must be configured.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.CallbackPath))
        {
            throw new InvalidOperationException($"{sectionName}:CallbackPath must be configured.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.SignedOutCallbackPath))
        {
            throw new InvalidOperationException($"{sectionName}:SignedOutCallbackPath must be configured.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.CookieName))
        {
            throw new InvalidOperationException($"{sectionName}:CookieName must be configured.");
        }

        if (string.IsNullOrWhiteSpace(oneLoginConfiguration.UserAgent))
        {
            throw new InvalidOperationException($"{sectionName}:UserAgent must be configured.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OneLoginDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = oneLoginConfiguration.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            })
            .AddOneLogin(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Environment = GetEnvironment(oneLoginConfiguration.Environment);
                options.ClientId = oneLoginConfiguration.ClientID;
                options.CallbackPath = oneLoginConfiguration.CallbackPath;
                options.SignedOutCallbackPath = oneLoginConfiguration.SignedOutCallbackPath;
                options.VectorsOfTrust = ["Cl.Cm"];
                options.Scope.Add("phone");
                options.CorrelationCookie.Name = $"{oneLoginConfiguration.CookieName}.Correlation.";
                options.NonceCookie.Name = $"{oneLoginConfiguration.CookieName}.Nonce.";

                using var rsa = RSA.Create();
                rsa.ImportFromPem(NormalizePem(oneLoginConfiguration.PrivateKey));
                options.ClientAuthenticationCredentials = new SigningCredentials(
                    new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true))
                    {
                        KeyId = oneLoginConfiguration.KeyId
                    },
                    SecurityAlgorithms.RsaSha256);
            });

        services.AddSingleton<IPostConfigureOptions<OneLoginOptions>>(
            new OneLoginPostConfigureOptions(oneLoginConfiguration.UserAgent));

        return services;
    }

    private static string GetEnvironment(string environment) => environment.Trim() switch
    {
        var value when value.Equals(OneLoginEnvironments.Integration, StringComparison.OrdinalIgnoreCase) =>
            OneLoginEnvironments.Integration,
        var value when value.Equals(OneLoginEnvironments.Production, StringComparison.OrdinalIgnoreCase) =>
            OneLoginEnvironments.Production,
        _ => throw new InvalidOperationException(
            $"Unsupported One Login environment '{environment}'. Use Integration or Production.")
    };

    private static string NormalizePem(string privateKey) =>
        privateKey.Contains("\\n", StringComparison.Ordinal)
            ? privateKey.Replace("\\r", string.Empty, StringComparison.Ordinal)
                .Replace("\\n", Environment.NewLine, StringComparison.Ordinal)
            : privateKey;

    private sealed class OneLoginPostConfigureOptions : IPostConfigureOptions<OneLoginOptions>
    {
        private static readonly PropertyInfo OpenIdConnectOptionsProperty = typeof(OneLoginOptions).GetProperty(
            "OpenIdConnectOptions",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("The One Login OIDC options could not be found.");

        private readonly string userAgent;

        public OneLoginPostConfigureOptions(string userAgent)
        {
            this.userAgent = userAgent;
        }

        public void PostConfigure(string? name, OneLoginOptions options)
        {
            var oidcOptions = (OpenIdConnectOptions?)OpenIdConnectOptionsProperty.GetValue(options)
                ?? throw new InvalidOperationException("The One Login OIDC options could not be read.");

            oidcOptions.ClaimActions.MapUniqueJsonKey("phone_number", "phone_number");
            oidcOptions.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            
        }
    }
}
