using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

            IQueryable<ProviderProfile> providerQuery = context.ProviderProfile;
            providerQuery = providerQuery.Where(p => p.IsInRegister == true &&
            (string.IsNullOrEmpty(searchText)
            || EF.Functions.TrigramsSimilarity(p.RegisteredName.ToLower(), searchText.ToLower()) > .2
             || EF.Functions.TrigramsSimilarity(p.TradingName!.ToLower(), searchText.ToLower()) > .2)
             || p.Services.Any(ci => ci.IsInRegister == true && (EF.Functions.TrigramsSimilarity(ci.ServiceName.ToLower(), searchText.ToLower()) > .2)))
            .Include(p => p.Services).ThenInclude(ci => ci.ServiceRoleMapping)
            .Include(p => p.Services).ThenInclude(ci => ci.ServiceSupSchemeMapping)
            .OrderByDescending(p => p.PublishedTime)
            .ThenBy(p => p.RegisteredName)
            .AsSplitQuery();
            // Include roles and schemes filters

            providerQuery = providerQuery.Include(p => p.Services
            .Where(ci => ci.IsInRegister == true &&
               (string.IsNullOrEmpty(searchText) || EF.Functions.TrigramsSimilarity(ci.ServiceName.ToLower(), searchText.ToLower()) > .2 ||
               EF.Functions.TrigramsSimilarity(ci.Provider.RegisteredName.ToLower(), searchText.ToLower()) > .2 || EF.Functions.TrigramsSimilarity(ci.Provider.TradingName.ToLower(), searchText.ToLower()) > .2) &&
              (!roles.Any() || (ci.ServiceRoleMapping != null && ci.ServiceRoleMapping.Any(r => roles.Contains(r.RoleId)))) &&
             (!schemes.Any() || (ci.ServiceSupSchemeMapping != null && ci.ServiceSupSchemeMapping.Any(s => schemes.Contains(s.SupplementarySchemeId))))
             ));
            return await providerQuery.ToListAsync();

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
           .OrderBy(c => c.ModifiedTime).FirstOrDefaultAsync(p => p.Id == providerId) ?? new ProviderProfile();
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
                .FirstOrDefaultAsync(s => s.Id == serviceId) ?? new Service();
            return service;
        }
    }
}
