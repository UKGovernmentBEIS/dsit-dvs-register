using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models
{
    public class ContactViewModel
    {

        [Required(ErrorMessage = "Enter your full name.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Enter your job title.")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com.")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com.")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        [UKPhoneNumber(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]       
        public string TelephoneNumber { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
