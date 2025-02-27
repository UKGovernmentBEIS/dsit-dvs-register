namespace DVSRegister.BusinessLogic.Models
{
    public class ServiceDraftTokenDto
    {
        public string Id { get; set; }

        public string TokenId { get; set; }
        public string Token { get; set; }
      
        public int ServiceDraftId { get; set; }
        public ServiceDraftDto ServiceDraft { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
