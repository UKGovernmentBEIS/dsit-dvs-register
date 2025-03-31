using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class QualityLevelViewModel :ServiceSummaryBaseViewModel
    {
        public List<QualityLevelDto>? AvailableQualityOfAuthenticators { get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select the quality of authenticator")]
        public List<int>? SelectedQualityofAuthenticatorIds { get; set; }
        public List<QualityLevelDto>? SelectedQualityofAuthenticators { get; set; }

        public List<QualityLevelDto>? AvailableLevelOfProtections { get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select the level of protection")]
        public List<int>? SelectedLevelOfProtectionIds { get; set; }
        public List<QualityLevelDto>? SelectedLevelOfProtections { get; set; }
       
    }
}
