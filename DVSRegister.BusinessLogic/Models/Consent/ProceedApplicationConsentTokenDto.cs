using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models
{
    public class ProceedApplicationConsentTokenDto
    {
        public string Id { get; set; }
        public string TokenId { get; set; }
        public string Token { get; set; }      
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
