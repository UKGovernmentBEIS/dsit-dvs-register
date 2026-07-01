namespace DVSRegister.BusinessLogic.Models
{
    public class TrustFrameworkVersionDto
    {       
      
        public int Id { get; set; }
        public string TrustFrameworkName { get; set; }

        public Decimal Version { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
