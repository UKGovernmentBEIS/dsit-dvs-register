using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
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

        public async Task<List<ProviderProfile>> GetProviders(List<int> roles, List<int> schemes,string searchText = "")
        {

            IQueryable<ProviderProfile> providerQuery = context.ProviderProfile;
            providerQuery = providerQuery.Where(p => p.ProviderStatus == ProviderStatusEnum.Published &&
            (string.IsNullOrEmpty(searchText) || p.SearchVector.Matches(searchText)))
            .Include(p => p.Services).ThenInclude(ci => ci.ServiceRoleMapping)
            .Include(p => p.Services).ThenInclude(ci => ci.ServiceSupSchemeMapping);
            // Include roles and schemes filters

            providerQuery = providerQuery.Include(p => p.Services
            .Where(ci => ci.ServiceStatus == ServiceStatusEnum.Published &&
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
            ProviderProfile providerProfile = new ();
            providerProfile = await context.ProviderProfile
            .Include(p => p.Services.Where(s => s.ServiceStatus == ServiceStatusEnum.Published))
           .Include(p => p.Services).ThenInclude(x => x.ServiceRoleMapping).ThenInclude(p => p.Role)
           .Include(p => p.Services).ThenInclude(x => x.ServiceIdentityProfileMapping).ThenInclude(p=>p.IdentityProfile)
           .Include(p => p.Services).ThenInclude(x => x.ServiceSupSchemeMapping).ThenInclude(p => p.SupplementaryScheme)
            .Include(p => p.Services).ThenInclude(x => x.ServiceQualityLevelMapping).ThenInclude(p=>p.QualityLevel)    
           .Where(p => p.Id == providerId && p.Services.Any(ci => ci.ServiceStatus == ServiceStatusEnum.Published))
           .OrderBy(c => c.ModifiedTime).FirstOrDefaultAsync() ?? new ProviderProfile();
            return providerProfile;
        }
    }
}
