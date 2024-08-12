using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class QualityLevelViewModel
    {
        public List<QualityLevelDto>? AvailableQualityLevels { get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select the identity profile(s) for the digital identity and attribute service provider")]
        public List<int>? SelectedQualityLevelIds{ get; set; }
        public List<QualityLevelDto>? SelectedQualityLevels{ get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
