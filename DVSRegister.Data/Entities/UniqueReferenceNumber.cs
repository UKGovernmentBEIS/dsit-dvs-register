using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
	public class UniqueReferenceNumber : BaseEntity
	{
        public UniqueReferenceNumber() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string URN { get; set; }

        public string? RegisteredDIPName { get; set; }

        public DateTime? LastCheckedTimeStamp { get; set; }

        public DateTime? ReleasedTimeStamp { get; set; }

        public string? CheckedByCAB { get; set; }

        public int? Validity { get; set; }

        public string? Status { get; set; }

    }
}