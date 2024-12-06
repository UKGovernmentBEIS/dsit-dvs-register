namespace DVSRegister.Validations;

using Microsoft.AspNetCore.Mvc.ModelBinding;


public class ValidationHelper
{
    public record FieldComparisonConfig(
        string FirstFieldName,
        string SecondFieldName,
        string ErrorMessage
        );
    
    public static void ValidateDuplicateFields(
        ModelStateDictionary modelState,
        string? primaryValue,
        string? secondaryValue,
        FieldComparisonConfig config)
    {
        if (!string.IsNullOrEmpty(primaryValue) &&
            !string.IsNullOrEmpty(secondaryValue) &&
            primaryValue.Equals(secondaryValue, StringComparison.OrdinalIgnoreCase))
        { 
            modelState.AddModelError(config.SecondFieldName, config.ErrorMessage);
        }
    }
    
    

}