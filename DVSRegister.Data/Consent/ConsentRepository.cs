using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Repositories
{
    public class ConsentRepository : IConsentRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<ConsentRepository> logger;

        public ConsentRepository (DVSRegisterDbContext context, ILogger<ConsentRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }




        #region Opening Loop     

        public async Task<ProceedApplicationConsentToken> GetProceedApplicationConsentToken(string token, string tokenId)
        {
            return await context.ProceedApplicationConsentToken.Include(p => p.Service).ThenInclude(p => p.Provider)
            .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId)??new ProceedApplicationConsentToken();
        }

        public async Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedinUserEmail)
        {
            var consent = await context.ProceedApplicationConsentToken.FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);            

            if (consent != null)
            {
                var service = await context.Service.FirstOrDefaultAsync(s => s.Id == consent.ServiceId);
                service.OpeningLoopTokenStatus = TokenStatusEnum.RequestCompleted;
                context.ProceedApplicationConsentToken.Remove(consent);
                await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.RemoveOpeningLoopToken, loggedinUserEmail);
                logger.LogInformation("Opening Loop : Token Removed for service {0}", consent.Service.ServiceName);
                return true;
            }
            return false;
        }

        // opening loop - update service status to received
        public async Task<GenericResponse> UpdateServiceStatus(int serviceId, ServiceStatusEnum serviceStatus, string providerEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var service = await context.Service.FirstOrDefaultAsync(e => e.Id == serviceId);
                if (service != null)
                {
                    service.ServiceStatus = serviceStatus;
                    service.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.OpeningLoop, providerEmail);
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

        #endregion


        #region closing the loop

       

        public async Task<ProceedPublishConsentToken> GetProceedPublishConsentToken(string token, string tokenId)
        {
            return await context.ProceedPublishConsentToken.Include(p => p.Service).FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId)??new ProceedPublishConsentToken();
        }
     
        public async Task<bool> RemoveProceedPublishConsentToken(string token, string tokenId, string loggedInUserEmail)
        {
            var consent = await context.ProceedPublishConsentToken.Include(p => p.Service)
           .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);
            
            var service = await context.Service.FirstOrDefaultAsync(s => s.Id == consent.ServiceId);
            service.ClosingLoopTokenStatus = TokenStatusEnum.RequestCompleted;

            if (consent != null)
            {
                context.ProceedPublishConsentToken.Remove(consent);
                await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.RemoveClosingLoopToken, loggedInUserEmail);
                logger.LogInformation("Closing Loop : Token Removed for service {0}", consent.Service.ServiceName);
                return true;                
            }

            return false;
        }

        // closing the loop
        public async Task<GenericResponse> UpdateServiceAndProviderStatus(int serviceId, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {

                var serviceEntity = await context.Service.FirstOrDefaultAsync(e => e.Id == serviceId);               

                if (serviceEntity != null )
                {                   

                    serviceEntity.ServiceStatus = ServiceStatusEnum.ReadyToPublish;
                    serviceEntity.ModifiedTime = DateTime.UtcNow;
               
                    var providerEntity = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(e => e.Id == serviceEntity.ProviderProfileId);
                    ProviderStatusEnum providerStatus = RepositoryHelper.GetProviderStatus(providerEntity.Services, providerEntity.ProviderStatus);
                    providerEntity.ProviderStatus = providerStatus;
                    providerEntity.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.ClosingTheLoop, loggedInUserEmail);

                    if (await AddTrustMarkNumber(serviceEntity.Id,serviceEntity.ServiceKey, providerEntity.Id, loggedInUserEmail))
                    {
                        transaction.Commit();
                        genericResponse.Success = true;
                    }
                    else
                    {
                        transaction.Rollback();
                        genericResponse.Success = false;
                    }

                   
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

        private async Task<bool> AddTrustMarkNumber(int serviceId, int serviceKey, int providerId, string loggedInUserEmail)
        {
            bool success = false;
            try
            {
                int serviceNumber;
                int companyId;
                var existingTrustmarkWithServiceKey = await context.TrustmarkNumber.FirstOrDefaultAsync(t => t.ServiceKey == serviceKey);
                if(existingTrustmarkWithServiceKey == null)
                {
                    var existingTrustmark = await context.TrustmarkNumber.FirstOrDefaultAsync(t => t.ProviderProfileId == providerId);
                    if (existingTrustmark != null)
                    {
                        // If it exists, select the existing CompanyId
                        // select max of service number, if doesnt exist set as 0                    
                        serviceNumber = await context.TrustmarkNumber.Where(p => p.ProviderProfileId == providerId).MaxAsync(p => (int?)p.ServiceNumber) ?? 0;
                        companyId = existingTrustmark.CompanyId;

                    }
                    else
                    {
                        //If doesn't exist, get max company id or return initial value as 199 and then increment by 1
                        int maxCompanyId = await context.TrustmarkNumber.MaxAsync(t => (int?)t.CompanyId) ?? 199;
                        companyId = maxCompanyId + 1;
                        serviceNumber = 0; // service number initialize to 0 if doesnt exist
                    }

                    TrustmarkNumber trustmarkNumber = new()
                    {
                        ProviderProfileId = providerId,
                        ServiceId = serviceId,
                        CompanyId = companyId,
                        ServiceNumber = serviceNumber + 1, // service id start with 1 
                        TimeStamp = DateTime.UtcNow,
                        ServiceKey = serviceKey

                    };

                    await context.TrustmarkNumber.AddAsync(trustmarkNumber);
                    await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.TrustmarkNumberGeneration, loggedInUserEmail);
                    success = true;
                }
                else
                {
                    success = true; // if already TM number with same service key exists , donot generate new TM number
                    Console.WriteLine("TM number already generated for first version:ProviderId{0} ServiceId: {1} ServiceKey: {2}", providerId, serviceId, serviceKey);
                }
                
            }
            catch (Exception ex)
            {
                success = false;
                logger.LogError($"Failed to generate trustmark number: {ex}");
               Console.WriteLine("ProviderId:{0} serviceId: {1}", providerId, serviceId);
            }
            return success;

        }
        #endregion

        public async Task<Service> GetServiceDetails(int serviceId)
        {

            var baseQuery = context.Service
            .Where(p => p.Id == serviceId)
            .Include(p => p.Provider)
            .Include(p => p.CertificateReview)
            .Include(p => p.ProceedApplicationConsentToken)
             .Include(p => p.TrustFrameworkVersion)
             .Include(p => p.UnderPinningService)
             .ThenInclude(p => p.CabUser).ThenInclude(cu => cu.Cab)
             .Include (p => p.UnderPinningService)
             .ThenInclude(p => p.Provider)
            .Include(p => p.ManualUnderPinningService).ThenInclude(x => x.Cab)
            .Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Include(p => p.ServiceRoleMapping)
            .ThenInclude(s => s.Role).AsSplitQuery();



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

                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)
               .ThenInclude(ssm => ssm.SchemeGPG44Mapping).ThenInclude(ssm => ssm.QualityLevel);

                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)
                    .ThenInclude(ssm => ssm.SchemeGPG45Mapping).ThenInclude(ssm => ssm.IdentityProfile);
            }
            if (await baseQuery.AnyAsync(p => p.ServiceIdentityProfileMapping != null && p.ServiceIdentityProfileMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceIdentityProfileMapping)
                    .ThenInclude(ssm => ssm.IdentityProfile);
            }
            var service = await queryWithOptionalIncludes.FirstOrDefaultAsync() ?? new Service();


            return service;
        }

        public async Task<Service> GetService(int serviceId)
        {
            return await context.Service.FirstOrDefaultAsync(e => e.Id == serviceId)??new();
        }

        public async Task<List<Service>> GetServiceList(int providerId)
        {
            return await context.Service.Where(s => s.ProviderProfileId == providerId).ToListAsync();
        }

    }
}
