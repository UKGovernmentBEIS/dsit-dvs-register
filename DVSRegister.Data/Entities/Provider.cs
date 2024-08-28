using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;
using NpgsqlTypes;

namespace DVSRegister.Data.Entities
{
    public class Provider 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("PreRegistration")]
        public int PreRegistrationId { get; set; }
        public PreRegistration PreRegistration { get; set; }
        public string RegisteredName { get; set; }
        public string TradingName { get; set; }
        public string PublicContactEmail { get; set; }
        public string TelephoneNumber { get; set; }
        public string WebsiteAddress { get; set; }
        public string Address { get; set; }      
        public ProviderStatusEnum ProviderStatus { get;set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? PublishedTime { get;set; }      
        public NpgsqlTsVector SearchVector { get; set; }

    }
}
