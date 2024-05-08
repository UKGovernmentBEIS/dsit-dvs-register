using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB
{
    public class SupplementarySchemeViewModel
    {
        public List<SupplementarySchemeDto>? AvailableSchemes { get; set; }     
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<SupplementarySchemeDto>? SelectedSupplementarySchemes { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
