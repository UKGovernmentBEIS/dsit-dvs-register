using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ConfirmPasswordViewModel
    {       

        [Required(ErrorMessage = "Enter a valid password")]
        [MinLength(8, ErrorMessage = "Enter a password that is at least 8 characters long")]
        [DataType(DataType.Password)]       
        public string? Password { get; set; }


        [Required(ErrorMessage = "Enter a valid password")]
        [MinLength(8, ErrorMessage = "Enter a password that is at least 8 characters long")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Enter the same password in both fields")]
        public string? ConfirmPassword { get; set; }

        public bool? PasswordReset { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

