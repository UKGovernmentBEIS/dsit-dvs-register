using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class ProviderDetailsViewModel
    {
        public bool IsCompanyInfoEditable { get; set; }
        public ProviderProfileDto Provider { get; set; }    
        public string LastUpdated { get; set; }
    }
}
