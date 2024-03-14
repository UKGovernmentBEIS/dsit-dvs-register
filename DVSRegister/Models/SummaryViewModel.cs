using DVSRegister.BusinessLogic.Models.PreAssessment;

namespace DVSRegister.Models
{
    public class SummaryViewModel
    {
        public bool IsApplicationSponsor { get; set; }
        public bool NotApplicationSponsor { get; set; }
        public bool ConfirmAccuracy { get; set; }
        public SponsorViewModel SponsorViewModel { get; set; }
        public CountryViewModel CountryViewModel { get; set; }
        public CompanyViewModel CompanyViewModel { get; set; }
        public List<CountryDto> SelectedCountries { get; set; }
    }
}
