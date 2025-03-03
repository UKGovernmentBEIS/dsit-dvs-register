using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceIdentityProfileMappingDraft
    {
        public ServiceIdentityProfileMappingDraft() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ServiceDraft")]
        public int ServiceDraftId { get; set; }
        public ServiceDraft ServiceDraft { get; set; }

        [ForeignKey("IdentityProfile")]
        public int IdentityProfileId { get; set; }
        public IdentityProfile IdentityProfile { get; set; }
    }
}
