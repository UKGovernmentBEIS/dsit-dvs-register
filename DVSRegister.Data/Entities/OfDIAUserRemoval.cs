using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    //Removal is soft delete only - in the application's view it is delete
    public class OfDIAUserRemoval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public OfDIAUserRemovalReasonEnum? RemovalReason { get; set; }
        public string? Comment { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovedTime { get; set; }
       
    }
}
