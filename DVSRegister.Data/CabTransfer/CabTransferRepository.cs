using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CabTransfer
{
    public class CabTransferRepository : ICabTransferRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CabTransferRepository> logger;

        public CabTransferRepository(DVSRegisterDbContext context, ILogger<CabTransferRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
   

        public async Task<Service> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId)
        {

            var baseQuery = context.Service.Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId)
             .Include(p => p.CabTransferRequest).ThenInclude(p=>p.RequestManagement)
              .Include(p => p.Provider)
              .Include(p => p.TrustFrameworkVersion)
              .Include(p => p.UnderPinningService).ThenInclude(p => p.Provider)
              .Include(p => p.UnderPinningService).ThenInclude(p => p.CabUser).ThenInclude(p=>p.Cab)
               .Include(p => p.ManualUnderPinningService).ThenInclude(p=>p.Cab)
            .Include(p => p.ServiceRoleMapping).ThenInclude(s => s.Role);


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
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping).ThenInclude(s=>s.SchemeGPG44Mapping)
                   .ThenInclude(ssm => ssm.QualityLevel);
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG45Mapping)
                  .ThenInclude(ssm => ssm.IdentityProfile);
            }

            if (await baseQuery.AnyAsync(p => p.ServiceIdentityProfileMapping != null && p.ServiceIdentityProfileMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceIdentityProfileMapping)
                    .ThenInclude(ssm => ssm.IdentityProfile);
            }
            var service = await queryWithOptionalIncludes.AsNoTracking().FirstOrDefaultAsync() ?? new Service();


            return service;
        }

        public async Task<CabTransferRequest> GetCabTransferRequestDetails(int requestId)
        {
            return await context.CabTransferRequest.Include(r => r.Service).ThenInclude(r => r.Provider)            
            .Include(r => r.FromCabUser).ThenInclude(s=>s.Cab).Where(r => r.Id == requestId).FirstOrDefaultAsync() ?? new CabTransferRequest();
        }

        public async Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId,int providerProfileId,  string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                
                var entity = await context.CabTransferRequest.Include(c=>c.RequestManagement).Include(c=>c.Service).Where(x=>x.Id == requestId).FirstOrDefaultAsync();
                var cabUser = await context.CabUser.Where(x => x.CabEmail == loggedInUserEmail && x.IsActive).FirstOrDefaultAsync();
                var previousVersions = await context.Service.Where(x => x.ServiceKey == entity.Service.ServiceKey && x.ServiceVersion < entity.Service.ServiceVersion).ToListAsync();
                var inProgressServices = await context.Service.Include(c=>c.CertificateReview).Include(p=>p.PublicInterestCheck).Include(x=>x.ActionLogs)
                    .Where(x => x.ServiceKey == entity.Service.ServiceKey && x.ServiceVersion > entity.Service.ServiceVersion).ToListAsync();
                if (entity != null && entity.RequestManagement != null && entity.Service != null && 
                (entity.Service.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || entity.Service.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign))
                {
                    if(approve)
                    {
                        var existingMapping = await context.ProviderProfileCabMapping.FirstOrDefaultAsync(m => m.CabId == cabUser.CabId && m.ProviderId == providerProfileId);
                        if (existingMapping == null)
                        {
                            await context.ProviderProfileCabMapping.AddAsync(new ProviderProfileCabMapping
                            {
                                CabId = entity.ToCabId,
                                ProviderId = providerProfileId
                            });
                        }
                        entity.RequestManagement.RequestStatus = RequestStatusEnum.Approved;
                        entity.Service.CabUserId =  cabUser.Id;
                        entity.Service.IsCurrent = true;
                        foreach(var previousService in previousVersions)
                        {
                            previousService.CabUserId = cabUser.Id;
                            previousService.ModifiedTime = DateTime.UtcNow;
                        }

                        foreach (var inProgressService in inProgressServices)
                        {
                            context.Service.Remove(inProgressService);
                        }
                    }
                    else
                    {
                        entity.RequestManagement.RequestStatus = RequestStatusEnum.Rejected;
                        entity.Service.ServiceStatus = entity.PreviousServiceStatus;
                    }
                    entity.Service.ModifiedTime = DateTime.UtcNow;
                    //Previous status will be updated on certificate upload (reapplictaion) 
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.ApproveOrRejectReAssign, loggedInUserEmail);
                   
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    genericResponse.Success = true;
                    throw new InvalidOperationException("Details null or invalid service status");
                }
               

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex, "Error in ApproveOrCancelTransferRequest");
            }
            return genericResponse;
        }
        
        public async Task<List<CabUser>> GetActiveCabUsers(int cabId)
        {
            return await context.CabUser.Include(s=>s.Cab).Where(s => s.CabId ==cabId && s.IsActive).ToListAsync();
        }
    }
}
