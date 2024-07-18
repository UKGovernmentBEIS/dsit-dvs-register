using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DVSRegister.Data.Entities
{
    public class RegisterPublishLog
    {
        public RegisterPublishLog() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Provider")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        public string? ProviderName { get; set; }
        public string? Services { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
