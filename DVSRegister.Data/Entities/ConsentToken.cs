using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Entities
{
    [Index(nameof(TokenId))]
    [Index(nameof(Token))]
    public class ConsentToken 
    {
        public ConsentToken() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [ForeignKey("CertificateReview")]
        public int CertificateReviewId { get; set; }
        public CertificateReview CertificateReview { get; set; }
        public string TokenId { get; set; }
        public string Token {  get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
