using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB.Provider
{
    public class ProviderListViewModel
    {
        public List<ProviderProfileDto>? Providers { get; set; }
        public string? SearchText { get; set; }
    }
}
