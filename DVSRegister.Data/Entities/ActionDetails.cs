using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ActionDetails
    {
        public ActionDetails() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ActionDetailsKey { get; set; }
        public string ActionDescription { get; set; }

        [ForeignKey("ActionCategory")]
        public int ActionCategoryId { get; set; }
        public ActionCategory ActionCategory { get; set; }        

    }
}
