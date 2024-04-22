using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    public class UKPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;
           
            if (phoneNumber != null)
            {
                phoneNumber = phoneNumber.Replace(" ", "");
                if (!IsValidUKPhoneNumber(phoneNumber))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private bool IsValidUKPhoneNumber(string phoneNumber)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                PhoneNumber parsedNumber = phoneNumberUtil.Parse(phoneNumber, "GB");
                return phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
