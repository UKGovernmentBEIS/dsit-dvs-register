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
        public async Task<RemoveProviderToken> GetRemoveProviderToken(string token, string tokenId)
        {
            return await context.RemoveProviderToken.Include(p => p.Provider).ThenInclude(p => p.Services).Include(p=>p.RemoveTokenServiceMapping)
            .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId) ?? new RemoveProviderToken();
        }


     
        public async Task<ProviderProfile> GetProviderDetails(int providerId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p => p.Services).Include(p=>p.CabUser)
            .Where(p => p.Id == providerId).FirstOrDefaultAsync() ?? new ProviderProfile();


            // If provider has services, fetch the details for each service
            if (provider.Services != null && provider.Services.Count > 0)
            {
                foreach (var service in provider.Services)
                {

                    var baseQuery = context.Service
                    .Where(p => p.Id == service.Id)
                    .Include(p => p.ServiceRoleMapping).ThenInclude(s => s.Role)
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

        public async Task<GenericResponse> UpdateRemovalStatus(int providerProfileId, TeamEnum teamEnum, EventTypeEnum eventType, List<int>? serviceIds, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (serviceIds != null && serviceIds.Count > 0)// remove only selected services
                {
                    foreach (var item in serviceIds)
                    {
                        var service = await context.Service.Where(s => s.Id == item && s.ProviderProfileId == providerProfileId).FirstOrDefaultAsync();
                        if (service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation)
                        {
                            service.ServiceStatus = ServiceStatusEnum.Removed;
                            service.ModifiedTime = DateTime.UtcNow;
                            service.RemovedTime = DateTime.UtcNow;
                            service.RemovalTokenStatus = TokenStatusEnum.RequestCompleted;
                        }
                       
                    }
                   
                }
                else // remove all services and provider
                {
                    var existingProvider = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
                    if (existingProvider != null)
                    {
                        existingProvider.ModifiedTime = DateTime.UtcNow;                      
                        existingProvider.RemovedTime = DateTime.UtcNow;
                        // Update the status of each service
                        if (existingProvider.Services != null)
                        {
                            foreach (var service in existingProvider.Services)
                            {
                                if (service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation )
                                {
                                    service.ServiceStatus = ServiceStatusEnum.Removed;
                                    service.ModifiedTime = DateTime.UtcNow;
                                    service.RemovedTime = DateTime.UtcNow;
                                    service.RemovalTokenStatus = TokenStatusEnum.RequestCompleted;
                                }                               
                            }
                        }
                    }
                }
                await context.SaveChangesAsync(teamEnum, eventType, loggedInUserEmail);
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }




        public async Task<GenericResponse> CancelServiceRemoval(int providerProfileId, TeamEnum teamEnum, EventTypeEnum eventType, List<int>? serviceIds, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (serviceIds != null && serviceIds.Count > 0)// republish services
                {
                    foreach (var item in serviceIds)
                    {
                        var service = await context.Service.Where(s => s.Id == item && s.ProviderProfileId == providerProfileId).FirstOrDefaultAsync();
                        if (service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation)
                        {
                            service.ServiceStatus = ServiceStatusEnum.Published;
                            service.ModifiedTime = DateTime.UtcNow;
                            service.RemovalTokenStatus = TokenStatusEnum.UserCancelled;

                        }

                    }

                }               
                await context.SaveChangesAsync(teamEnum, eventType, loggedInUserEmail);
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }


        

        public async Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail)
        {
            var removeProviderToken = await context.RemoveProviderToken.Include(p => p.Provider).Include(p => p.RemoveTokenServiceMapping)
           .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);
            if (removeProviderToken != null)
            {
                context.RemoveProviderToken.Remove(removeProviderToken);
                await context.SaveChangesAsync(TeamEnum.Provider, EventTypeEnum.RemoveProviderRemovalToken, loggedInUserEmail);
                logger.LogInformation("Remove Provider Token : Token Removed for Provider {0}", removeProviderToken.Provider.TradingName);
                return true;
            }

            return false;
        }




        public async Task UpdateRemovalTokenStatus(int providerProfileId, List<int> serviceIds, TokenStatusEnum tokenStatus)
        {            
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (serviceIds != null && serviceIds.Count > 0)
                {
                    foreach (var item in serviceIds)
                    {
                        var service = await context.Service.Where(s => s.Id == item && s.ProviderProfileId == providerProfileId).FirstOrDefaultAsync();
                        service.RemovalTokenStatus = tokenStatus;
                    }
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();               
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }          
        }
        #endregion

    }
}
