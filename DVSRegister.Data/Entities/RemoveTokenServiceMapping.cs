using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class RemoveTokenServiceMapping
    {
        public RemoveTokenServiceMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("RemoveProviderToken")]
        public int RemoveProviderTokenId { get; set; }
        public RemoveProviderToken RemoveProviderToken { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
