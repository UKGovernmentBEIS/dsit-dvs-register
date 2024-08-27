using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class LoginViewModel : EnterEmailViewModel
    {
        [Required(ErrorMessage = "Enter a password")]      
        public string Password { get; set; }
    }
}
