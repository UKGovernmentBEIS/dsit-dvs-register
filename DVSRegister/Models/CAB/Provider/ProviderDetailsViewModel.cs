using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.UI;

namespace DVSRegister.Models.CAB.Provider
{
    public class ProviderDetailsViewModel : PaginationParameters
    {
        public bool IsCompanyInfoEditable { get; set; }
        public ProviderProfileDto Provider { get; set; }    
        public string LastUpdated { get; set; }
    }
}
