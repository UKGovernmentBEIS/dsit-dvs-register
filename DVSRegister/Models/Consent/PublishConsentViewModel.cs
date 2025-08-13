namespace DVSRegister.Models
{
    public class PublishConsentViewModel : ConsentViewModel
    {
        [MustBeTrue(ErrorMessage = "Select ‘I agree to publish these details on the register’")]
        public bool? HasConsent { get; set; }
    }
}
