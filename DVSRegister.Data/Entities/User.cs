using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class User :BaseEntity
    {
        public User() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
