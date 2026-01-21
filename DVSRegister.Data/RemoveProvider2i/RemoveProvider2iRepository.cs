using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data
{
    public class RemoveProvider2iRepository :IRemoveProvider2iRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<RemoveProvider2iRepository> logger;

        public RemoveProvider2iRepository(DVSRegisterDbContext context, ILogger<RemoveProvider2iRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        #region Remove provider
        public async Task<ProviderRemovalRequest> GetRemoveProviderToken(string token, string tokenId)
        {
            return await context.ProviderRemovalRequest.Include(p => p.Provider)
            .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId)??null!;
        }
     
        public async Task<ProviderProfile> GetProviderDetails(int providerId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p => p.Services.Where(s=>s.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation))
            .Where(p => p.Id == providerId).FirstOrDefaultAsync() ?? new ProviderProfile();


            // If provider has services, fetch the details for each service
            if (provider.Services != null && provider.Services.Count > 0)
            {
                foreach (var service in provider.Services)
                {
                    var baseQuery = context.Service
                    .Where(p => p.Id == service.Id)
                    .Include(p => p.ServiceRoleMapping).ThenInclude(s => s.Role)
                    .Include(p => p.TrustFrameworkVersion)
                    .Include(p => p.UnderPinningService).ThenInclude(p => p.Provider)
                    .Include(p => p.UnderPinningService).ThenInclude(p => p.CabUser).ThenInclude(cu => cu.Cab)
                    .Include(p => p.ManualUnderPinningService).ThenInclude(p => p.Cab)
                    .AsSplitQuery();

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

                    var detailedService = await queryWithOptionalIncludes.FirstOrDefaultAsync() ?? new Service();                

                    service.ServiceRoleMapping = detailedService.ServiceRoleMapping;
                    service.ServiceQualityLevelMapping = detailedService.ServiceQualityLevelMapping;
                    service.ServiceSupSchemeMapping = detailedService.ServiceSupSchemeMapping;
                    service.ServiceIdentityProfileMapping = detailedService.ServiceIdentityProfileMapping;
                }
            }

            return provider;
        }
        public async Task<GenericResponse> ApproveProviderRemoval(int providerProfileId, int providerRemovalRequestId,  string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await context.ProviderProfile.Include(p=>p.ProviderRemovalRequests).ThenInclude(r=> r.ProviderRemovalRequestServiceMapping).Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
                var providerRemovalRequest = await context.ProviderRemovalRequest.FirstOrDefaultAsync(p => p.Id == providerRemovalRequestId && p.ProviderProfileId == providerProfileId);
                if (existingProvider != null && providerRemovalRequest!=null)
                {
                    existingProvider.IsInRegister = false;
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.ProviderStatus = ProviderStatusEnum.NA;
                    providerRemovalRequest.RemovedTime = DateTime.UtcNow;
                    providerRemovalRequest.Token = null;
                    providerRemovalRequest.TokenId = null;
                    providerRemovalRequest.IsRequestPending = false;
                    if (providerRemovalRequest.PreviousProviderStatus == ProviderStatusEnum.UpdatesRequested)
                    {
                        var pendingProvidereUpdateRequest = await context.ProviderProfileDraft.FirstOrDefaultAsync(p => p.ProviderProfileId == providerProfileId);
                        context.ProviderProfileDraft.Remove(pendingProvidereUpdateRequest);
                    }

                    // Update the status of each service
                    if (existingProvider.Services != null)
                    {
                        foreach (var service in existingProvider.Services.Where(x=>x.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation))
                        {
                            service.ServiceStatus = ServiceStatusEnum.Removed;
                            service.ModifiedTime = DateTime.UtcNow;
                            service.IsInRegister = false;

                            var mapping = providerRemovalRequest.ProviderRemovalRequestServiceMapping?.FirstOrDefault(m => m.ServiceId == service.Id);
                            if (mapping.PreviousServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation || mapping.PreviousServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation)
                            {
                                var pendingRemovalRequest = await context.ServiceRemovalRequest.FirstOrDefaultAsync(s => s.ServiceId == service.Id);
                                context.ServiceRemovalRequest.Remove(pendingRemovalRequest);
                            }
                            if (mapping.PreviousServiceStatus == ServiceStatusEnum.UpdatesRequested)
                            {
                                var pendingServiceUpdateRequest = await context.ServiceDraft.FirstOrDefaultAsync(s => s.ServiceId == service.Id);
                                context.ServiceDraft.Remove(pendingServiceUpdateRequest);
                            }
                            if (mapping.PreviousServiceStatus == ServiceStatusEnum.PublishedUnderReassign || mapping.PreviousServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
                            {
                                var pendingReassignmentRequest = await context.CabTransferRequest.Include(s => s.RequestManagement)
                                 .OrderByDescending(c => c.Id)
                                 .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.RequestManagement.RequestStatus == RequestStatusEnum.AwaitingRemoval);


                                if (pendingReassignmentRequest != null)
                                {
                                    var pendingRequest = await context.RequestManagement.Where(x => x.Id == pendingReassignmentRequest.RequestManagementId).FirstOrDefaultAsync();
                                    if (pendingRequest != null) { pendingRequest.RequestStatus = RequestStatusEnum.Removed; }

                                }
                            }
                        }
                    }
                    await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.RemoveProvider2i, loggedInUserEmail);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    genericResponse.Success = false;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }
        public async Task<GenericResponse> CancelRemoveProviderRequest(int providerProfileId, int providerRemovalRequestId, string loggedInUserEmail)
        {
            var genericResponse = new GenericResponse();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var provider = await context.ProviderProfile.Include(p => p.ProviderRemovalRequests).ThenInclude(r => r.ProviderRemovalRequestServiceMapping)
                 .Include(p => p.Services).ThenInclude(s=>s.ServiceRemovalRequest)
                 .Include(p => p.Services).ThenInclude(s => s.CabTransferRequest).ThenInclude(s => s.RequestManagement)
                 .FirstOrDefaultAsync(p => p.Id == providerProfileId);
                var currentRequest = await context.ProviderRemovalRequest.FirstOrDefaultAsync(p => p.Id == providerRemovalRequestId && p.ProviderProfileId == providerProfileId);

                if (currentRequest != null && provider!=null && provider.IsInRegister)
                {
                    provider.ModifiedTime = DateTime.UtcNow;
                    provider.ProviderStatus = ProviderStatusEnum.NA;

                    context.ProviderRemovalRequest.Remove(currentRequest);

                    foreach (var service in provider.Services.Where(s => s.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation))
                    {
                        var serviceMapping = currentRequest.ProviderRemovalRequestServiceMapping?.FirstOrDefault(m => m.ServiceId == service.Id);
                        service.ModifiedTime = DateTime.UtcNow;
                        service.ServiceStatus = serviceMapping.PreviousServiceStatus;

                        if (service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation || service.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation)
                        {                         

                            var serviceRemovalRequest = service.ServiceRemovalRequest?.Where(x =>  x?.IsRequestPending == true).FirstOrDefault();
                            if (serviceRemovalRequest == null) { throw new InvalidOperationException("Service removal request returned null"); }
                          
                            serviceRemovalRequest.IsRequestPending = true;
                        }
                        else if (service.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || service.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
                        {
                            var pendingReassignmentRequest = await context.CabTransferRequest.Include(c => c.RequestManagement).FirstOrDefaultAsync(s => s.ServiceId == service.Id);

                            if (pendingReassignmentRequest != null && serviceMapping.PreviousCabTransferStatus != null)
                            {
                                pendingReassignmentRequest.RequestManagement.RequestStatus = (RequestStatusEnum)serviceMapping.PreviousCabTransferStatus;
                            }
                        }
                    }
                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.CancelRemovalRequest, loggedInUserEmail);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    genericResponse.Success = false;
                }
               
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }

        #endregion


        #region Remove service

        public async Task<ServiceRemovalRequest> GetRemoveServiceToken(string token, string tokenId)
        {
            return await context.ServiceRemovalRequest.Include(p => p.Service)
            .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId) ?? null!;
        }
        public async Task<ServiceRemovalRequest> GetProviderDetailsWithService(string token, string tokenId)
        {
            return await context.ServiceRemovalRequest.FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId)??null!;
        }

        public async Task<Service> GetServiceDetails(int serviceId)
        {
            var baseQuery = context.Service
                   .Where(p => p.Id == serviceId)
                   .Include(p => p.ServiceRoleMapping).ThenInclude(s => s.Role)
                   .Include(p => p.TrustFrameworkVersion)
                   .Include(p=>p.Provider)
                   .Include(p => p.UnderPinningService).ThenInclude(p => p.Provider)
                   .Include(p => p.UnderPinningService).ThenInclude(p => p.CabUser).ThenInclude(cu => cu.Cab)
                   .Include(p => p.ManualUnderPinningService).ThenInclude(p => p.Cab)
                   .AsSplitQuery();

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

            var detailedService = await queryWithOptionalIncludes.FirstOrDefaultAsync() ?? new Service();

            return detailedService;
        }


        public async Task<GenericResponse> ApproveServiceRemoval(int serviceId, int serviceRemovalRequestId, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingService = await context.Service.Include(p => p.ServiceRemovalRequest).FirstOrDefaultAsync(p => p.Id == serviceId);
                var provider = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == existingService.ProviderProfileId);
                var serviceRemovalRequest = await context.ServiceRemovalRequest.FirstOrDefaultAsync(p => p.Id == serviceRemovalRequestId && p.ServiceId == serviceId);
                if (existingService != null && serviceRemovalRequest != null && existingService.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation)
                {
                    existingService.IsInRegister = false;
                    existingService.ModifiedTime = DateTime.UtcNow;
                    existingService.ServiceStatus = ServiceStatusEnum.Removed;
                    serviceRemovalRequest.RemovedTime = DateTime.UtcNow;
                    serviceRemovalRequest.Token = null;
                    serviceRemovalRequest.TokenId = null;
                    serviceRemovalRequest.IsRequestPending = false;                   

                    if( provider.Services.Where(s => s.Id != serviceId).All(s => s.IsInRegister == false))
                    {                      

                        ProviderRemovalRequest providerRemovalRequest = new ();
                        providerRemovalRequest.RemovedTime = DateTime.UtcNow;                       
                        providerRemovalRequest.IsRequestPending = false;
                        providerRemovalRequest.ProviderProfileId = provider.Id;
                        providerRemovalRequest.PreviousProviderStatus = provider.ProviderStatus;
                        await context.ProviderRemovalRequest.AddAsync(providerRemovalRequest);

                        provider.ModifiedTime = DateTime.UtcNow;
                        provider.IsInRegister = false;
                        provider.ProviderStatus = ProviderStatusEnum.NA;
                    }

                    await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.RemoveProvider2i, loggedInUserEmail);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {                   
                    await transaction.RollbackAsync();
                    genericResponse.Success = false;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }
        public async Task<GenericResponse> CancelRemoveServiceRequest(int serviceId, int serviceRemovalRequestId, string loggedInUserEmail)
        {
            var genericResponse = new GenericResponse();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var service = await context.Service.Include(s => s.ServiceRemovalRequest).FirstOrDefaultAsync(s => s.Id == serviceId);
                var serviceRemoval = await context.ServiceRemovalRequest.Where(s => s.Id == serviceRemovalRequestId && s.ServiceId == serviceId).FirstOrDefaultAsync();              

                if(serviceRemoval!=null && service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation)
                {
                    serviceRemoval.IsRequestPending = false;
                    serviceRemoval.Token = null;
                    serviceRemoval.TokenId = null;
                    service.ModifiedTime = DateTime.UtcNow;
                    service.ServiceStatus = ServiceStatusEnum.Published;
                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.CancelRemovalRequest, loggedInUserEmail);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    genericResponse.Success = false;
                }
              
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }



        #endregion
    }
}
