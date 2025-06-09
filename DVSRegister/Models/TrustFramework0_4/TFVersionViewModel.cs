using DVSRegister.BusinessLogic.Models;
using DVSRegister.Models.CAB;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models
{

    public class TFVersionViewModel : ServiceSummaryBaseViewModel
    {
        public List<TrustFrameworkVersionDto>? AvailableVersions{ get; set; }

        [Required(ErrorMessage = "Select the TF Version")] //TO do : check error message
        public int? SelectedTFVersionId { get; set; }
        public TrustFrameworkVersionDto?  SelectedTFVersion { get; set; }

    }
}
