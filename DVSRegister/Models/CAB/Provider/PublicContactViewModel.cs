using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class PublicContactViewModel
    {

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format")]       
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? PublicContactEmail { get; set; }

        public string? ProviderTelephoneNumber { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address")]
        public string? ProviderWebsiteAddress { get; set; }

        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid link to contact page")]
        public string? LinkToContactPage { get; set; }
        
        public int ProviderId { get; set; }

       
    }
}
