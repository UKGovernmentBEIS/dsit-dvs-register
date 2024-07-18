using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class URNViewModel
    {

        [Required (ErrorMessage = "Enter a unique reference number")]
        public string? URN { get; set; }
        public int PreregistrationId { get; set; }

        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }
    }
}
