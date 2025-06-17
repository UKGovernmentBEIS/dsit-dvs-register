using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class SchemeGPG44Mapping
    {

        public SchemeGPG44Mapping() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }    

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("QualityLevel")]
        public int QualityLevelId { get; set; }
        public QualityLevel QualityLevel { get; set; }

        [ForeignKey("SupplementaryScheme")]
        public int SupplementarySchemeId { get; set; }
        public SupplementaryScheme SupplementaryScheme { get; set; }
    }
}
