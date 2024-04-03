namespace DVSRegister.Models
{

    public class SponsorViewModel
    {      
        public string? SponsorFullName { get; set; }
        public string? SponsorJobTitle { get; set; }
        public string? SponsorEmail { get; set; }
        public string? SponsorTelephoneNumber { get; set; }
        public ContactViewModel? ContactViewModel { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
