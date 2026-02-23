using DVSRegister.BusinessLogic.Models.CAB;
using System.Text.Json.Serialization;

namespace DVSRegister.BusinessLogic.Models
{
    public class TrustmarkNumberDto
    {    
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto Provider { get; set; }
        public int ServiceId { get; set; }

        [JsonIgnore]
        public ServiceDto Service { get; set; }
        public int CompanyId { get; set; }
        public int ServiceNumber { get; set; }    
        public string TrustMarkNumber { get; set; }
        public int ServiceKey { get; set; }
        public string PngLogoLink { get; set; }
        public string JpegLogoLink { get; set; }
        public string SvgLogoLink { get; set; }
        public bool TrustMarkNumberVerified { get; set; }
        public bool LogoVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
