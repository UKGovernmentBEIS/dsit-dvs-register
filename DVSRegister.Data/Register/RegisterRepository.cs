using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DVSRegister.CommonUtility.Models.Enums;
using System.Data;

namespace DVSRegister.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<RegisterRepository> logger;

        public RegisterRepository(DVSRegisterDbContext context, ILogger<RegisterRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }

        public async Task<List<Provider>> GetProviders(List<int> roles, List<int> schemes,string searchText = "")
        {
           IQueryable<Provider> providerQuery = context.Provider;
            
            providerQuery = providerQuery.Where(p => p.ProviderStatus == ProviderStatusEnum.Published &&
            (string.IsNullOrEmpty(searchText) || p.SearchVector.Matches(searchText)))
            .Include(p => p.CertificateInformation).ThenInclude(ci => ci.CertificateInfoRoleMappings)
            .Include(p => p.CertificateInformation).ThenInclude(ci => ci.CertificateInfoSupSchemeMappings);
            // Include roles and schemes filters

            providerQuery = providerQuery.Include(p => p.CertificateInformation
            .Where(ci => ci.CertificateInfoStatus == CertificateInfoStatusEnum.Published &&
              (!roles.Any() || (ci.CertificateInfoRoleMappings != null && ci.CertificateInfoRoleMappings.Any(r => roles.Contains(r.RoleId)))) &&
             (!schemes.Any() || (ci.CertificateInfoSupSchemeMappings != null && ci.CertificateInfoSupSchemeMappings.Any(s => schemes.Contains(s.SupplementarySchemeId))))
             ));
            return await providerQuery.ToListAsync();
        }

        public async Task<List<RegisterPublishLog>> GetRegisterPublishLogs()
        {
            return await context.RegisterPublishLog.OrderByDescending(p => p.CreatedTime).ToListAsync();
        }

        public async Task<Provider> GetProviderDetails(int providerId)
        {
            Provider provider = new Provider();
            provider = await context.Provider.Include(p => p.PreRegistration)
           .Include(p => p.CertificateInformation).ThenInclude(x => x.CertificateInfoRoleMappings)
           .Include(p => p.CertificateInformation).ThenInclude(x => x.CertificateInfoIdentityProfileMappings)
           .Include(p => p.CertificateInformation).ThenInclude(x => x.CertificateInfoSupSchemeMappings)
           .Where(p => p.Id == providerId  && p.CertificateInformation.Any(ci => ci.CertificateInfoStatus == CertificateInfoStatusEnum.Published))
           . OrderBy(c => c.ModifiedTime).FirstOrDefaultAsync() ?? new Provider();
            return provider;
        }
    }
}
