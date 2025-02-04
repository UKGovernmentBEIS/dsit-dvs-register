using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models
{
    public class CabUserDto
    {
        public int Id { get; set; }
        public int CabId { get; set; }     
        public string CabEmail { get; set; }
        public CabDto Cab { get; set; }
    }
}
