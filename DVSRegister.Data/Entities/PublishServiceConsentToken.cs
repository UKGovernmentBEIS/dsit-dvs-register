using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Entities
{
    [Index(nameof(TokenId))]
    [Index(nameof(Token))]
    public class PublishServiceConsentToken
    {
        public PublishServiceConsentToken() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
       
        public string TokenId { get; set; }
        public string Token {  get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
