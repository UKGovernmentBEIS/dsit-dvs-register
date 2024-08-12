using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class ServiceQualityLevelMapping
    {
        public ServiceQualityLevelMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("QualityLevel")]
        public int QualityLevelId { get; set; }
        public QualityLevel QualityLevel { get; set; }
    }
}

