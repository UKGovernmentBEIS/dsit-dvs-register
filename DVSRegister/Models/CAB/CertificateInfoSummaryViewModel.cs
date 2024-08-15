using System.ComponentModel.DataAnnotations;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class CertificateInfoSummaryViewModel
    {
        public int? PreRegistrationId { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's registered name")]
        [MaximumLength(160, ErrorMessage = "The company's registered name must be less than 161 characters.")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "The company's registered name must contain only letters, numbers and accepted characters.")]
        public string? RegisteredName { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's trading name")]
        public string? TradingName { get; set; }

        
        [Required(ErrorMessage ="Enter an email address in the correct format")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com.")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters.")]
        public string? PublicContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        [UKPhoneNumber(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        public string? TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Enter the digital identity and attribute provider's website address")]       
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address.")]
        public string? WebsiteAddress { get; set; }

        [Required(ErrorMessage ="Enter the company address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Enter the name of the service")]
        public string? ServiceName { get; set; }

        [Required(ErrorMessage = "Select if the digital identity and attribute service provider is certified against any supplementary schemes on their certificate")]
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }

        public bool FromSummaryPage { get;set; }
    }
}
