using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class RemovalReasons
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RemovalReasonId { get; set; }

        [Required]
        [MaxLength(255)]
        public string RemovalReason { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        public DateTime TimeUpdated { get; set; } = DateTime.UtcNow;

        public bool IsActiveReason { get; set; } = true;

        public bool RequiresAdditionalInfo { get; set; } = false;
    }
}
