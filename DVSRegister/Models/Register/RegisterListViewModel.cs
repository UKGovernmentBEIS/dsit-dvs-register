﻿using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class RegisterListViewModel
    {
        public List<ProviderDto>? Providers { get; set; }
        public string LastUpdated { get; set; }
        public string? SearchText { get; set; }
        public List<RoleDto>? AvailableRoles { get; set; }
        public List<int>? SelectedRoleIds { get; set; }
        public List<RoleDto>? SelectedRoles { get; set; }


        public List<SupplementarySchemeDto>? AvailableSchemes { get; set; }        
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<SupplementarySchemeDto>? SelectedSupplementarySchemes { get; set; }
    }
}
