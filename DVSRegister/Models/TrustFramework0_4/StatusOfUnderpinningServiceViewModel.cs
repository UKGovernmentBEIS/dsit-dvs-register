namespace DVSRegister.Models.TrustFramework0_4
{
    public class StatusOfUnderpinningServiceViewModel
    {
        public UnderpinningServiceStatus? SelectedOption { get; set; }
    }
    
    public enum UnderpinningServiceStatus
    {
        Published,
        Certified
    }
}




