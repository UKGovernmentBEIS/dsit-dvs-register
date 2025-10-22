using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;
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

        public async Task<PaginatedResult<ProviderProfile>> GetProviders(
    List<int> roles, List<int> schemes, List<int> tfVersions,
    int pageNum, string searchText = "", string sortBy = "")
        {
            var trimmedSearchText = searchText?.Trim().ToUpper();

            var filteredServices =
                from s in context.Service.AsNoTracking()
                where s.IsInRegister
                    && (!roles.Any() || s.ServiceRoleMapping.Any(r => roles.Contains(r.RoleId)))
                    && (!schemes.Any() || s.ServiceSupSchemeMapping.Any(sc => schemes.Contains(sc.SupplementarySchemeId)))
                    && (!tfVersions.Any() || tfVersions.Contains(s.TrustFrameworkVersionId))
                select new
                {
                    Service = s,
                    s.Provider,
                    RoleIds = s.ServiceRoleMapping.Select(r => r.RoleId).ToList(),
                    SchemeIds = s.ServiceSupSchemeMapping.Select(sc => sc.SupplementarySchemeId).ToList(),
                    NameMatch = string.IsNullOrEmpty(trimmedSearchText)
                        || s.ServiceName.ToUpper().Contains(trimmedSearchText),
                    ProviderMatch = string.IsNullOrEmpty(trimmedSearchText)
                        || s.Provider.RegisteredName.ToUpper().Contains(trimmedSearchText)
                        || (s.Provider.TradingName != null
                            && s.Provider.TradingName.ToUpper().Contains(trimmedSearchText))
                };

            var providerQuery =
                from s in filteredServices
                where s.NameMatch || s.ProviderMatch
                group s by s.Provider into g
                select new
                {
                    Provider = g.Key,
                    Services = g.Any(x => x.ProviderMatch)
                        ? g.Select(x => new
                        {
                            x.Service,
                            x.RoleIds,
                            x.SchemeIds
                        })
                        : g.Where(x => x.NameMatch)
                            .Select(x => new
                            {
                                x.Service,
                                x.RoleIds,
                                x.SchemeIds
                            })
                };

            var sortedProviders = sortBy switch
            {
                "Time" => providerQuery.OrderByDescending(p => p.Provider.ModifiedTime),
                "Alphabet" => providerQuery.OrderBy(p => p.Provider.RegisteredName),
                "ReverseAlphabet" => providerQuery.OrderByDescending(p => p.Provider.RegisteredName),
                _ => providerQuery.OrderByDescending(p => p.Provider.PublishedTime)
                                  .ThenBy(p => p.Provider.RegisteredName)
            };

            var totalCount = await sortedProviders.CountAsync();
            var pageItems = await sortedProviders
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToListAsync();

            var items = pageItems.Select(x =>
            {
                var provider = x.Provider;
                provider.Services = [.. x.Services.Select(s => new Service
        {
            Id = s.Service.Id,
            ServiceName = s.Service.ServiceName,
            IsInRegister = s.Service.IsInRegister,
            TrustFrameworkVersionId = s.Service.TrustFrameworkVersionId,
            ServiceRoleMapping = [.. s.RoleIds.Select(rid => new ServiceRoleMapping { RoleId = rid })],
            ServiceSupSchemeMapping = [.. s.SchemeIds.Select(sid =>
                new ServiceSupSchemeMapping { SupplementarySchemeId = sid })]
        })];

                return provider;
            }).ToList();

            return new PaginatedResult<ProviderProfile>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<PaginatedResult<Service>> GetServices(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "")
        {
            var trimmedSearchText = searchText.Trim().ToUpper();

            var query = context.Service
                .Include(s => s.Provider)
                .Where(s => s.IsInRegister)
                .Where(s => string.IsNullOrEmpty(searchText) || s.ServiceName.ToUpper().Contains(trimmedSearchText) ||
                s.Provider.RegisteredName.ToUpper().Contains(trimmedSearchText))
                .Where(s => !roles.Any() || s.ServiceRoleMapping.Any(r => roles.Contains(r.RoleId)))
                .Where(s => !schemes.Any() || s.ServiceSupSchemeMapping.Any(sc => schemes.Contains(sc.SupplementarySchemeId)))
                .Where(s => !tfVersions.Any() || tfVersions.Contains(s.TrustFrameworkVersionId));

            query = sortBy switch
            {
                "Time" => query.OrderByDescending(s => s.ModifiedTime),
                "Alphabet" => query.OrderBy(s => s.ServiceName),
                "ReverseAlphabet" => query.OrderByDescending(s => s.ServiceName),
                "IssueDate" => query.OrderByDescending(s => s.ConformityIssueDate),
                "ExpiryDate" => query.OrderByDescending(s => s.ConformityExpiryDate),
                _ => query.OrderByDescending(s => s.PublishedTime).ThenBy(s => s.ServiceName)
            };

            query = query
                .Include(s => s.Provider)
                .Include(s => s.ServiceRoleMapping)
                .Include(s => s.ServiceSupSchemeMapping);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToListAsync();

            return new PaginatedResult<Service>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResult<GroupedResult<ActionLogs>>> GetUpdateLogs(int pageNumber)
        {

            var query = context.ActionLogs.Include(x => x.ActionDetails).Include(x => x.ProviderProfile).Include(x => x.Service)
                .Where(x => x.ShowInRegisterUpdates == true && x.ProviderProfile.IsInRegister == true && (x.Service == null || (x.Service != null && x.Service.IsInRegister == true)));

            int totalCount = await query.Select(x => x.LogDate) .Distinct().CountAsync();       
            int totalPages = (int)Math.Ceiling(totalCount / (double)10);
            DateTime? latestLogDate = await query.MaxAsync(x => (DateTime?)x.LogDate);

            var groupedLogs = await query
                .GroupBy(x => x.LogDate)
                .OrderByDescending(g => g.Key)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(g => new GroupedResult<ActionLogs>
                {
                    LogDate = g.Key,
                    Items = g.OrderByDescending(l => l.LoggedTime).ToList()
                }).ToListAsync();

            return new PaginatedResult<GroupedResult<ActionLogs>>
            {
                Items = groupedLogs,
                TotalCount = totalCount,
                LastUpdated = latestLogDate
            };
        }
        public async Task<DateTime> GetLastUpdatedDate()
        {
            var query = context.ActionLogs.Include(x => x.ActionDetails).Include(x => x.ProviderProfile).Include(x => x.Service)
            .Where(x => x.ShowInRegisterUpdates == true && x.ProviderProfile.IsInRegister == true && (x.Service == null || (x.Service != null && x.Service.IsInRegister == true)));
            DateTime latestUpdateDate = await query.MaxAsync(x => x.LogDate);
            return latestUpdateDate;

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
            var baseQuery = context.Service
             .Where(p => p.Id == serviceId)
             .Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
              .Include(p => p.Provider)
              .Include(p => p.TrustFrameworkVersion)
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

                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)
                    .ThenInclude(ssm => ssm.SchemeGPG44Mapping).ThenInclude(ssm => ssm.QualityLevel);
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)
                    .ThenInclude(ssm => ssm.SchemeGPG45Mapping).ThenInclude(ssm => ssm.IdentityProfile);
            }
            if (await baseQuery.AnyAsync(p => p.ServiceIdentityProfileMapping != null && p.ServiceIdentityProfileMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceIdentityProfileMapping)
                    .ThenInclude(ssm => ssm.IdentityProfile);
            }
            if (await baseQuery.AnyAsync(p => p.ServiceType == ServiceTypeEnum.WhiteLabelled))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.UnderPinningService).ThenInclude(p => p.Provider)
             .Include(p => p.UnderPinningService).ThenInclude(p => p.CabUser).ThenInclude(cu => cu.Cab)
             .Include(p => p.ManualUnderPinningService).ThenInclude(ms => ms.Cab);
            }

            var service = await queryWithOptionalIncludes.SingleOrDefaultAsync() ?? new Service();
            return service;
        }

        public async Task<List<Service>> GetPublishedServices()
        {
            return await context.Service.AsNoTracking()//Read only, so no need for tracking query
             .Include(service => service.Provider).AsNoTracking()
             .Include(service => service.CabUser).ThenInclude(cabUser => cabUser.Cab).AsNoTracking()
             .Include(service => service.ServiceSupSchemeMapping).ThenInclude(ssm => ssm.SupplementaryScheme).AsNoTracking()
             .Where(ci => ci.IsInRegister == true).OrderBy(ci => ci.Provider.RegisteredName)
             .ToListAsync();
        }
    } 
}
