using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class EnterEmailViewModel
    {
        [Required(ErrorMessage = "Enter an email address")]        
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        public string Email { get; set; }
        public bool? PasswordReset { get; set; }
    }
}
