using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CAB
{
    public class CabRepository : ICabRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<PreRegistrationRepository> logger;

        public CabRepository(DVSRegisterDbContext context, ILogger<PreRegistrationRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }
        public async Task<GenericResponse> SaveCertificateInformation(Provider provider)
        {

            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.Provider.Include(p => p.CertificateInformation)
               .FirstOrDefaultAsync(p => p.PreRegistrationId == provider.PreRegistrationId);

                if (existingProvider !=null)
                {
                    existingProvider.RegisteredName  = provider.RegisteredName;
                    existingProvider.TradingName = provider.TradingName;
                    existingProvider.PublicContactEmail = provider.PublicContactEmail;
                    existingProvider.TelephoneNumber = provider.TelephoneNumber;
                    existingProvider.WebsiteAddress = provider.WebsiteAddress;
                    existingProvider.Address = provider.Address;                  
                    // at a time only one certificate info is passed from UI
                    existingProvider.CertificateInformation.Add(provider.CertificateInformation.ToList()[0]);
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = existingProvider.Id;
                }
                else
                {
                    provider.CreatedTime = DateTime.UtcNow;
                    var entity = await context.Provider.AddAsync(provider);
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = entity.Entity.Id;
                }
                transaction.Commit();
                genericResponse.Success = true;
              
            }
            catch (Exception ex)
            {               
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }

        public async Task<UniqueReferenceNumber> GetURNDetails(string URN)
        {
            var uniqueReferenceNumber = await context.UniqueReferenceNumber.FirstOrDefaultAsync(e => e.URN == URN);
            return uniqueReferenceNumber;
        }

        public async Task<PreRegistration> GetPreRegistrationDetails(string URN)
        {
            var preRegistration = await context.PreRegistration.FirstOrDefaultAsync(e => e.URN == URN);
            return preRegistration;
        }

        public async Task<List<Role>> GetRoles()
        {
            return await context.Role.OrderBy(c => c.RoleName).ToListAsync();
        }

        public async Task<List<IdentityProfile>> GetIdentityProfiles()
        {
            return await context.IdentityProfile.OrderBy(c => c.IdentityProfileName).ToListAsync();
        }

        public async Task<List<SupplementaryScheme>> GetSupplementarySchemes()
        {
            return await context.SupplementaryScheme.OrderBy(c => c.SchemeName).ToListAsync();
        }

        public async Task<GenericResponse> UpdateURNStatus(UniqueReferenceNumber uniqueReferenceNumber)
        {

            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingEntity = await context.UniqueReferenceNumber.FirstOrDefaultAsync(e => e.URN == uniqueReferenceNumber.URN);
                if (existingEntity != null)
                {
                    existingEntity.URNStatus = uniqueReferenceNumber.URNStatus;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    genericResponse.Success = true;
                }

            }
            catch (Exception ex)
            {
                genericResponse.EmailSent = false;
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex.Message);
            }
            return genericResponse;

        }

        public async Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);

                if (existingProvider !=null)
                {
                    existingProvider.RegisteredName = providerProfile.RegisteredName;
                    existingProvider.TradingName = providerProfile.TradingName;
                    existingProvider.HasRegistrationNumber = providerProfile.HasRegistrationNumber;
                    existingProvider.CompanyRegistrationNumber = providerProfile.CompanyRegistrationNumber;
                    existingProvider.DUNSNumber = providerProfile.DUNSNumber;
                    existingProvider.PrimaryContactFullName= providerProfile.PrimaryContactFullName;
                    existingProvider.PrimaryContactJobTitle= providerProfile.PrimaryContactJobTitle;
                    existingProvider.PrimaryContactEmail = providerProfile.PrimaryContactEmail;
                    existingProvider.PrimaryContactTelephoneNumber = providerProfile.PrimaryContactTelephoneNumber;
                    existingProvider.SecondaryContactFullName = providerProfile.SecondaryContactFullName;
                    existingProvider.SecondaryContactJobTitle = providerProfile.SecondaryContactJobTitle;
                    existingProvider.SecondaryContactEmail = providerProfile.SecondaryContactEmail;
                    existingProvider.SecondaryContactTelephoneNumber = providerProfile.SecondaryContactTelephoneNumber;
                    existingProvider.PublicContactEmail= providerProfile.PublicContactEmail;
                    existingProvider.ProviderTelephoneNumber = providerProfile.ProviderTelephoneNumber;
                    existingProvider.ProviderWebsiteAddress = providerProfile.ProviderWebsiteAddress;
                    existingProvider.ProviderStatus = providerProfile.ProviderStatus;
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                }
                else
                {
                    providerProfile.CreatedTime = DateTime.UtcNow;
                    var entity = await context.ProviderProfile.AddAsync(providerProfile);
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = entity.Entity.Id;
                }
                transaction.Commit();
                genericResponse.Success = true;

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveProviderProfile");
            }
            return genericResponse;
        }

        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName)
        {
            var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.RegisteredName == registeredName);

            if (existingProvider !=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<GenericResponse> SaveService(Service service)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingService = await context.Service.FirstOrDefaultAsync(p => p.ProviderProfileId == service.ProviderProfileId && p.ServiceName == service.ServiceName);

                if (existingService !=null)
                {
                    existingService.ServiceName = service.ServiceName;
                    existingService.WebsiteAddress = service.WebsiteAddress;
                    existingService.CompanyAddress = service.CompanyAddress;
                    existingService.ServiceRoleMapping = service.ServiceRoleMapping;
                    existingService.ServiceQualityLevelMapping = service.ServiceQualityLevelMapping;
                    existingService.ServiceIdentityProfileMapping =service.ServiceIdentityProfileMapping;
                    existingService.HasSupplementarySchemes = service.HasSupplementarySchemes;
                    existingService.HasGPG44 = service.HasGPG44;
                    existingService.ServiceSupSchemeMapping = service.ServiceSupSchemeMapping;
                    existingService.FileName = service.FileName;
                    existingService.FileLink = service.FileLink;
                    existingService.FileSizeInKb =service.FileSizeInKb;
                    existingService.ConformityIssueDate = service.ConformityIssueDate;
                    existingService.ConformityExpiryDate =service.ConformityExpiryDate;
                    existingService.CabUserId = service.CabUserId;
                    existingService.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                }
                else
                {
                    service.CreatedTime = DateTime.UtcNow;
                    var entity = await context.Service.AddAsync(service);
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = entity.Entity.Id;
                }
                transaction.Commit();
                genericResponse.Success = true;

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveService");
            }
            return genericResponse;
        }
    }
}
