using DVSRegister.CommonUtility;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ConfirmPasswordViewModel
    {       

        [Required(ErrorMessage = Constants.EnterPassword)]
        [MinLength(8, ErrorMessage = Constants.EnterPassword)]
        [DataType(DataType.Password)]       
        public string? Password { get; set; }


        [Required(ErrorMessage = Constants.EnterPassword)]
        [MinLength(8, ErrorMessage = Constants.EnterPassword)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Enter the same password in both fields")]
        public string? ConfirmPassword { get; set; }

        public bool? PasswordReset { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

