using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.UI
{
    public class PaginationAndFilteringParameters
    {
        public string? SearchText { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public int PageNumber { get; set; }
        public string? SortBy{ get; set; }
        public string LastUpdated { get; set; }
        public List<RoleDto>? AvailableRoles { get; set; }
        public List<int>? SelectedRoleIds { get; set; }
        public List<RoleDto>? SelectedRoles { get; set; }
        public List<SupplementarySchemeDto>? AvailableSchemes { get; set; }
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<SupplementarySchemeDto>? SelectedSupplementarySchemes { get; set; }
        public List<TrustFrameworkVersionDto>? AvailableTrustFrameworkVersion { get; set; }
        public List<int>? SelectedTrustFrameworkVersionId { get; set; }
        public List<TrustFrameworkVersionDto>? SelectedTrustFrameworkVersion { get; set; }
    }
}
