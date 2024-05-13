using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class LoginPageViewModel
    {
        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com.")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@([a-zA-Z0-9.-]+\.)?gov\.uk$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Enter a valid password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "The MFA code must be a 6-digit number.")]
        public string MFACode { get; set; }
    }
}