using DVSRegister.CommonUtility.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class QualityLevel
    {
        public QualityLevel() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Level { get; set; }
        public QualityTypeEnum QualityType { get; set; }
    }
}
