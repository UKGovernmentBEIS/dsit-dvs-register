using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class SecondaryContactViewModel
    {
        [Required(ErrorMessage = "Enter your full name")]
        public string? SecondaryContactFullName { get; set; }

        [Required(ErrorMessage = "Enter your job title")]
        public string? SecondaryContactJobTitle { get; set; }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? SecondaryContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        public string? SecondaryContactTelephoneNumber { get; set; }

        public bool FromSummaryPage { get; set; }
    }
}
