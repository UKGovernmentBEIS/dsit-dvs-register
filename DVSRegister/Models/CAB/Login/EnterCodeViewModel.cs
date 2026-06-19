using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class EnterCodeViewModel
    {
        [Required(ErrorMessage = "Enter a valid code")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Enter the code you received in your email. It is 6 digits long")]
        public string Code { get; set; }

        public string? Email { get; set; }

        public bool? PasswordReset { get; set; }
    }
}

