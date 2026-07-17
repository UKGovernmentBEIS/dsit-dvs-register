using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class IdentityProfileViewModel :ServiceSummaryBaseViewModel
    {
        public List<IdentityProfileDto>? AvailableIdentityProfiles{ get; set; }

        [EnsureMinimumCount(ErrorMessage = $"Select the identity profiles for the {Constants.RegisterNameSingular} provider")]
        public List<int>? SelectedIdentityProfileIds { get; set; }
        public List<IdentityProfileDto>? SelectedIdentityProfiles { get; set; }  
     
    }
}
