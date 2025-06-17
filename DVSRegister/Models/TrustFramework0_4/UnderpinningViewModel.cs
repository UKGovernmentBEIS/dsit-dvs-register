namespace DVSRegister.Models.TrustFramework0_4
{
    public class UnderpinningViewModel
    {
        public UnderpinType? SelectedOption { get; set; }
    }
    
    public enum UnderpinType
    {
        Underpinning,
        WhiteLabelled,
        Neither
    }
}




