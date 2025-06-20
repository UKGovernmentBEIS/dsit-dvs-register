using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class ServiceSupSchemeMapping
    {
        public ServiceSupSchemeMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("SupplementaryScheme")]
        public int SupplementarySchemeId { get; set; }
        public SupplementaryScheme SupplementaryScheme { get; set; }

        public bool? HasGpg44Mapping { get; set; }

        public ICollection<SchemeGPG44Mapping> SchemeGpg44Mapping { get; set; }

        public ICollection<SchemeGPG45Mapping> SchemeGpg45Mapping { get; set; }
    }
}
