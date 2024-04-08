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
        [AcceptedLength(255, ErrorMessage = "Enter an email address that is less than 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]
        [UKPhoneNumber(ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]       
        public string TelephoneNumber { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
