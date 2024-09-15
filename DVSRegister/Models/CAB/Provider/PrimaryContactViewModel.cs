using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class PrimaryContactViewModel
    {
        [Required(ErrorMessage = "Enter a full name")]
        public string? PrimaryContactFullName { get; set; }

        [Required(ErrorMessage = "Enter a job title")]
        public string? PrimaryContactJobTitle { get; set; }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? PrimaryContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        public string? PrimaryContactTelephoneNumber { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
