using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class RemovalRequestViewModel
    {
        public int ProviderId { get; set; }
        public int ServiceKey { get; set; }
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Enter details on the reason for removal")]
        [MaximumLength(1000, ErrorMessage = "The removal reason must be less than 1001 characters")]
        [AcceptedCharacters(@"^[A-Za-zÀ-ž0-9 &@£$€¥(){}!«»""'''?""/*=#%+.,:;\r\n\\/-]+$", ErrorMessage = "The removal reason must contain only letters, numbers and accepted characters")]
        public string RemovalReasonByCab { get; set; }   
        public bool IsProviderRemoval {  get; set; }
    }
}
