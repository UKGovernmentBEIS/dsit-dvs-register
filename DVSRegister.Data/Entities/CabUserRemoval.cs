using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    //Removal is soft delete only - in the applicatopn perspective it is delete
    //suspended - means deactivated in userpool
    public class CabUserRemoval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("CabUser")]
        public int CabUserUserId { get; set; }
        public CabUser? CabUser { get; set; }
        public CabUserRemovalReasonEnum? RemovalReason { get; set; }
        public string? Comment { get; set; }
        public RemovalStatus RemovalStatus { get; set; } 
        public DateTime? RemovalTime { get; set; }
    }
}
