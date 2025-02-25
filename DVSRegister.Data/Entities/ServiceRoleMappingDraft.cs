using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class ServiceRoleMappingDraft
    {
        public ServiceRoleMappingDraft() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ServiceDraft")]
        public int ServiceDraftId { get; set; }
        public ServiceDraft ServiceDraft { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}