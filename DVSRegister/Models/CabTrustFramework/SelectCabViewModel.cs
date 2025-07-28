using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.CAB;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CabTrustFramework
{
    public class SelectCabViewModel : ServiceSummaryBaseViewModel
    {
        public IReadOnlyList<CabDto>? Cabs { get; set; }

        [Required(ErrorMessage = "Select the Conformity Assessment Body for the underpinning service")]
        public int? SelectedCabId { get; set; }
        public string? SelectedCabName { get; set; }
        public bool IsSelected { get; set; }
    }
}
