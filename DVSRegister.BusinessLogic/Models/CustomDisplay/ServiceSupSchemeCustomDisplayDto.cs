using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models

{
    public class ServiceSupSchemeCustomDisplayDto
    {

        public int Id { get; set; }  
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMappingDto? ServiceSupSchemeMapping { get; set; }
        public bool IsSupplementarySchemeHidden { get; set; }
     
 
    }
}
