namespace DVSRegister.CommonUtility.Models
{
    public class ProviderEditRequestReceived
    {
        public string Id { get; set; } 
        public string ProviderName { get; set; }
        public string PreviousData { get; set; }
        public string CurrentData { get; set; }
    }
}
