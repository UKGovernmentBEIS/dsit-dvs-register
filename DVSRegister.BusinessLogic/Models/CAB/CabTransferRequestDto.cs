using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Models
{
    public class CabTransferRequestDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto ProviderProfile { get; set; }
        public int FromCabUserId { get; set; }
        public CabUserDto FromCabUser { get; set; }
        public int ToCabId { get; set; }
        public CabDto ToCab { get; set; }
        public ServiceStatusEnum PreviousServiceStatus { get; set; }
        public bool CertificateUploaded { get; set; }
        public string RequestManagementId { get; set; }
        public RequestManagementDto RequestManagement { get; set; }
        public DateTime DecisionTime { get; set; }
    }
}
