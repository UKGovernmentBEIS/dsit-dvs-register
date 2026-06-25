using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Models
{
    public class CabUserDto
    {
        public int Id { get; set; }
        public int CabId { get; set; }     
        public string CabEmail { get; set; }
        public CabDto Cab { get; set; }      
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        
        public AccountStatusEnum AccountStatus { get; set; }
        public DateTime CreatedTime { get; set; }  
        public DateTime? ModifiedDate { get; set; } 
        public DateTime? LastLoggedIn { get; set; } 
       
    }
}
