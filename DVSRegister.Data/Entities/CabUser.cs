using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    // Represents an CAB portal application user, synchronized with Cognito User Pool identity
    public class CabUser
    {
  
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Cab")]
        public int CabId { get; set; }
        public Cab? Cab { get; set; }
        public  string CabEmail { get; set; } // User’s email address (used for communication and Cognito identity attributes)
        public  string? UserName { get; set; }// Unique username mapped to Cognito user (primary identifier in user pool) not mandatory in application
        public string? FullName { get; set; } // Display name entered by the user    
        public AccountStatusEnum AccountStatus { get; set; } // Represents user lifecycle state aligned with Cognito status
        public DateTime CreatedTime { get; set; }  // Timestamp when the user was invited by OFDIA admin
        public DateTime? ModifiedDate { get; set; } // Timestamp of last update (edit/delete) performed by admin 
        public DateTime? LastLoggedIn { get; set; }   // Last successful login timestamp (tracked at application level, not directly from Cognito)
        public ICollection<CabUserRemoval>? CabUserRemoval { get; set; }  // Related records capturing cab user removal history for audit purposes, removed user can be added again

    }
}
