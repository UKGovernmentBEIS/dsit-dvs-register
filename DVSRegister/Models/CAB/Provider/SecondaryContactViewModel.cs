using DVSRegister.Models.CAB.Provider;
using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class SecondaryContactViewModel : ProviderSummaryBaseViewModel
    {
        [Required(ErrorMessage = "Enter a full name")]
        [MaximumLength(160, ErrorMessage = "The full name must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-zÀ-ž '\-]+$", ErrorMessage = "The full name must contain only letters and accepted characters")]
        public string? SecondaryContactFullName { get; set; }

        [Required(ErrorMessage = "Enter a job title")]
        [MaximumLength(160, ErrorMessage = "The job title must be less than 161 characters")]
        [AcceptedCharacters(@"^[A-Za-zÀ-ž0-9 &'\-,.()]+$", ErrorMessage = "The job title must contain only letters, numbers and accepted characters")]
        public string? SecondaryContactJobTitle { get; set; }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaximumLength(255, ErrorMessage = "Enter an email address that is less than 255 characters")]
        public string? SecondaryContactEmail { get; set; }

        [Required(ErrorMessage = "Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000")]
        [UKPhoneNumber(ErrorMessage = "Enter a valid UK telephone number")]
        public string? SecondaryContactTelephoneNumber { get; set; }

      
    }
}
