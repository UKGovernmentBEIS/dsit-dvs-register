using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CAB
{
    public class CabRepository : ICabRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CabRepository> logger;

        public CabRepository(DVSRegisterDbContext context, ILogger<CabRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }
    

        public async Task<List<Role>> GetRoles()
        {
            return await context.Role.OrderBy(c => c.Order).ToListAsync();
        }

        public async Task<List<IdentityProfile>> GetIdentityProfiles()
        {
            return await context.IdentityProfile.OrderBy(c => c.IdentityProfileName).ToListAsync();
        }

        public async Task<List<SupplementaryScheme>> GetSupplementarySchemes()
        {
            return await context.SupplementaryScheme.OrderBy(c => c.Order).ToListAsync();
        }

        public async Task<List<QualityLevel>> QualityLevels()
        {
            return await context.QualityLevel.ToListAsync();
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

        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId)
        {
            var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.RegisteredName.ToLower() == registeredName.ToLower() && p.Id != providerId);

            if (existingProvider !=null)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                providerQuery = providerQuery.Where(p => p.Services.Any(s => EF.Functions.TrigramsSimilarity(s.ServiceName.ToLower(), searchText.ToLower()) > .1));
            }
            var searchResults = await providerQuery.ToListAsync();
            return searchResults;
        }        

        public async Task<ProviderProfile> GetProvider(int providerId,int cabId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p=>p.Services).ThenInclude(p=>p.CertificateReview)
            .Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == providerId && p.CabUser.CabId == cabId).OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime).FirstOrDefaultAsync() ?? new ProviderProfile();
            return provider;
        }

            public async Task<Service> GetServiceDetailsWithProvider(int serviceId, int cabId)
            {

            Service service = new();
            service = await context.Service.Include(p => p.CabUser).ThenInclude(p => p.Cab)
            .Include(p => p.Provider)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId).FirstOrDefaultAsync() ?? new Service();
            return service;

            }

            public async Task<Service> GetServiceDetails(int serviceId, int cabId)
            {

            var baseQuery = context.Service.Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId)
             .Include(p => p.CertificateReview)
            .Include(p => p.ServiceRoleMapping)           
            .ThenInclude(s => s.Role);

           
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

            if (await baseQuery.AnyAsync(p => p.ServiceIdentityProfileMapping != null && p.ServiceIdentityProfileMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceIdentityProfileMapping)
                    .ThenInclude(ssm => ssm.IdentityProfile);
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


        #region Save/update
        public async Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                providerProfile.CreatedTime = DateTime.UtcNow;
                var entity = await context.ProviderProfile.AddAsync(providerProfile);
                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddProvider, loggedInUserEmail);
                genericResponse.InstanceId = entity.Entity.Id;
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
        public async Task<GenericResponse> SaveService(Service service, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {

                var existingService = await context.Service.Include(x=>x.ServiceRoleMapping).Include(x=>x.ServiceIdentityProfileMapping)
                .Include(x=>x.ServiceQualityLevelMapping).Include(x=>x.ServiceSupSchemeMapping)              
                 .Where(x=>x.Id == service.Id).FirstOrDefaultAsync();                
                if(existingService != null && existingService.Id>0) 
                {
                    
                    existingService.ServiceName = service.ServiceName;
                    existingService.WebSiteAddress = service.WebSiteAddress;
                    existingService.CompanyAddress = service.CompanyAddress;

                    if (existingService.ServiceRoleMapping != null & existingService.ServiceRoleMapping?.Count > 0)
                        context.ServiceRoleMapping.RemoveRange(existingService.ServiceRoleMapping);            
                    existingService.ServiceRoleMapping = service.ServiceRoleMapping ;

                    if (existingService.ServiceIdentityProfileMapping != null & existingService.ServiceIdentityProfileMapping?.Count > 0)
                        context.ServiceIdentityProfileMapping.RemoveRange(existingService.ServiceIdentityProfileMapping);
                    existingService.ServiceIdentityProfileMapping = service.ServiceIdentityProfileMapping;

                    if (existingService.ServiceQualityLevelMapping != null & existingService.ServiceQualityLevelMapping?.Count > 0)
                        context.ServiceQualityLevelMapping.RemoveRange(existingService.ServiceQualityLevelMapping);

                    existingService.ServiceQualityLevelMapping = service.ServiceQualityLevelMapping;
                    existingService.HasSupplementarySchemes = service.HasSupplementarySchemes;
                    existingService.HasGPG44 = service.HasGPG44 ;
                    existingService.HasGPG45 = service.HasGPG45;
                    if (existingService.ServiceSupSchemeMapping != null & existingService.ServiceSupSchemeMapping?.Count > 0)
                        context.ServiceSupSchemeMapping.RemoveRange(existingService.ServiceSupSchemeMapping);
                    existingService.ServiceSupSchemeMapping = service.ServiceSupSchemeMapping;
                    existingService.FileLink = service.FileLink;
                    existingService.FileName = service.FileName;
                    existingService.FileSizeInKb = service.FileSizeInKb;
                    existingService.ConformityIssueDate = service.ConformityIssueDate;
                    existingService.ConformityExpiryDate = service.ConformityExpiryDate;                  
                    existingService.ServiceStatus = service.ServiceStatus;
                    existingService.ModifiedTime = DateTime.UtcNow;                   
                    genericResponse.InstanceId = existingService.Id;
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.SaveAsDraftService, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;
                }
                else
                {
                    // Get the current highest ServiceNumber for the given ProviderProfileId
                    var serviceNumbers = await context.Service.Where(s => s.ProviderProfileId == service.ProviderProfileId)
                    .Select(s => s.ServiceNumber).ToListAsync();
                    int nextServiceNumber = serviceNumbers.Any() ? serviceNumbers.Max() + 1 : 1;
                    service.ServiceNumber = nextServiceNumber;
                    service.CreatedTime = DateTime.UtcNow;
                    if(service.ServiceStatus == ServiceStatusEnum.SavedAsDraft)
                    {
                        service.ModifiedTime = DateTime.UtcNow;
                    }
                    AttachListToDbContext(service);
                    var entity = await context.Service.AddAsync(service);
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddService, loggedInUserEmail);
                    genericResponse.InstanceId = entity.Entity.Id;
                    transaction.Commit();
                    genericResponse.Success = true;
                }

              

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveService");
            }
            return genericResponse;
        }

     

        public async Task<GenericResponse> UpdateCompanyInfo(ProviderProfile providerProfile, string loggedInUserEmail)
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
                    if (existingProvider.HasParentCompany && !string.IsNullOrEmpty(providerProfile.ParentCompanyLocation))
                    {
                        existingProvider.ParentCompanyLocation = providerProfile.ParentCompanyLocation;
                    }
                    if (existingProvider.HasParentCompany && !string.IsNullOrEmpty(providerProfile.ParentCompanyRegisteredName))
                    {
                        existingProvider.ParentCompanyRegisteredName = providerProfile.ParentCompanyRegisteredName;
                    }
                    if (existingProvider.HasRegistrationNumber && !string.IsNullOrEmpty(providerProfile.CompanyRegistrationNumber))
                    {
                        existingProvider.CompanyRegistrationNumber = providerProfile.CompanyRegistrationNumber;
                    }
                    else
                    {
                        existingProvider.DUNSNumber = providerProfile.DUNSNumber;
                    }

                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.CompanyInfoUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in UpdateCompanyInfo");
            }
            return genericResponse;
        }
        public async Task<GenericResponse> UpdatePrimaryContact(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);
                if (existingProvider !=null)
                {
                    existingProvider.PrimaryContactFullName= providerProfile.PrimaryContactFullName;
                    existingProvider.PrimaryContactJobTitle= providerProfile.PrimaryContactJobTitle;
                    existingProvider.PrimaryContactEmail = providerProfile.PrimaryContactEmail;
                    existingProvider.PrimaryContactTelephoneNumber = providerProfile.PrimaryContactTelephoneNumber;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.PrimaryContactUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in UpdatePrimaryContact");
            }
            return genericResponse;
        }
        public async Task<GenericResponse> UpdateSecondaryContact(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);
                if (existingProvider !=null)
                {
                    existingProvider.SecondaryContactFullName= providerProfile.SecondaryContactFullName;
                    existingProvider.SecondaryContactJobTitle= providerProfile.SecondaryContactJobTitle;
                    existingProvider.SecondaryContactEmail = providerProfile.SecondaryContactEmail;
                    existingProvider.SecondaryContactTelephoneNumber = providerProfile.SecondaryContactTelephoneNumber;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.SecondaryContactUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in UpdateSecondaryContact");
            }
            return genericResponse;
        }
        public async Task<GenericResponse> UpdatePublicProviderInformation(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);
                if (existingProvider !=null)
                {
                    existingProvider.PublicContactEmail= providerProfile.PublicContactEmail;
                    existingProvider.ProviderTelephoneNumber = providerProfile.ProviderTelephoneNumber;
                    existingProvider.ProviderWebsiteAddress = providerProfile.ProviderWebsiteAddress;
                    existingProvider.ProviderStatus = providerProfile.ProviderStatus;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.PublicContactUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in UpdatePublicProviderInformation");
            }
            return genericResponse;
        }
        #endregion


        #region private methods

        // For event logs, need to attach each item to context
        private void AttachListToDbContext(Service service)
        {
            if(service.ServiceRoleMapping != null)
            {
                foreach (var mapping in service.ServiceRoleMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                }
            }
          
            if(service.ServiceIdentityProfileMapping!=null)
            {
                foreach (var mapping in service.ServiceIdentityProfileMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                }
            }


            if (service.ServiceQualityLevelMapping != null)
            {
                foreach (var mapping in service.ServiceQualityLevelMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                } 
            }
            if (service.ServiceSupSchemeMapping !=null )
            {
                foreach (var mapping in service.ServiceSupSchemeMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                } 
            }
        }
        #endregion
    }
}
