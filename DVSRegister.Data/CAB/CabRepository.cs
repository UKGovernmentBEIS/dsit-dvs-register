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

        public async Task<List<QualityLevel>> QualityLevels()
        {
            return await context.QualityLevel.ToListAsync();
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
            var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.RegisteredName.ToLower() == registeredName.ToLower());

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
                    // Get the current highest ServiceNumber for the given ProviderProfileId
                    var serviceNumbers = await context.Service.Where(s => s.ProviderProfileId == service.ProviderProfileId)
                    .Select(s => s.ServiceNumber).ToListAsync();
                    int nextServiceNumber = serviceNumbers.Any()? serviceNumbers.Max() + 1 :1;                   
                    service.ServiceNumber = nextServiceNumber;               
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



        public async Task<List<ProviderProfile>> GetProviders(int cabId, string searchText = "")
        {
            //Filter based on cab type as well, fetch records for users with same cab type
            IQueryable<ProviderProfile> providerQuery = context.ProviderProfile.Include(p => p.Services).Include(p => p.CabUser)
            .ThenInclude(cu => cu.Cab)
            .Where(p => p.CabUser.CabId == cabId)
            .OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime);
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim().ToLower();              
                providerQuery = providerQuery.Where(p => p.SearchVector.Matches(searchText) ||
                                                         p.Services.Any(s => s.SearchVector.Matches(searchText)));
            }           
            var searchResults = await providerQuery.ToListAsync();
            return searchResults;
        }

        public async Task<ProviderProfile> GetProvider(int providerId,int cabId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p=>p.Services).Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == providerId && p.CabUser.CabId == cabId).OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime).FirstOrDefaultAsync() ?? new ProviderProfile();
            return provider;
        }

        public async Task<Service> GetServiceDetails(int serviceId, int cabId)
        {          

            var baseQuery = context.Service.Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId)
            .Include(p => p.ServiceRoleMapping)
            .ThenInclude(s => s.Role)
            .Include(p => p.ServiceIdentityProfileMapping)
            .ThenInclude(p => p.IdentityProfile);

           
            IQueryable<Service> queryWithOptionalIncludes = baseQuery;
            if (await baseQuery.AnyAsync(p => p.ServiceQualityLevelMapping != null && p.ServiceQualityLevelMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceQualityLevelMapping)
                    .ThenInclude(sq => sq.QualityLevel);
            }

            if (await baseQuery.AnyAsync(p => p.ServiceSupSchemeMapping != null && p.ServiceSupSchemeMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)
                    .ThenInclude(ssm => ssm.SupplementaryScheme);
            }           
            var service = await queryWithOptionalIncludes.FirstOrDefaultAsync() ?? new Service();


            return service;
        }

        public async Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p => p.CabUser).ThenInclude(p=>p.Cab).Where(x=>x.Id == providerId).FirstOrDefaultAsync()??new ProviderProfile();
            if(provider.CabUser.Cab.Id > 0 &&  provider.CabUser.Cab.Id == cabId)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
