using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class SupplementarySchemeViewModel
    {
        public List<SupplementarySchemeDto>? AvailableSchemes { get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select if the digital identity and attribute service provider is certified against any supplementary schemes on their certificate")]
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<SupplementarySchemeDto>? SelectedSupplementarySchemes { get; set; }
        public bool FromSummaryPage { get; set; }
        public bool FromDetailsPage { get; set; }
    }
}
