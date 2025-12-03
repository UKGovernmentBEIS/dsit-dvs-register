using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;
using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data
{
    public class HomeRepository: IHomeRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CabRepository> logger;

        public HomeRepository(DVSRegisterDbContext context, ILogger<CabRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<PaginatedResult<Service>> GetDraftApplications(int cabId, int pageNumber, string sort, string sortAction)
        {
            try
            {
                var baseQuery = context.Service
                .Include(s => s.CabUser)
                .Where(s => s.ServiceStatus == ServiceStatusEnum.SavedAsDraft && s.CabUser.CabId == cabId)
                .Include(s => s.Provider);

                return await SortAndPaginate(pageNumber, sort, sortAction, baseQuery);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<PaginatedResult<Service>> GetSentBackApplications(int cabId, int pageNumber, string sort, string sortAction)
        {
            try
            {
                var baseQuery = context.Service
                .Include(s => s.CabUser)
                .Where(s => s.ServiceStatus == ServiceStatusEnum.AmendmentsRequired && s.CabUser.CabId == cabId)
                .Include(s => s.Provider);

                return await SortAndPaginate(pageNumber, sort, sortAction, baseQuery);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<PaginatedResult<Service>> GetPendingReassignmentRequests(int cabId, int pageNumber, string sort, string sortAction)
        {
            try
            {
                var baseQuery = context.Service
                    .Include(s => s.CabTransferRequest)!.ThenInclude(r => r.RequestManagement)
                    .Where(s => (s.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || s.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
                    && s.CabTransferRequest!.Where(c =>c.Id == s.CabTransferRequest!.Max(x=>x.Id)).FirstOrDefault()!.ToCabId == cabId 
                    && s.CabTransferRequest!.Where(c => c.Id == s.CabTransferRequest!.Max(x => x.Id))!.FirstOrDefault()!.RequestManagement.RequestStatus == RequestStatusEnum.Pending)
                    .Include(s => s.Provider)
                    .Include(s => s.CabUser).ThenInclude(c => c.Cab);
                return await SortAndPaginate(pageNumber, sort, sortAction, baseQuery);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Dictionary<string, int>> GetPendingCounts(int cabId)
        {           
            int draft = context.Service
                .Where(s => s.ServiceStatus == ServiceStatusEnum.SavedAsDraft && s.CabUser.CabId == cabId)
                .Count();

            int sendBack = context.Service
                .Where(s => s.ServiceStatus == ServiceStatusEnum.AmendmentsRequired && s.CabUser.CabId == cabId)
                .Count();

            int reassignment = context.Service
                .Where(s => (s.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || s.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign) 
                   && s.CabTransferRequest!.Where(c => c.Id == s.CabTransferRequest!.Max(x => x.Id)).FirstOrDefault()!.ToCabId == cabId
                    && s.CabTransferRequest!.Where(c => c.Id == s.CabTransferRequest!.Max(x => x.Id))!.FirstOrDefault()!.RequestManagement.RequestStatus == RequestStatusEnum.Pending)
                .Count();


            return new Dictionary<string, int>
            {
                ["DraftApplications"] = draft,
                ["SentBackApplications"] = sendBack,
                ["PendingReassignmentRequests"] = reassignment
            };
        }


        public async Task<PaginatedResult<ProviderProfile>> GetAllProviders(int cabId, int pageNumber, string sort, string sortAction, string searchText)
        {
            IQueryable<ProviderProfile> baseQuery = context.ProviderProfile
                .Include(p => p.ProviderProfileCabMapping)
                .Where(p => (string.IsNullOrEmpty(searchText) || p.RegisteredName.ToLower().Contains(searchText.Trim().ToLower()))
                && p.ProviderProfileCabMapping.Any(p => p.CabId == cabId))
                .Include(p => p.Services);


            Func<IQueryable<ProviderProfile>, IOrderedQueryable<ProviderProfile>> orderByFunc = sort switch
            {
                "provider" => s => sortAction == "descending" ? baseQuery.OrderByDescending(r => r.RegisteredName) : baseQuery.OrderBy(r => r.RegisteredName),
                "date" => s => sortAction == "descending" ? baseQuery.OrderByDescending(r => r.ModifiedTime ?? r.CreatedTime) :
                        baseQuery.OrderBy(r => r.ModifiedTime ?? r.CreatedTime)
            };

            var orderedQuery = orderByFunc(baseQuery);
            var totalCount = await baseQuery.CountAsync();
            var items = await orderedQuery
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();

            return new PaginatedResult<ProviderProfile>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

       

        #region Private methods

        private static Task<PaginatedResult<Service>> SortAndPaginate(int pageNumber, string sort, string sortAction, IEnumerable<Service> baseQuery)
        {
            Func<IEnumerable<Service>, IOrderedEnumerable<Service>> orderByFunc = sort switch
            {
                "service name" => s => sortAction == "descending" ? s.OrderByDescending(r => r.ServiceName) : s.OrderBy(r => r.ServiceName),
                "provider" => s => sortAction == "descending" ? s.OrderByDescending(r => r.Provider.RegisteredName) : s.OrderBy(r => r.Provider.RegisteredName),
                "date" => s => sortAction == "descending" ? s.OrderByDescending(r => (r.ModifiedTime ?? r.CreatedTime)) : s.OrderBy(r => (r.ModifiedTime ?? r.CreatedTime)),
                "dateAfterPublish" => s => sortAction == "descending" ? s.OrderByDescending(r => (r.ModifiedTime ?? r.PublishedTime)) : s.OrderBy(r => (r.ModifiedTime ?? r.PublishedTime)),                
                "application" => s => sortAction == "descending" ? s.OrderByDescending(r => r.ServiceVersion) : s.OrderBy(r => r.ServiceVersion),
                "cab" => s => sortAction == "descending" ? s.OrderByDescending(r => r.CabUser.Cab.CabName) : s.OrderBy(r => r.CabUser.Cab.CabName),
                _ => s => s.OrderBy(r => r.ModifiedTime)
            };

            var orderedQuery = orderByFunc(baseQuery).ThenBy(s => s.ModifiedTime);
            var totalCount = orderedQuery.Count();
            var items = orderedQuery
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToList();

            var result = new PaginatedResult<Service>
            {
                Items = items,
                TotalCount = totalCount
            };

            return Task.FromResult(result);
        }
        #endregion


    }
}
