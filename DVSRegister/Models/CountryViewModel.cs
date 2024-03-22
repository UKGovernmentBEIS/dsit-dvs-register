using DVSRegister.BusinessLogic.Models.PreRegistration;

namespace DVSRegister.Models
{
    public class CountryViewModel
    {
       
        public List<CountryDto> AvailableCountries { get; set; }       
        public List<int> SelectedCountryIds { get; set; }
        public List<CountryDto> SelectedCountries { get; set; }
    }
}
