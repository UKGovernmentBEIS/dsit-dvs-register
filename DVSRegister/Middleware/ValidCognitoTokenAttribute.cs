using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public class ValidCognitoTokenAttribute : ActionFilterAttribute
{   
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var sessionToken = context.HttpContext.Session.GetString("IdToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            sessionToken = sessionToken.Substring(1, sessionToken.Length - 2);

            var cognitoClient = context.HttpContext.RequestServices.GetService<CognitoClient>();
            var userPoolId = cognitoClient._userPoolId;
            var region = cognitoClient._region;
            var clientId = cognitoClient._clientId;

            Task<TokenValidationResult> result = TokenExtensions.ValidateToken(sessionToken, userPoolId, region, clientId);

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
