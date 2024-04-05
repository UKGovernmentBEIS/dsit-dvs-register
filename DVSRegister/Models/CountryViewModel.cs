using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;
namespace DVSRegister.Models
{
    public class CountryViewModel
    {
        public bool? IsApplicationSponsor { get; set; }
        public List<CountryDto>? AvailableCountries { get; set; }

        [EnsureMinimumCount (ErrorMessage = "Select atleast once country your company trades in.")]
        public List<int>? SelectedCountryIds { get; set; }
        public List<CountryDto>? SelectedCountries { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
