using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models
{
    public class CompanyViewModel
    {
        [Required(ErrorMessage = "Enter the registered name of your company.")]
        [AcceptedLength(160, ErrorMessage = "Your company's registered name must be less than 161 characters.")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "Your company's registered name must contain only letters, numbers and accepted characters.")]
        public string RegisteredCompanyName { get; set; }

        [Required(ErrorMessage = "Enter the trading name of your company.")]
        public string TradingName { get; set; }

        [Required(ErrorMessage = "Enter a Companies House Number.")]
        [AcceptedLength(8, ErrorMessage = "Your Companies House number must be 8 char long.")]
        [MinLength(8, ErrorMessage = "Your Companies House number must be 8 char long.")]
        [AcceptedCharacters(@"^[a-zA-Z0-9]*$", ErrorMessage = "Your Companies House number must contain only letters and numbers.")]
        public string CompanyRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Select if you have a parent company outside the UK.")]
        public bool? HasParentCompany { get; set; }

        [RequiredIf("HasParentCompany", true, ErrorMessage = "Enter the registered name of your parent company.")]
        public string? ParentCompanyRegisteredName { get; set; }

        [RequiredIf("HasParentCompany", true, ErrorMessage = "Enter the location of your parent company.")]
        public string? ParentCompanyLocation { get; set; }
    }
}
