using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;

namespace DVSRegister.Models
{
    public class RemoveProviderViewModel
    {
        [MustBeTrue(ErrorMessage = "Select ‘Remove my details from the " + Constants.RegisterNameLower + "’")]
        public bool? HasConsent { get; set; }
        public string? token { get; set; }
        public ProviderProfileDto? Provider { get; set; }
    }
}
