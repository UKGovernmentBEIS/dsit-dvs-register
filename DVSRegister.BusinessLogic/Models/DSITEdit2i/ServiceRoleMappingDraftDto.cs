using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class ServiceRoleMappingDraftDto
    {
        public int Id { get; set; }
        public int ServiceDraftId { get; set; }
        public ServiceDraftDto ServiceDraft { get; set; }
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
    }
}