using DVSRegister.BusinessLogic.Models.PreRegistration;

namespace DVSRegister.Models
{
    public class CountryViewModel
    {
        public bool? IsApplicationSponsor { get; set; }
        public List<CountryDto> AvailableCountries { get; set; }       
        public List<int> SelectedCountryIds { get; set; }
        public List<CountryDto> SelectedCountries { get; set; }
    }
}
