using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class RegisterListViewModel
    {
        public List<ProviderDto>? Registers { get; set; }
        public string? SearchText { get; set; }
        public List<int>? SelectedRoleTypeIds { get; set; }
        public List<int>? SelectedSchemeIds { get; set; }
    }
}
