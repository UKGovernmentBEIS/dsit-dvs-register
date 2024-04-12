using DVSRegister.Models;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class PreregistrationViewModelsTests
    {
        [Fact]
        public void IsApplicationSponsor_Required_Validation_Pass()
        {
            var viewModel = new SummaryViewModel { IsApplicationSponsor = true, ConfirmAccuracy = true };
            var validationResults = ValidateModel(viewModel);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void IsApplicationSponsor_Required_Validation_Fail()
        {
            var viewModel = new SummaryViewModel { ConfirmAccuracy = true };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Select yes if you are the application sponsor.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void ConfirmAccuracy_Required_Validation_Fail()
        {
            var viewModel = new SummaryViewModel { IsApplicationSponsor = true };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Select to confirm that the information you have provided is correct.", validationResults[0].ErrorMessage);
        }


        [Fact]
        public void SponsorEmail_Required_Validation_Fail()
        {

            var viewModel = new SponsorViewModel();
            var validationResults = ValidateModel(viewModel);
            Assert.Equal("Enter the application sponsor's full name.", validationResults[0].ErrorMessage);
            Assert.Equal("Enter the application sponsor's job title.", validationResults[1].ErrorMessage);
            Assert.Equal("Enter an email address in the correct format, like name@example.com.", validationResults[2].ErrorMessage);
            Assert.Equal("Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192", validationResults[3].ErrorMessage);
        }

        [Fact]
        public void SponsorEmail_Format_Validation_Fail()
        {
            var viewModel = new SponsorViewModel
            {
                SponsorEmail = "wrong email",
                SponsorFullName = "test",
                SponsorJobTitle = "test",
                SponsorTelephoneNumber  = "+44 808 157 0192"
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter an email address in the correct format, like name@example.com.", validationResults[0].ErrorMessage);
        }
        [Fact]
        public void SponsorEmail_Length_Validation_Fail()
        {

            var viewModel = new SponsorViewModel
            {
                SponsorFullName = "test",
                SponsorJobTitle = "test",
                SponsorTelephoneNumber  = "+44 808 157 0192",
                SponsorEmail = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@gmail.com"
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter an email address that is less than 255 characters.", validationResults[0].ErrorMessage);
        }


        [Fact]
        public void SponsorTelephoneNumber_Format_Validation_Fail()
        {
            var viewModel = new SponsorViewModel
            {
                SponsorTelephoneNumber = "123",
                SponsorFullName = "test",
                SponsorJobTitle = "test",
                SponsorEmail = "test@test.com"
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void ContactViewModel_Required_Validation_Fail()
        {

            var viewModel = new ContactViewModel();
            var validationResults = ValidateModel(viewModel);
            Assert.Equal("Enter your full name.", validationResults[0].ErrorMessage);
            Assert.Equal("Enter your job title.", validationResults[1].ErrorMessage);
            Assert.Equal("Enter an email address in the correct format, like name@example.com.", validationResults[2].ErrorMessage);
            Assert.Equal("Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000", validationResults[3].ErrorMessage);
        }

        [Fact]
        public void Email_Format_Validation_Fail()
        {
            var viewModel = new ContactViewModel { Email = "wrong email", FullName = "test", JobTitle ="test", TelephoneNumber = "+44 808 157 0192" };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter an email address in the correct format, like name@example.com.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void Email_Length_Validation_Fail()
        {

            var viewModel = new ContactViewModel
            {
                FullName = "test",
                JobTitle ="test",
                TelephoneNumber = "+44 808 157 0192",
                Email = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@gmail.com"
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter an email address that is less than 255 characters.", validationResults[0].ErrorMessage);
        }



        [Fact]
        public void TelephoneNumber_Format_Validation_Fail()
        {

            var viewModel = new ContactViewModel
            {
                FullName = "test",
                JobTitle ="test",
                Email = "test@test.com",
                TelephoneNumber = "123"
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Enter a telephone number, like 01632 960000, 07700 900 000 or +44 20 7946 0000", validationResults[0].ErrorMessage); // Error message should match
        }

        [Fact]
        public void Company_Required_Validation_Fail()
        {

            var viewModel = new CompanyViewModel { HasParentCompany = false };
            var validationResults = ValidateModel(viewModel);
            Assert.Equal("Enter the registered name of your company.", validationResults[0].ErrorMessage);
            Assert.Equal("Enter the trading name of your company.", validationResults[1].ErrorMessage);
            Assert.Equal("Enter a Companies House Number.", validationResults[2].ErrorMessage);
        }

        [Fact]
        public void RegisteredCompanyName_Length_Validation_Fail()
        {

            var viewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                TradingName = "test",
                CompanyRegistrationNumber = "12345678",
                HasParentCompany = false
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Your company's registered name must be less than 161 characters.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void RegisteredCompanyName_AcceptedChar_Validation_Fail()
        {

            var viewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "***",
                TradingName = "test",
                CompanyRegistrationNumber = "12345678",
                HasParentCompany = false
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Your company's registered name must contain only letters, numbers and accepted characters.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void CompanyRegistrationNumber_Length_Validation_Fail()
        {

            var viewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "test",
                TradingName = "test",
                CompanyRegistrationNumber = "1233455676777",
                HasParentCompany = false
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Your Companies House number must be 8 characters long.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void CompanyRegistrationNumber_AccepterChars_Validation_Fail()
        {

            var viewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "test",
                TradingName = "test",
                CompanyRegistrationNumber = "1234567*",
                HasParentCompany = false
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Single(validationResults);
            Assert.Equal("Your Companies House number must contain only letters and numbers.", validationResults[0].ErrorMessage);
        }

        [Fact]
        public void ParentCompanyName_Required_Validation_Fail()
        {

            var viewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "test",
                TradingName = "test",
                CompanyRegistrationNumber = "12345678",
                HasParentCompany = true
            };
            var validationResults = ValidateModel(viewModel);
            Assert.Equal("Enter the registered name of your parent company.", validationResults[0].ErrorMessage);
            Assert.Equal("Enter the location of your parent company.", validationResults[1].ErrorMessage);
        }




        #region
        private static System.Collections.Generic.List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new System.Collections.Generic.List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
        #endregion
    }
}
