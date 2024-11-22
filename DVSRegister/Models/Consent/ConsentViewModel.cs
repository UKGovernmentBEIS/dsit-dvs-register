using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class ConsentViewModel
    {
        [MustBeTrue(ErrorMessage = "Select ‘I agree to publish these details on the register’")]        
        public bool? HasConsent { get; set; }
        public string? token { get; set; }       
        public ServiceDto? Service { get; set; }
    }
}
