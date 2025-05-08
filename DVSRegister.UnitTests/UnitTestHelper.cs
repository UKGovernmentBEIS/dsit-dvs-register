using DVSRegister.CommonUtility.Models;
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


        public static Service CreateService(int cabUserId, string serviceName, int providerProfileId, ServiceStatusEnum serviceStatus, 
            bool? hasGpg44, bool? hasGpg45, bool? hasSupplementarySchemes, int serviceKey)
        {
            var service = new Service
            {

            ServiceKey = serviceKey,
            ProviderProfileId = providerProfileId,
             ServiceName = serviceName,           
            WebSiteAddress = "https://www.sample-service.com",
            CompanyAddress = "123 Sample St, Sample City, SC 12345",
            ServiceRoleMapping =
            [
                 new() { RoleId = 1},
                 new() { RoleId = 2 }             
            ],
            HasGPG44 = hasGpg44,
            ServiceQualityLevelMapping = Convert.ToBoolean(hasGpg44) ? new List<ServiceQualityLevelMapping>
            {
                new() {  QualityLevelId = 1 },
                new() {  QualityLevelId = 4 } 
            }
            : [],
            HasGPG45 = hasGpg45,
            ServiceIdentityProfileMapping =Convert.ToBoolean(hasGpg45) ? new List<ServiceIdentityProfileMapping>() 
            { 
             new() { IdentityProfileId = 1 } ,
             new() { IdentityProfileId = 2 }
            }
            : [],
            HasSupplementarySchemes = hasSupplementarySchemes,
            ServiceSupSchemeMapping = Convert.ToBoolean(hasSupplementarySchemes) ? new List<ServiceSupSchemeMapping>()
            {
             new() { SupplementarySchemeId = 1 } ,
             new() { SupplementarySchemeId = 2 }
            }
            : [],
            FileName = "sample-file.pdf",
            FileLink = "test.pdf",
            FileSizeInKb = 150.5m, 
            ConformityIssueDate = DateTime.Now,
            ConformityExpiryDate = DateTime.Now.AddYears(2),
            CabUserId = cabUserId,
            ServiceStatus = serviceStatus            

            };

            return service;
        }

    }
}
