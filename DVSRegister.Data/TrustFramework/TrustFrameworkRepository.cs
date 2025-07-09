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

        public async Task<List<Service>> GetPublishedUnderpinningServices(string searchText)
        {
            var baseQuery = context.Service
                .Include(s => s.Provider)
                .Include(s => s.CertificateReview)
                .Include(s => s.TrustFrameworkVersion)
                .Include(s => s.CabUser).ThenInclude(s => s.Cab);

            var filteredQuery = baseQuery
                .Where(s => s.TrustFrameworkVersion.Version == Constants.TFVersion0_4 && s.ServiceType == ServiceTypeEnum.UnderPinning 
                 && s.ServiceStatus == ServiceStatusEnum.Published);           

            string trimmedSearchText = searchText.Trim().ToLower();
            filteredQuery = filteredQuery
                .Where(s => s.ServiceName.ToLower().Contains(trimmedSearchText) ||
                            s.Provider.RegisteredName.ToLower().Contains(trimmedSearchText));

            return await filteredQuery.AsNoTracking().ToListAsync();
        }


        public async Task<List<Service>> GetServicesWithManualUnderinningService(string searchText)
        {
            var trimmedSearchText = searchText.Trim().ToLower();
            //select manually entered under pinning services for a white labelled type
            var manualUnderPinningServices = await context.Service.Include(s => s.ManualUnderPinningService)
            .ThenInclude(s => s.Cab).Include(s => s.CertificateReview)
            .Where(x => x.ServiceType == ServiceTypeEnum.WhiteLabelled
                            && x.ManualUnderPinningServiceId != null
                            && x.ManualUnderPinningServiceId > 0
                            && x.CertificateReview.CertificateReviewStatus == CertificateReviewEnum.Approved
                            && (string.IsNullOrEmpty(trimmedSearchText) ||
                                x.ManualUnderPinningService.ServiceName.ToLower().Contains(trimmedSearchText) ||
                                x.Provider.RegisteredName.ToLower().Contains(trimmedSearchText))). AsNoTracking().ToListAsync();
          return manualUnderPinningServices;
        }

        public async Task<Service> GetServiceDetails(int serviceId)
        {
            return await context.Service
                .Include(s => s.Provider)
                .Include(s => s.CertificateReview)
                .Include(s => s.PublicInterestCheck)
                 .Include(s => s.ManualUnderPinningService).ThenInclude(s=>s.Cab)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SupplementaryScheme)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG44Mapping).ThenInclude(s => s.QualityLevel)
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG45Mapping).ThenInclude(s => s.IdentityProfile)
                .Include(s => s.ServiceRoleMapping).ThenInclude(s => s.Role)
                .Include(s => s.ServiceQualityLevelMapping).ThenInclude(s => s.QualityLevel)
                .Include(s => s.ServiceIdentityProfileMapping).ThenInclude(s => s.IdentityProfile)
                .Include(s => s.CabUser).ThenInclude(s => s.Cab)
                .Include(s => s.TrustFrameworkVersion).AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == serviceId);
        }

    }
}
