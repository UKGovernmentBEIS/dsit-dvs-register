using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ProfileSummaryViewModel
    {     
        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }
        public bool? HasRegistrationNumber { get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? DUNSNumber { get; set; }      
        public PrimaryContactViewModel? PrimaryContact { get; set; }
        public SecondaryContactViewModel? SecondaryContact { get; set; }

        [Required(ErrorMessage = "Enter an email address in the correct format")]
        [AcceptedLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? PublicContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        public string? ProviderTelephoneNumber { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address")]
        public string? ProviderWebsiteAddress { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
