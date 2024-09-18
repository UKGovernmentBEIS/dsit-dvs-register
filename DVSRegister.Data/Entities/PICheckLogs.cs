using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class PICheckLogs
    {
        public PICheckLogs() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ReviewTypeEnum ReviewType { get; set; }
        public string Comment { get; set; }

        [ForeignKey("PublicInterestCheck")]
        public int PublicInterestCheckId { get; set; }
        public PublicInterestCheck PublicInterestCheck { get; set; }      

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime LogTime { get; set; }
    }


       
}
