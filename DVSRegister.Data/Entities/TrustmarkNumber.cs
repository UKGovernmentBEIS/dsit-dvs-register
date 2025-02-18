using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{

    [PrimaryKey(nameof(CompanyId), nameof(ServiceNumber))] 
    public class TrustmarkNumber
    {       

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }     
     
        public int CompanyId { get; set; }       
        public int ServiceNumber { get; set; }
       
        //Generated using company id and service id
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string TrustMarkNumber { get; set; }
        public int ServiceKey { get; set; }
        public DateTime TimeStamp { get; set; } 
    }
}
