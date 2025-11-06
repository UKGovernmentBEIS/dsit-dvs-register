using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.UI;
using Microsoft.DotNet.Scaffolding.Shared;

namespace DVSRegister.Models.Register
{
    public class AllProvidersViewModel : PaginationAndFilteringParameters
    {
        public List<ProviderProfileDto>? Providers { get; set; }
        public string? CurrentSort { get; set; }
        public string? CurrentSortAction { get; set; }


    }

}
}
