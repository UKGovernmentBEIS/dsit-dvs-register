using Microsoft.IdentityModel.JsonWebTokens;

namespace DVSRegister.Middleware
{
    public static class TokenHandlerProvider
    {
        public static JsonWebTokenHandler TokenHandler { get; set; } = new JsonWebTokenHandler();
    }
}
