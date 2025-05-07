using DVSRegister.Data.Entities;

namespace DVSRegister.UnitTests
{
    public static class UnitTestHelper
    {
        public static ProviderProfile CreateProviderProfile(int cabUserId, string registeredName)
        {
            var providerProfile = new ProviderProfile
            {
                RegisteredName = registeredName,
                TradingName = "ABC Trading",
                HasRegistrationNumber = true,
                CompanyRegistrationNumber = "AC012345",
                HasParentCompany = false,
                ParentCompanyRegisteredName = null,
                ParentCompanyLocation = null,
                PrimaryContactFullName = "John Doe",
                PrimaryContactJobTitle = "Manager",
                PrimaryContactEmail = "john.doe@abccorp.com",
                PrimaryContactTelephoneNumber = "123-456-7890",
                SecondaryContactFullName = "Jane Smith",
                SecondaryContactJobTitle = "Assistant Manager",
                SecondaryContactEmail = "jane.smith@abccorp.com",
                SecondaryContactTelephoneNumber = "098-765-4321",
                PublicContactEmail = "contact@abccorp.com",
                ProviderTelephoneNumber = "555-555-5555",
                ProviderWebsiteAddress = "www.abccorp.com",
                CabUserId = cabUserId

            };

            return providerProfile;
        }

        public static ProviderProfile CreateProviderProfileWithDunsNumber(int cabUserId, string registeredName)
        {
            var providerProfile = new ProviderProfile
            {
                RegisteredName = registeredName,
                TradingName = "ABC Trading",
                HasRegistrationNumber = false,
                CompanyRegistrationNumber = null,
                DUNSNumber = "12345678",
                HasParentCompany = false,
                ParentCompanyRegisteredName = null,
                ParentCompanyLocation = null,
                PrimaryContactFullName = "John Doe",
                PrimaryContactJobTitle = "Manager",
                PrimaryContactEmail = "john.doe@abccorp.com",
                PrimaryContactTelephoneNumber = "123-456-7890",
                SecondaryContactFullName = "Jane Smith",
                SecondaryContactJobTitle = "Assistant Manager",
                SecondaryContactEmail = "jane.smith@abccorp.com",
                SecondaryContactTelephoneNumber = "098-765-4321",
                PublicContactEmail = "contact@abccorp.com",
                ProviderTelephoneNumber = "555-555-5555",
                ProviderWebsiteAddress = "www.abccorp.com",
                CabUserId = cabUserId

            };

            return providerProfile;
        }

        public static ProviderProfile CreateProviderProfileWithParentCompany(int cabUserId, string registeredName)
        {
            var providerProfile = new ProviderProfile
            {
                RegisteredName = registeredName,
                TradingName = "ABC Trading",
                HasRegistrationNumber = false,
                CompanyRegistrationNumber = null,
                DUNSNumber = "12345678",
                HasParentCompany = true,
                ParentCompanyRegisteredName = "Test Parent",
                ParentCompanyLocation = "USA",
                PrimaryContactFullName = "John Doe",
                PrimaryContactJobTitle = "Manager",
                PrimaryContactEmail = "john.doe@abccorp.com",
                PrimaryContactTelephoneNumber = "123-456-7890",
                SecondaryContactFullName = "Jane Smith",
                SecondaryContactJobTitle = "Assistant Manager",
                SecondaryContactEmail = "jane.smith@abccorp.com",
                SecondaryContactTelephoneNumber = "098-765-4321",
                PublicContactEmail = "contact@abccorp.com",
                ProviderTelephoneNumber = "555-555-5555",
                ProviderWebsiteAddress = "www.abccorp.com",
                CabUserId = cabUserId

            };

            return providerProfile;
        }

       
    }
}
