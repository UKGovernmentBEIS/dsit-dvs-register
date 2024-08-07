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
        public string CAB { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedTime { get; set; }       
    }
}
