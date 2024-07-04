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
    }
}
