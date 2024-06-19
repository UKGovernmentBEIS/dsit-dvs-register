using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ProviderDto
    {
        public int Id { get; set; }     
        public int PreRegistrationId { get; set; }       
        public string RegisteredName { get; set; }
        public string TradingName { get; set; }
        public string PublicContactEmail { get; set; }
        public string TelephoneNumber { get; set; }
        public string WebsiteAddress { get; set; }
        public string Address { get; set; }       
        public ProviderStatusEnum ProviderStatus { get; set; }       
        public DateTime? PublishedTime { get; set; }
        public ICollection<CertificateInfoDto> CertificateInformation { get; set; }
    }
}
