using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB.Provider
{
    public class ProviderListViewModel
    {
        public List<ProviderProfileDto>? Providers { get; set; }
        public string? SearchText { get; set; }
        public bool HasPendingReAssignments {  get; set; }
        public List<CabTransferRequestDto> PendingCertificateUploads { get; set; }
        public string? ProviderServiceNames {  get; set; }
    }
}
