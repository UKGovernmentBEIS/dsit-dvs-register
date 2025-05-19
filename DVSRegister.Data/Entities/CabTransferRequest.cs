using DVSRegister.CommonUtility.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class CabTransferRequest
    {
        public CabTransferRequest() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }

        [ForeignKey("CabUser")]
        public int FromCabUserId { get; set; }       
        public CabUser FromCabUser { get; set; }

        [ForeignKey("Cab")]
        public int ToCabId { get; set; }     
        public Cab ToCab { get; set; }       
        public ServiceStatusEnum PreviousServiceStatus { get; set; }        
        public bool CertificateUploaded { get; set; }      

        [ForeignKey("RequestManagement")]
        public string  RequestManagementId { get; set; }
        public RequestManagement RequestManagement { get; set; }
        public DateTime DecisionTime { get; set; }

    }
}
