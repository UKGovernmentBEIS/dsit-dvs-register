using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class RoleViewModel
    {
        public List<RoleDto>? AvailableRoles{ get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select at least one option.")]
        public List<int>? SelectedRoleIds { get; set; }
        public List<RoleDto>? SelectedRoles{ get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
