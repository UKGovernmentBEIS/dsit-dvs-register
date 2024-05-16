using System;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class MFARegistrationViewModel
    {
        public string SecretToken { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "The MFA code must be a 6-digit number.")]
        public string MFACode { get; set; }
    }
}

