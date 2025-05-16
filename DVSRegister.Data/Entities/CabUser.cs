using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class CabUser
    {
        public CabUser() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Cab")]
        public int CabId { get; set; }
        public Cab Cab { get; set; }
        public string CabEmail { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
