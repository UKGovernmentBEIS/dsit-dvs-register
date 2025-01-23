using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class MFACodeViewModel
    {
        [Required(ErrorMessage = "Enter MFA code")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "The MFA code must be a six-digit number")]
        public string MFACode { get; set; }
    }
}
