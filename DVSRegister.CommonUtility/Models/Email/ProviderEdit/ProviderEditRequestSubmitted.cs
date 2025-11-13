namespace DVSRegister.CommonUtility.Models
{
    public class ProviderEditRequestSubmitted
    {
        public string Id { get; set; }
        public string RecipientName { get; set; }
        public string ProviderName { get; set; }       
        public string PreviousData { get; set; }
        public string CurrentData { get; set; }
    }
}
