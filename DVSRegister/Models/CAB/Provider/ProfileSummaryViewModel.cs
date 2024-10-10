using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ProfileSummaryViewModel
    {
        [Required(ErrorMessage = "Enter the digital identity and attribute provider's registered name")]
        [MaximumLength(160, ErrorMessage = "The company's registered name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "The company's registered name must contain only letters, numbers and accepted characters")]
        public string? RegisteredName { get; set; }       
        public string? TradingName { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the provider has either a Companies House or charity registration number")]
        public bool? HasRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Enter a Companies House or charity registration number")]
        [RequiredLength(8, ErrorMessage = "Your Companies House number must be 8 characters long.")]
        [AcceptedCharacters(@"^[a-zA-Z0-9]*$", ErrorMessage = "Your Companies House number must contain only letters and numbers.")]
        public string? CompanyRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Enter a D-U-N-S number")]
        [RequiredLength(9, ErrorMessage = "Your D-U-N-S number must be 9 characters long")]
        [AcceptedCharacters(@"^[0-9]+$", ErrorMessage = "Your D-U-N-S number must contain only numbers")]
        public string? DUNSNumber { get; set; }

        [Required(ErrorMessage = "Select if you have a parent company outside the UK")]
        public bool? HasParentCompany { get; set; }

        [Required(ErrorMessage = "Enter the registered name of your parent company")]
        [MaximumLength(160, ErrorMessage = "Your company's registered name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "Your parent company's registered name must contain only letters, numbers and accepted characters.")]
        public string? ParentCompanyRegisteredName { get; set; }

        [Required(ErrorMessage = "Enter the location of your parent company")]
        public string? ParentCompanyLocation { get; set; }

        public PrimaryContactViewModel? PrimaryContact { get; set; }
        public SecondaryContactViewModel? SecondaryContact { get; set; }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format")]
        [Required(ErrorMessage = "Enter an email address in the correct format")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? PublicContactEmail { get; set; }
      
        public string? ProviderTelephoneNumber { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address")]
        public string? ProviderWebsiteAddress { get; set; }

        public bool FromSummaryPage { get; set; }
    }
}
