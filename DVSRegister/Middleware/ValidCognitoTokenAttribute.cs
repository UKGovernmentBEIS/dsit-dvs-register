using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

public class ValidCognitoTokenAttribute : ActionFilterAttribute
{   
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var cognitoClient = context.HttpContext.RequestServices.GetService<CognitoClient>();
            var sessionToken = context.HttpContext.Session.GetString("IdToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
               throw new UnauthorizedAccessException("Invalid token");

            }
            sessionToken = sessionToken.Substring(1, sessionToken.Length - 2);       
           

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
            var tokenHandler = new JsonWebTokenHandler();
            var result = tokenHandler.ValidateTokenAsync(sessionToken, validationParameters);
            if (result.Result.IsValid)
            {
                var claimsPrincipal = new ClaimsPrincipal(result.Result.ClaimsIdentity);
                context.HttpContext.User = claimsPrincipal;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            base.OnActionExecuting(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception ex"+ex.Message);
            // If an exception occurs (indicating the token is invalid), redirect to the Login page
            context.Result = new RedirectToActionResult("LoginPage", "Login", null);
        }
    }
}
