using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public class RemoveProviderRepository(DVSRegisterDbContext context, ILogger<RemoveProviderRepository> logger) : IRemoveProviderRepository
    {
        private readonly DVSRegisterDbContext context = context;
        private readonly ILogger<RemoveProviderRepository> logger = logger;

        public async Task<GenericResponse> UpdateProviderStatus(int providerProfileId, ProviderStatusEnum providerStatus, string loggedInUserEmail, EventTypeEnum eventType, TeamEnum team)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfileId);
                if (existingProvider != null)
                {

                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.ProviderStatus = providerStatus;

                    if (providerStatus == ProviderStatusEnum.RemovedFromRegister)
                    {
                        existingProvider.RemovedTime = DateTime.UtcNow;
                        existingProvider.IsInRegister = false;
                    }

                }
                await context.SaveChangesAsync(team, eventType, loggedInUserEmail);
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

        public async Task<ProviderProfile> GetProviderWithAllServices(int providerId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p => p.Services.Where(x => x.IsCurrent == true))
            .Where(p => p.Id == providerId).FirstOrDefaultAsync() ?? new ProviderProfile();
            return provider;
        }
    }
}
