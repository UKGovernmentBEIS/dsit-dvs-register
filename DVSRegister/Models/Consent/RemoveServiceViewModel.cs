using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;

namespace DVSRegister.Models
{
    public class RemoveServiceViewModel
    {
        [MustBeTrue(ErrorMessage = "Select ‘Remove my details from the " + Constants.RegisterNameLower + "’")]
        public bool? HasConsent { get; set; }
        public string? token { get; set; }
        public ServiceDto? Service { get; set; }
    }
}
