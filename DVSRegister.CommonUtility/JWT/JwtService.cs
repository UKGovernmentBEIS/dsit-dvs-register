using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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

        public TokenDetails GenerateToken()
        {
            TokenDetails tokenDetails = new TokenDetails();
            tokenDetails.TokenId  = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Website,configuration["ReviewPortalLink"]),
                new Claim(JwtRegisteredClaimNames.Jti,   tokenDetails.TokenId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var handler = new JsonWebTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
                SigningCredentials = credentials,
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience
            };
            var token = handler.CreateToken(tokenDescriptor);
            tokenDetails.Token = token;
            return tokenDetails;
        }

        public async Task<TokenDetails> ValidateToken(string token, string audience = "")
        {
          
            TokenDetails tokenDetails = new TokenDetails();
            var tokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
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
                }
                else
                {
                    if(claimsPrincipal!=null && claimsPrincipal.Exception !=null)
                    {
                        logger.LogError($"Claims principal exception: {claimsPrincipal.Exception}");
                    }
                    tokenDetails.IsAuthorised = false;
                }               
               
            }
            catch (Exception ex)
            {
                tokenDetails.IsAuthorised = false;

                logger.LogError($"Validate token error: {ex}");
                logger.LogError($"Stacktrace: {ex.StackTrace}");
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
