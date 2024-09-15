using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class EnterCodeViewModel
    {
        [Required(ErrorMessage = "Enter a valid code")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "The code must be a 6-digit number.")]
        public string Code { get; set; }

        public string? Email { get; set; }

        public bool? PasswordReset { get; set; }
    }
}

