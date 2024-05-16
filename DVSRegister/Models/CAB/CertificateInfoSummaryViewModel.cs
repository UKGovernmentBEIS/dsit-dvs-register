using System.ComponentModel.DataAnnotations;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class CertificateInfoSummaryViewModel
    {
        public int? PreRegistrationId { get; set; }

        [Required(ErrorMessage = "Enter the registered name of your company.")]
        [AcceptedLength(160, ErrorMessage = "Your company's registered name must be less than 161 characters.")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "Your company's registered name must contain only letters, numbers and accepted characters.")]
        public string? RegisteredName { get; set; }

        [Required(ErrorMessage = "Enter the trading name of your company.")]
        public string? TradingName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage ="Email field is mandatory")]
        public string? PublicContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        [UKPhoneNumber(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        public string? TelephoneNumber { get; set; }

        [Required(ErrorMessage ="Enter a website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Invalid website address.")]
        public string? WebsiteAddress { get; set; }

        [Required(ErrorMessage ="Enter a valid address")]
        public string? Address { get; set; }        
        public string? ServiceName { get; set; }
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }
    }
}
