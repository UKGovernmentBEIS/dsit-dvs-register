using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class SignUpViewModel :EnterEmailViewModel
    {    


        [Required(ErrorMessage = "Enter a valid password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "The MFA code must be a 6-digit number.")]
        public string MFACode { get; set; }
      
    }
}