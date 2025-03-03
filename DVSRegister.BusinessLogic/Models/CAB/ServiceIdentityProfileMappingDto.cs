using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ServiceIdentityProfileMappingDto
    {
        public int Id { get; set; }
    
        public int ServiceId { get; set; }
        public Service Service { get; set; }      
        public int IdentityProfileId { get; set; }
        public IdentityProfileDto IdentityProfile { get; set; }
    }
}
