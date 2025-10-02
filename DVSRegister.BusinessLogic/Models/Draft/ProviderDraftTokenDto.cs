namespace DVSRegister.BusinessLogic.Models
{
    public class ProviderDraftTokenDto
    {
        public string Id { get; set; }
        public string TokenId { get; set; }
        public string Token { get; set; }     
        public int ProviderProfileDraftId { get; set; }
        public ProviderProfileDraftDto ProviderProfileDraft { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
