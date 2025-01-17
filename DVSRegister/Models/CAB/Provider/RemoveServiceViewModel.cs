namespace DVSRegister.Models.CAB
{
    public class RemoveServiceViewModel
    {
        public int ProviderId { get; set; }
        
        public string ProviderName { get; set; }
        public string ProviderStatus { get; set; }
        public string PublishedTime { get; set; }
        public string CabName { get; set; }

        public string PublicContactEmail { get; set; }
        public string ProviderTelephoneNumber { get; set; }
        public string ProviderWebsiteAddress { get; set; }

        public string PrimaryContactEmail { get; set; }
        public string SecondaryContactEmail { get; set; }

        public List<RemoveServiceDetails> Services { get; set; } = new List<RemoveServiceDetails>();
    }

    public class RemoveServiceDetails
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string PublicationStatus { get; set; }
    }
}