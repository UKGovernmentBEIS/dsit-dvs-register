using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;


namespace DVSRegister.Extensions
{
    public static class TokenExtensions
    {
        public static Task<TokenValidationResult> ValidateToken(string sessionToken, string userPoolId, string region, string clientId)
        {
            string cognitoIssuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
            string jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
            string cognitoAudience = clientId;

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
            return result;
        
        }
    }
}
