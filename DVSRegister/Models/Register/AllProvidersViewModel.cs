using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.UI;

namespace DVSRegister.Models.Register
{
    public class AllProvidersViewModel : PaginationAndFilteringParameters
    {
        public List<ProviderProfileDto>? Providers { get; set; }
        
    }
}
