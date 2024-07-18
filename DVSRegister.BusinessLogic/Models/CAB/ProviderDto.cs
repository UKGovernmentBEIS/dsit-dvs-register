using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime? CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? PublishedTime { get; set; }
        public ICollection<CertificateInfoDto> CertificateInformation { get; set; }       
    }
}
