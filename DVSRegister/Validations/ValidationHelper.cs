namespace DVSRegister.Validations;

using DVSRegister.CommonUtility;
using DVSRegister.Models;
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


    public static DateTime? ValidateIssueDate(DateViewModel dateViewModel, DateTime? expiryDate, bool fromSummaryPage, ModelStateDictionary modelstate, bool isTFVersion0_4 = false)
    {
        DateTime? date = null;
        DateTime minDate = new DateTime(1900, 1, 1);
        DateTime minIssueDate;


        try
        {
            if (dateViewModel.Day == null || dateViewModel.Month == null || dateViewModel.Year == null)
            {
                if (dateViewModel.Day == null)
                {
                    modelstate.AddModelError("Day", Constants.ConformityIssueDayError);
                }
                if (dateViewModel.Month == null)
                {
                    modelstate.AddModelError("Month", Constants.ConformityIssueMonthError);
                }
                if (dateViewModel.Year == null)
                {
                    modelstate.AddModelError("Year", Constants.ConformityIssueYearError);
                }
            }
            else
            {
                date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                if (date > DateTime.Today)
                {
                    modelstate.AddModelError("ValidDate", Constants.ConformityIssuePastDateError);
                }
                if (date < minDate)
                {
                    modelstate.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);
                }

                if (expiryDate.HasValue && fromSummaryPage)
                {
                    minIssueDate = isTFVersion0_4? expiryDate.Value.AddYears(-3).AddDays(-59): expiryDate.Value.AddYears(-2).AddDays(-59);
                    if (date < minIssueDate)
                    {
                        modelstate.AddModelError("ValidDate", isTFVersion0_4 ? Constants.ConformityMaxExpiryDateErrorTF0_4: Constants.ConformityMaxExpiryDateError);
                    }
                }

            }
        }
        catch (Exception)
        {
            modelstate.AddModelError("ValidDate", Constants.ConformityIssueDateInvalidError);

        }
        return date;
    }

    public static DateTime? ValidateExpiryDate(DateViewModel dateViewModel, DateTime issueDate, ModelStateDictionary modelstate,bool isTFVersion0_4 = false, int years = 2, int days = 59)
    {
        DateTime? date = null;

        try
        {
            if (dateViewModel.Day == null || dateViewModel.Month == null || dateViewModel.Year == null)
            {
                if (dateViewModel.Day == null)
                {
                    modelstate.AddModelError("Day", Constants.ConformityExpiryDayError);
                }
                if (dateViewModel.Month == null)
                {
                    modelstate.AddModelError("Month", Constants.ConformityExpiryMonthError);
                }
                if (dateViewModel.Year == null)
                {
                    modelstate.AddModelError("Year", Constants.ConformityExpiryYearError);
                }
            }
            else
            {
                date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
                var maxExpiryDate = issueDate.AddYears(years).AddDays(days);
                if (date <= DateTime.Today)
                {
                    modelstate.AddModelError("ValidDate", Constants.ConformityExpiryPastDateError);
                }
                else if (date <= issueDate)
                {
                    modelstate.AddModelError("ValidDate", Constants.ConformityIssueDateExpiryDateError);
                }
                else if (date > maxExpiryDate)
                {
                    modelstate.AddModelError("ValidDate", isTFVersion0_4? Constants.ConformityMaxExpiryDateErrorTF0_4: Constants.ConformityMaxExpiryDateError);
                }
            }

        }
        catch (Exception)
        {
            modelstate.AddModelError("ValidDate", Constants.ConformityExpiryDateInvalidError);

        }
        return date;
    }

    public static DateTime? ValidateCustomExpiryDate(DateViewModel dateViewModel, DateTime issueDate, ModelStateDictionary modelstate)
    {
        DateTime? date = null;

        try
        {
            if (dateViewModel.Day == null || dateViewModel.Month == null || dateViewModel.Year == null)
            {
                if (dateViewModel.Day == null)
                {
                    modelstate.AddModelError("Day", Constants.ConformityExpiryDayError);
                }
                if (dateViewModel.Month == null)
                {
                    modelstate.AddModelError("Month", Constants.ConformityExpiryMonthError);
                }
                if (dateViewModel.Year == null)
                {
                    modelstate.AddModelError("Year", Constants.ConformityExpiryYearError);
                }
            }
            else
            {
                date = new DateTime(Convert.ToInt32(dateViewModel.Year), Convert.ToInt32(dateViewModel.Month), Convert.ToInt32(dateViewModel.Day));
            
                if (date <= DateTime.Today)
                {
                    modelstate.AddModelError("ValidDate", Constants.ConformityExpiryPastDateError);
                }
               
            }

        }
        catch (Exception)
        {
            modelstate.AddModelError("ValidDate", Constants.ConformityExpiryDateInvalidError);

        }
        return date;
    }
    public static bool ValidatePdfSignature(Stream stream)
    {
        if (stream == null || stream.Length < 5) return false;

        long originalPos = stream.Position;
        try
        {
            Span<byte> header = stackalloc byte[5];
            stream.Position = 0;
            int read = stream.Read(header);
            if (read < 5) return false;

            return header[0] == 0x25 && // %
                   header[1] == 0x50 && // P
                   header[2] == 0x44 && // D
                   header[3] == 0x46 && // F
                   header[4] == 0x2D;   // -
        }
        finally
        {
            stream.Position = originalPos;
        }
    }
}