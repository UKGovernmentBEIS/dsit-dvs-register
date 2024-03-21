namespace DVSRegister.Models
{

    public class SponsorViewModel
    {
        public string? FullName { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? SponsorFullName { get; set; }
        public string? SponsorJobTitle { get; set; }
        public string? SponsorEmail { get; set; }
        public string? SponsorTelephoneNumber { get; set; }

        public ContactViewModel? ContactViewModel { get; set; }
    }
}
