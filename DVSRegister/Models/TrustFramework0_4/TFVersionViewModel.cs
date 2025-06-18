using DVSRegister.BusinessLogic.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models
{

    public class TFVersionViewModel : ServiceSummaryBaseViewModel
    {
        public List<TrustFrameworkVersionDto>? AvailableVersions{ get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select the TF Version")] //TO do : check error message
        public int SelectedTFVersionId { get; set; }
        public TrustFrameworkVersionDto  SelectedTFVersion { get; set; }

    }
}
