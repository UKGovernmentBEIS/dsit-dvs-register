using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility.JWT
{
    public interface IJwtService
    {
        public TokenDetails GenerateToken();
        public Task<TokenDetails> ValidateToken(string token, string audience = "");
    }
}
