using System.ComponentModel.DataAnnotations;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB
{
    public class RoleViewModel : ServiceSummaryBaseViewModel, IValidatableObject
    {
        public List<RoleDto>? AvailableRoles { get; set; }

        public List<int>? SelectedRoleIds { get; set; }

        public List<RoleDto>? SelectedRoles { get; set; }

        public decimal? TrustFrameworkVersion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedRoleIds == null || !SelectedRoleIds.Any())
            {
                var message = TrustFrameworkVersion switch
                {
                    0.4m => "Select the roles from the UK digital identity and attributes trust framework that apply to the provider's service",
                    _ => "Select the roles from the UK digital verification services trust framework that apply to the provider's service"
                };

                yield return new ValidationResult(message, [nameof(SelectedRoleIds)]);
            }
        }
    }
}