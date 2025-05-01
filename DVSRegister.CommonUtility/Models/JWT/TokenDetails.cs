namespace DVSRegister.CommonUtility.Models
{
    public class TokenDetails
    {
        public bool IsAuthorised { get; set; }
        public bool IsExpired { get; set; }
        public string Token { get; set; }
        public string TokenId {  get; set; }

        public int ProviderProfileId { get; set; }
        public List<int>? ServiceIds { get; set; }
    }
}
