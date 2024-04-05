using DVSRegister.BusinessLogic.Models.PreRegistration;

namespace DVSRegister.Models
{
    public class SummaryViewModel
    {
        public bool IsApplicationSponsor { get; set; }
        public bool NotApplicationSponsor { get; set; }

        [MustBeTrue(ErrorMessage = "Select to confirm that the information you have provided is correct.")]
        public bool ConfirmAccuracy { get; set; }
        public SponsorViewModel? SponsorViewModel { get; set; }
        public CountryViewModel? CountryViewModel { get; set; }
        public CompanyViewModel? CompanyViewModel { get; set; }      
       
    }
}
