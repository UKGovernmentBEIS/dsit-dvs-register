using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;

public class ValidCognitoTokenAttribute : ActionFilterAttribute
{   
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var cognitoClient = context.HttpContext.RequestServices.GetService<CognitoClient>();
            var sessionToken = context.HttpContext.Session.GetString("IdToken");
            sessionToken = sessionToken.Substring(1, sessionToken.Length - 2);
       
            if (string.IsNullOrEmpty(sessionToken))
            {
                context.Result = new RedirectToActionResult("", "Login", null);
                return;
            }

            string cognitoIssuer = $"https://cognito-idp.{cognitoClient._region}.amazonaws.com/{cognitoClient._userPoolId}";
            string jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
            string cognitoAudience = cognitoClient._clientId;
            
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    string json = new WebClient().DownloadString(jwtKeySetUrl);

                    IList<JsonWebKey> keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

                    return (IEnumerable<SecurityKey>)keys;
                },
                ValidIssuer = cognitoIssuer,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidAudience = cognitoAudience
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(sessionToken, validationParameters, out var validatedToken);
            context.HttpContext.User = principal;
            base.OnActionExecuting(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception ex"+ex.Message);
            // If an exception occurs (indicating the token is invalid), redirect to the Login page
            context.Result = new RedirectToActionResult("", "Login", null);
        }
    }
}
