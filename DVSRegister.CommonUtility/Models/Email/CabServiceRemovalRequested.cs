namespace DVSRegister.CommonUtility.Models
{
    public class CabServiceRemovalRequested
    {
        public string Id { get; set; }
        public string RecipientName { get; set; }
        public string CompanyName { get; set; }
        public string ServiceName { get; set; }
        public string ReasonForRemoval { get; set; }
    }
}
