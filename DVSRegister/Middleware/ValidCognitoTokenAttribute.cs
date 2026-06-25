using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public class ValidCognitoTokenAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
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

                string? email = result.Result?.Claims?.FirstOrDefault(c => c.Key == "email").Value?.ToString();

                if (string.IsNullOrEmpty(email)) throw new UnauthorizedAccessException("Email not found in claims");

                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                var user = await userService.GetUser(email);

                if (user == null || user.AccountStatus != AccountStatusEnum.Active)
                    context.Result = new RedirectToActionResult("StartPageWithBanner", "Login", null);
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
        
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception ex"+ex.Message);
            // If an exception occurs (indicating the token is invalid), redirect to the Login page
            context.Result = new RedirectToActionResult("LoginPage", "Login", null);
        }
    }
}
