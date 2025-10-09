using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly DVSRegisterDbContext context;

        public RegisterRepository(DVSRegisterDbContext context)
        {
            this.context = context;
        }

        public async Task<List<ProviderProfile>> GetProviders(List<int> roles, List<int> schemes, string searchText = "")
        {
            IQueryable<ProviderProfile> providerQuery = context.ProviderProfile
                .Where(p => p.IsInRegister == true);

            // Apply search filters if searchText is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                var lowerSearchText = searchText.ToLower();
                providerQuery = providerQuery.Where(p =>
                    // Search in provider names with partial matching
                    p.RegisteredName.ToLower().Contains(lowerSearchText) ||
                    p.TradingName!.ToLower().Contains(lowerSearchText) ||
                    // Search in service names
                    p.Services.Any(s => s.IsInRegister == true && s.ServiceName.ToLower().Contains(lowerSearchText))
                );
            }

            // Apply role and scheme filters
            if (roles.Any() || schemes.Any())
            {
                providerQuery = providerQuery.Where(p =>
                    p.Services.Any(s => s.IsInRegister == true &&
                        (!roles.Any() || s.ServiceRoleMapping.Any(r => roles.Contains(r.RoleId))) &&
                        (!schemes.Any() || s.ServiceSupSchemeMapping.Any(sm => schemes.Contains(sm.SupplementarySchemeId)))
                    )
                );
            }

            // Return related data and execute query
            return await providerQuery
                .Include(p => p.Services.Where(s => s.IsInRegister == true))
                    .ThenInclude(s => s.ServiceRoleMapping)
                .Include(p => p.Services.Where(s => s.IsInRegister == true))
                    .ThenInclude(s => s.ServiceSupSchemeMapping)
                .OrderByDescending(p => p.PublishedTime)
                .ThenBy(p => p.RegisteredName)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<RegisterPublishLog>> GetRegisterPublishLogs()
        {
            return await context.RegisterPublishLog.OrderByDescending(p => p.CreatedTime).ToListAsync();
        }

        public async Task<ProviderProfile> GetProviderDetails(int providerId)
        {
            ProviderProfile providerProfile = new();
            providerProfile = await context.ProviderProfile
          .Include(p => p.Services.Where(ci => ci.IsInRegister == true))
          .Include(p => p.Services).ThenInclude(s => s.UnderPinningService).ThenInclude(s => s.Provider)
          .Include(p => p.Services).ThenInclude(s => s.UnderPinningService).ThenInclude(s => s.CabUser).ThenInclude(s => s.Cab)
          .Include(p => p.Services).ThenInclude(s => s.ManualUnderPinningService).ThenInclude(s => s.Cab)
          .Include(p => p.Services).ThenInclude(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SupplementaryScheme)
          .Include(p => p.Services).ThenInclude(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG44Mapping).ThenInclude(s => s.QualityLevel)
          .Include(p => p.Services).ThenInclude(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG45Mapping).ThenInclude(s => s.IdentityProfile)
           .Include(p => p.Services).ThenInclude(x => x.ServiceRoleMapping).ThenInclude(p => p.Role)
           .Include(p => p.Services).ThenInclude(x => x.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
            .Include(p => p.Services).ThenInclude(x => x.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
            .Include(p => p.Services).ThenInclude(p => p.TrustFrameworkVersion)
           .OrderBy(c => c.ModifiedTime).FirstOrDefaultAsync(p => p.Id == providerId && p.IsInRegister == true);
            return providerProfile;
        }

        public async Task<Service> GetServiceDetails(int serviceId)
        {
            Service service = new();
            service = await context.Service
                .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SupplementaryScheme)
                .Include(x => x.ServiceRoleMapping).ThenInclude(p => p.Role)
                .Include(x => x.ServiceIdentityProfileMapping).ThenInclude(p => p.IdentityProfile)
                .Include(x => x.ServiceQualityLevelMapping).ThenInclude(p => p.QualityLevel)
                .Include(p => p.TrustFrameworkVersion)
                .Include(x => x.Provider).AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.ServiceType == ServiceTypeEnum.UnderPinning && s.IsInRegister == true );
            return service;
        }
    }
}
