namespace DVSRegister.Models
{
    public class CompanyViewModel
    {
        public string RegisteredCompanyName { get; set; }

        public string TradingName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public bool? HasParentCompany { get; set; }
        public string? ParentCompanyRegisteredName { get; set; }

        public string? ParentCompanyLocation { get; set; }
    }
}
