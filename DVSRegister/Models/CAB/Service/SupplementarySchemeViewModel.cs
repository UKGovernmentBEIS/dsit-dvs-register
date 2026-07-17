using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class SupplementarySchemeViewModel : ServiceSummaryBaseViewModel
    {
        public List<SupplementarySchemeDto>? AvailableSchemes { get; set; }

        [EnsureMinimumCount(ErrorMessage = $"Select if the {Constants.RegisterNameSingular} provider is certified against any supplementary schemes on their certificate")]
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<SupplementarySchemeDto>? SelectedSupplementarySchemes { get; set; }     
    }
}
