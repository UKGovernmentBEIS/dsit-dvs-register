using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class ManualUnderPinningServiceDto
    {
       
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ProviderName { get; set; }
        public string CABName { get; set; }
        public CabDto? SelectedCab { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }


    }
}
