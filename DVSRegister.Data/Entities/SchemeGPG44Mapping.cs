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

        [ForeignKey("QualityLevel")]
        public int QualityLevelId { get; set; }
        public QualityLevel QualityLevel { get; set; }

        [ForeignKey("ServiceSupSchemeMapping")]
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMapping ServiceSupSchemeMapping { get; set; }


    }
}
