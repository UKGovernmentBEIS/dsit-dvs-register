using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ConfirmPasswordViewModel
    {       

        [Required(ErrorMessage = "Enter a valid password")]
        [MinLength(8, ErrorMessage = "Password length must be minimum 8 characters")]
        [DataType(DataType.Password)]       
        public string? Password { get; set; }


        [Required(ErrorMessage = "Enter a valid password")]
        [MinLength(8, ErrorMessage = "Password length must be minimum 8 characters")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public bool? PasswordReset { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

