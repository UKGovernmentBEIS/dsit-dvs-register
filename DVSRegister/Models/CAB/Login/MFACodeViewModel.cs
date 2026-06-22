using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class MFACodeViewModel
    {
        [Required(ErrorMessage = "Enter MFA code")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Enter the code shown in your authenticator app. It is 6 digits long")]
        public string MFACode { get; set; }
    }
}
