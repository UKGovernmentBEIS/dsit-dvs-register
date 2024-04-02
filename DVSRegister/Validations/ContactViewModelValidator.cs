using System.Text.RegularExpressions;
using DVSRegister.Extensions;
using DVSRegister.Models;
using PhoneNumbers;

namespace DVSRegister.Validations
{
	public class ContactViewModelValidator
	{
		public ContactViewModelValidator()
		{
			
		}

        public static void ValidateContactViewModel(HttpContext httpContext, ContactViewModel contactViewModel)
        {
            httpContext?.Session.Set("IsFullNameValid", ValidateFullName(contactViewModel.FullName));
            httpContext?.Session.Set("IsEmailValid", ValidateEmail(contactViewModel.Email));
            httpContext?.Session.Set("IsJobTitleValid", ValidateJobTitle(contactViewModel.JobTitle));
            httpContext?.Session.Set("IsPhoneNumberValid", ValidatePhoneNumber(contactViewModel.TelephoneNumber));
        }

        public static bool IsContactModelValid(HttpContext httpContext)
        {
            return !(bool)(httpContext?.Session.Get<bool>("IsFullNameValid")) || !(bool)(httpContext?.Session.Get<bool>("IsEmailValid")) || !(bool)(httpContext?.Session.Get<bool>("IsJobTitleValid")) || !(bool)(httpContext?.Session.Get<bool>("IsPhoneNumberValid"));
        }

        public static void ValidateSponsorViewModel(HttpContext httpContext, SponsorViewModel sponsorViewModel)
        {
            httpContext?.Session.Set("IsSponsorFullNameValid", ValidateFullName(sponsorViewModel.SponsorFullName));
            httpContext?.Session.Set("IsSponsorEmailValid", ValidateEmail(sponsorViewModel.SponsorEmail));
            httpContext?.Session.Set("IsSponsorJobTitleValid", ValidateJobTitle(sponsorViewModel.SponsorJobTitle));
            httpContext?.Session.Set("IsSponsorPhoneNumberValid", ValidatePhoneNumber(sponsorViewModel.SponsorTelephoneNumber));
        }

        public static bool IsSponsorModelValid(HttpContext httpContext)
        {
            return !(bool)(httpContext?.Session.Get<bool>("IsSponsorFullNameValid")) || !(bool)(httpContext?.Session.Get<bool>("IsSponsorEmailValid")) || !(bool)(httpContext?.Session.Get<bool>("IsSponsorJobTitleValid")) || !(bool)(httpContext?.Session.Get<bool>("IsSponsorPhoneNumberValid"));      
        }

        public static bool ValidateFullName(string fullName)
        {
            if (fullName != null && fullName.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateEmail(string email)
        {
            if(email == null)
            {
                return false;
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public static bool ValidateJobTitle(string jobTitle)
        {
            if (jobTitle != null && jobTitle.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                phoneNumber = phoneNumber.Replace(" ", "");
                PhoneNumber number = phoneUtil.Parse(phoneNumber, "GB");
                
                var result = phoneUtil.IsValidNumber(number);
                return result;
            }
            catch(Exception)
            {
                return false;
            }            
        }

        public static void RetrieveAndSetValidationParametersForContactModel(dynamic ViewBag, HttpContext httpContext)
        {   
            ViewBag.IsFullNameValid = httpContext?.Session.GetString("IsFullNameValid");
            ViewBag.IsEmailValid = httpContext?.Session.GetString("IsEmailValid");
            ViewBag.IsJobTitleValid = httpContext?.Session.GetString("IsJobTitleValid");
            ViewBag.IsPhoneNumberValid = httpContext?.Session.GetString("IsPhoneNumberValid");
        }

        public static void RetrieveAndSetValidationParametersForSponsorModel(dynamic ViewBag, HttpContext httpContext)
        {
            ViewBag.IsSponsorFullNameValid = httpContext?.Session.GetString("IsSponsorFullNameValid");
            ViewBag.IsSponsorEmailValid = httpContext?.Session.GetString("IsSponsorEmailValid");
            ViewBag.IsSponsorJobTitleValid = httpContext?.Session.GetString("IsSponsorJobTitleValid");
            ViewBag.IsSponsorPhoneNumberValid = httpContext?.Session.GetString("IsSponsorPhoneNumberValid");
        }
    }
}