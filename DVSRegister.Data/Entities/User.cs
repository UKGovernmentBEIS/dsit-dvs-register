using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class User 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }// User’s email address (used for communication and Cognito identity attributes)
        public string? UserName { get; set; }  // Unique username mapped to Cognito user (primary identifier in user pool) not mandatory in application
        public string? FullName { get; set; } // Display name entered by the user
        public bool IsActive { get; set; }// Application-level activation flag modified on delete or disable
        public AccountStatusEnum AccountStatus { get; set; } // Represents user lifecycle state aligned with Cognito status
        public string? Profile { get; set; } //DEV or DSIT user   
        public UserRoleEnum UserRole { get; set; }// Application role used for authorization (separate from Cognito groups/roles)      
        public ICollection<OfDIAUserRemoval>? OfDIAUserRemoval { get; set; }  // Related records capturing user removal history for audit purposes, removed user can be added again    
        public DateTime? CreatedDate { get; set; }      // Timestamp when the user was invited by OFDIA admin
        public DateTime? ModifiedDate { get; set; } // Timestamp of last update (edit/delete) performed by admin 
        public DateTime? LastLoggedIn { get; set; }   // Last successful login timestamp (tracked at application level, not directly from Cognito)
    }
}
