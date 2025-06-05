namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ProviderProfileCabMappingDto
    {
        public int Id { get; set; }   
        public int ProviderId { get; set; }
        public ProviderProfileDto ProviderProfile { get; set; }
        public int CabId { get; set; }
        public CabDto Cab { get; set; }
    }
}
