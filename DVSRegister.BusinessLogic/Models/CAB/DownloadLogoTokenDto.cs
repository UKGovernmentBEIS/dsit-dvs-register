using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class DownloadLogoTokenDto
    {
        public string Id { get; set; }
        public string TokenId { get; set; }
        public string Token { get; set; }
     
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
