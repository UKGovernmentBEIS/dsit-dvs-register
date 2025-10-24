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
        public async Task<GenericResponse> UpdateServiceStatus(int serviceId, string providerEmail, string agree)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var service = await context.Service.FirstOrDefaultAsync(e => e.Id == serviceId);
                if (service != null)
                {
                    if (agree == "accept")
                    {
                        service.ServiceStatus = ServiceStatusEnum.Received;
                    }
                    else
                    {
                        var certificateReview = service.CertificateReview.Where(x => x.IsLatestReviewVersion).SingleOrDefault();
                        certificateReview.CertificateReviewStatus = CertificateReviewEnum.DeclinedByProvider;
                    }
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
