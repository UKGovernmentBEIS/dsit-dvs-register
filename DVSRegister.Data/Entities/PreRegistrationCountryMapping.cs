using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class PreRegistrationCountryMapping
    {
        public PreRegistrationCountryMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("PreRegistration")]
        public int PreRegistrationId { get; set; }
        public PreRegistration PreRegistration { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
