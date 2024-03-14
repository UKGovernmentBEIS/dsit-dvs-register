using DVSRegister.BusinessLogic.Models.PreAssessment;

namespace DVSRegister.Models
{
    public class CountryViewModel
    {
        public List<CountryDto> AvailableCountries { get; set; }        
        public List<int> SelectedCountryIds { get; set; }
    }
}
