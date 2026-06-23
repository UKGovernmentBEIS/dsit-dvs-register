using DVSRegister.CommonUtility;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class MFACodeViewModel
    {
        [Required(ErrorMessage = Constants.EnterMFACode)]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = Constants.EnterMFACode)]
        public string MFACode { get; set; }
    }
}
