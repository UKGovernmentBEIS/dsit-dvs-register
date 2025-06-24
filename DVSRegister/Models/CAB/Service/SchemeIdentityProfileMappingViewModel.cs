using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB.Service
{
    public class SchemeIdentityProfileMappingViewModel 
    {      
        public int SchemeId { get; set; }
        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG45")]
        public bool? HasGPG45 { get; set; }
        public IdentityProfileViewModel IdentityProfile { get; set; }

    }
}
