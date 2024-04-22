using DVSRegister.BusinessLogic.Models.PreRegistration;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models
{
    public class SummaryViewModel
    {
        [Required(ErrorMessage = "Select yes if you are the application sponsor.")]
        public bool? IsApplicationSponsor { get; set; }       

        [MustBeTrue(ErrorMessage = "Select to confirm that the information you have provided is correct.")]
        public bool ConfirmAccuracy { get; set; }
        public SponsorViewModel? SponsorViewModel { get; set; }
        public CountryViewModel? CountryViewModel { get; set; }
        public CompanyViewModel? CompanyViewModel { get; set; }      
       
    }
}
