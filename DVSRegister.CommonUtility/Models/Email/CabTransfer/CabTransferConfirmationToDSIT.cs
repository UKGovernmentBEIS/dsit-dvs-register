using Notify.Models;

namespace DVSRegister.CommonUtility.Models.Email
{
    public class CabTransferConfirmationToDSIT
    {
        public string Id { get; set; }
        public string ExistingCabName { get; set; }
        public string AcceptingCabName { get; set; }
        public string ProviderName { get; set; }
        public string ServiceName { get; set; }
    }

        
}
