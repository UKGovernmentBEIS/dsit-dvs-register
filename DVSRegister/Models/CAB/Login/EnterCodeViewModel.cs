using DVSRegister.CommonUtility;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class EnterCodeViewModel
    {
        [Required(ErrorMessage = Constants.EnterCode)]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = Constants.EnterCode)]
        public string Code { get; set; }

        public string? Email { get; set; }

        public bool? PasswordReset { get; set; }
    }
}

