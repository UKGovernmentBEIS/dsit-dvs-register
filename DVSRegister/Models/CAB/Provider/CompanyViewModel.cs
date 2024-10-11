using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class CompanyViewModel
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "Enter the digital identity and attribute provider's registered name")]
        [MaximumLength(160, ErrorMessage = "The company's registered name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "The company's registered name must contain only letters, numbers and accepted characters")]
        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }       
        public bool? HasRegistrationNumber { get; set; }

        [RequiredIf("HasRegistrationNumber", true, ErrorMessage = "Enter a Companies House or charity registration number")]
        [RequiredLength(8, ErrorMessage = "Your Companies House number must be 8 characters long.")]
        [AcceptedCharacters(@"^[a-zA-Z0-9]*$", ErrorMessage = "Your Companies House number must contain only letters and numbers.")]
        public string? CompanyRegistrationNumber { get; set; }

        [RequiredIf("HasRegistrationNumber", false, ErrorMessage = "Enter a D-U-N-S number")]        
        [RequiredLength(9, ErrorMessage = "Your D-U-N-S number must be 9 characters long")]
        [AcceptedCharacters(@"^[0-9]+$", ErrorMessage = "Your D-U-N-S number must contain only numbers")]
        public string? DUNSNumber { get; set; }        
        public bool? HasParentCompany { get; set; }

        [RequiredIf("HasParentCompany",true, ErrorMessage = "Enter the registered name of your parent company")]
        [MaximumLength(160, ErrorMessage = "Your company's registered name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "Your parent company's registered name must contain only letters, numbers and accepted characters.")]
        public string? ParentCompanyRegisteredName { get; set; }

        [RequiredIf("HasParentCompany", true,ErrorMessage = "Enter the location of your parent company")]
        public string? ParentCompanyLocation { get; set; }
    }
}
