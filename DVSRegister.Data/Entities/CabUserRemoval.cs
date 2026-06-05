using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    //Removal is soft delete only - in the applicatopn perspective it is delete
    public class CabUserRemoval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("CabUser")]
        public int CabUserId { get; set; }
        public CabUser? CabUser { get; set; }
        public CabUserRemovalReasonEnum? RemovalReason { get; set; }
        public string? Comment { get; set; }
        public RemovalStatus RemovalStatus { get; set; }
        public DateTime? RequestTime { get; set; }
    }
}
