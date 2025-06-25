using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.TrustFramework
{
    public class TrustFrameworkRepository : ITrustFrameworkRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<TrustFrameworkRepository> logger;

        public TrustFrameworkRepository(DVSRegisterDbContext context, ILogger<TrustFrameworkRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<TrustFrameworkVersion>> GetTrustFrameworkVersions()
        {
            return await context.TrustFrameworkVersion
                .OrderBy(tf => tf.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cab>> GetCabs()
        {
            return await context.Cab
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Service>> GetServices(bool isPublished, string searchText)
        {
            var baseQuery = context.Service
                .Include(s => s.Provider)
                .Include(s => s.CertificateReview)
                .Include(s => s.TrustFrameworkVersion)
                .Include(s => s.CabUser).ThenInclude(s => s.Cab);

            var groupedQuery = await baseQuery
                .GroupBy(s => s.ServiceKey)
                .Select(g => g.OrderByDescending(s => s.ServiceVersion).FirstOrDefault())
                .ToListAsync();

            IEnumerable<Service> filteredQuery;

            filteredQuery = groupedQuery
                .Where(s => s.TrustFrameworkVersion.Version == Constants.TFVersion0_4);

            if (isPublished)
            {
                filteredQuery = filteredQuery
                    .Where(s => s.ServiceStatus == ServiceStatusEnum.Published);
            }
            else
            {
                filteredQuery = filteredQuery
                    .Where(s => s.CertificateReview != null &&
                                s.CertificateReview.CertificateReviewStatus == CertificateReviewEnum.Approved &&
                                s.ServiceStatus != ServiceStatusEnum.Published);
            }

            string trimmedSearchText = searchText.Trim().ToLower();
            filteredQuery = filteredQuery
                .Where(s => s.ServiceName.ToLower().Contains(trimmedSearchText) ||
                            s.Provider.RegisteredName.ToLower().Contains(trimmedSearchText));

            return filteredQuery.ToList();

        }

        public async Task<Service> GetServiceDetails(int serviceId)
        {
            return await context.Service
                .Include(s => s.Provider)
                .Include(s => s.CertificateReview)
                .Include(s => s.PublicInterestCheck)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SupplementaryScheme)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG44Mapping).ThenInclude(s => s.QualityLevel)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG45Mapping).ThenInclude(s => s.IdentityProfile)
                .Include(s => s.ServiceRoleMapping).ThenInclude(s => s.Role)
                .Include(s => s.ServiceQualityLevelMapping).ThenInclude(s => s.QualityLevel)
                .Include(s => s.ServiceIdentityProfileMapping).ThenInclude(s => s.IdentityProfile)
                .Include(s => s.CabUser).ThenInclude(s => s.Cab)
                .Include(s => s.TrustFrameworkVersion)
                .FirstOrDefaultAsync(s => s.Id == serviceId);
        }

    }
}
