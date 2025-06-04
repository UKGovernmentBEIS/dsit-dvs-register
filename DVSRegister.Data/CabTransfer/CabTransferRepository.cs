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

        public async Task<List<CabTransferRequest>> GetServiceTransferRequests(int cabId)
        {
            return await context.CabTransferRequest.Include(r=>r.RequestManagement).Include(r=>r.Service).ThenInclude(r=>r.Provider).Include(r => r.ToCab)
            .Include(r=>r.FromCabUser)
           .Where(r=>r.ToCabId == cabId).OrderBy(r=>r.DecisionTime).ToListAsync();
        }

        public async Task<Service> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId)
        {

            var baseQuery = context.Service.Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId)
             .Include(p => p.CabTransferRequest)
              .Include(p => p.Provider)
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

        public async Task<CabTransferRequest> GetCabTransferRequestDeatils(int requestId)
        {
            return await context.CabTransferRequest.Include(r => r.Service).ThenInclude(r => r.Provider)            
            .Include(r => r.FromCabUser).Where(r => r.Id == requestId).FirstOrDefaultAsync() ?? new CabTransferRequest();
        }

        public async Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                
                var entity = await context.CabTransferRequest.Include(c=>c.RequestManagement).Include(c=>c.Service).Where(x=>x.Id == requestId).FirstOrDefaultAsync();
                var cabUser = await context.CabUser.Where(x => x.CabEmail == loggedInUserEmail && x.IsActive).FirstOrDefaultAsync();
                if (entity != null && entity.RequestManagement != null && entity.Service != null && 
                (entity.Service.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || entity.Service.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign))
                {
                    if(approve)
                    {
                        entity.RequestManagement.RequestStatus = RequestStatusEnum.Approved;
                        entity.Service.CabUserId =  cabUser.Id;
                    }
                    else
                    {
                        entity.RequestManagement.RequestStatus = RequestStatusEnum.Rejected;
                    }
                    entity.Service.ServiceStatus = entity.PreviousServiceStatus;  
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
    }
}
