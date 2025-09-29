using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class RemovalRequestViewModel
    {
        public int ProviderId { get; set; } 
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Enter details on the reason for removal")]
        public string RemovalReasonByCab { get; set; }   
        public bool IsProviderRemoval {  get; set; }
    }
}
