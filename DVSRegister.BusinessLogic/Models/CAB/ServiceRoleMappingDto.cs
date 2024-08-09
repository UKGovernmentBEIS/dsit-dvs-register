namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ServiceRoleMappingDto
    {
       
        public int Id { get; set; }       
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }     
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
    }
}
