namespace DVSRegister.CommonUtility.Models
{
    public class JwtSettings
    {
        public const string ConfigSection = "JwtSettings";
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
