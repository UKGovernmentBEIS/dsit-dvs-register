using DVSRegister.CommonUtility.Models.Enums;
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

        [ForeignKey("PreRegistration")]
        public int? PreRegistrationId { get; set; }
        public PreRegistration PreRegistration { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string URN { get; set; }

        public string? RegisteredDIPName { get; set; }

        public DateTime? LastCheckedTimeStamp { get; set; }

        public DateTime? ReleasedTimeStamp { get; set; }

        public string? CheckedByCAB { get; set; }

        public int? Validity { get; set; }

        public URNStatusEnum URNStatus { get; set; }
       

    }
}