using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models.DSITEdit2i
{
    public class ManualUnderPinningServiceDraftDto
    {
        public int Id { get; set; }
        public string? ServiceName { get; set; }
        public string? ProviderName { get; set; }

        public string? SelectedCabName { get; set; } // not for saving to db, just for display in difference screen
        public int? CabId { get; set; }
        public CabDto Cab { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }
    }
}
