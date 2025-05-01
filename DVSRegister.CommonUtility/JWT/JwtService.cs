using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DVSRegister.CommonUtility.JWT
{
    public class JwtService :IJwtService
    {
        private readonly JwtSettings jwtSettings;
        private readonly IConfiguration configuration;
        private readonly ILogger<JwtService> logger;

        public JwtService(IOptions<JwtSettings> jwtSettings, IConfiguration configuration, ILogger<JwtService> logger)
        {
            this.jwtSettings = jwtSettings.Value;
            this.configuration = configuration;
            this.logger = logger;
        }

      

        public async Task<TokenDetails> ValidateToken(string token, string audience = "")
        {
          
            TokenDetails tokenDetails = new TokenDetails();
            var tokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = string.IsNullOrEmpty(audience)?  jwtSettings.Audience: "DSIT",
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };

            try
            {
                var claimsPrincipal = await tokenHandler.ValidateTokenAsync(token, validationParameters);
                if (claimsPrincipal != null && claimsPrincipal.ClaimsIdentity!= null && claimsPrincipal.ClaimsIdentity.IsAuthenticated)
                {
                    
                    var jti = Convert.ToString(claimsPrincipal.Claims.First(claim => claim.Key == JwtRegisteredClaimNames.Jti).Value)??string.Empty;
                    tokenDetails.IsAuthorised = true;
                    tokenDetails.TokenId = jti;
                    tokenDetails.Token = token;

                    if(claimsPrincipal.Claims.Any(claim => claim.Key == "ProviderProfileId"))
                    {
                        tokenDetails.ProviderProfileId = Convert.ToInt32(claimsPrincipal.Claims.First(claim => claim.Key == "ProviderProfileId").Value);
                    }

                    if (claimsPrincipal.Claims.Any(claim => claim.Key == "ServiceId"))
                    {
                        string serviceIdClaim= Convert.ToString(claimsPrincipal.Claims.First(claim => claim.Key == "ServiceId").Value)??string.Empty;
                        tokenDetails.ServiceIds = serviceIdClaim.Split(',').Select(int.Parse).ToList();
                    }                       

                }
                else
                {
                    if(claimsPrincipal!=null && claimsPrincipal.Exception !=null)
                    {
                        logger.LogError($"Claims principal exception: {claimsPrincipal.Exception}");

                        if(claimsPrincipal.Exception is SecurityTokenExpiredException)
                            tokenDetails.IsExpired = true;
                    }
                    tokenDetails.IsAuthorised = false;
                }               
               
            }
           
            catch (SecurityTokenException ex)
            {
                tokenDetails.IsAuthorised = false;
                logger.LogError("SecurityTokenException: {ex}", ex);
            }          
            catch (Exception ex)
            {
                tokenDetails.IsAuthorised = false;
                logger.LogError("Validate token error: {ex}",ex);
                logger.LogError("Stacktrace: {ex.StackTrace}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.Write("Inner Exception");
                    Console.Write(String.Concat(ex.InnerException.StackTrace, ex.InnerException.Message));
                }
            }
            return tokenDetails;
        }
    }
}
