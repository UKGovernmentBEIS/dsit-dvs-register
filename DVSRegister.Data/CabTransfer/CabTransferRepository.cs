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
            return await context.CabTransferRequest.Include(r=>r.RequestManagement).Include(r=>r.Service).Include(r=>r.ProviderProfile).Include(r => r.ToCab)
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
    }
}
