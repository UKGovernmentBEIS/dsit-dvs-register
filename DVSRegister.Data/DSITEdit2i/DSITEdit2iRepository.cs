using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Repositories
{
    public class DSITEdit2iRepository : IDSITEdit2iRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<DSITEdit2iRepository> logger;

        public DSITEdit2iRepository(DVSRegisterDbContext context, ILogger<DSITEdit2iRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }


        #region Provider update
        public async Task<ProviderDraftToken> GetProviderDraftToken(string token, string tokenId)
        {
            return await context.ProviderDraftToken.Include(p => p.ProviderProfileDraft).ThenInclude(p => p.Provider)
            .Include(p => p.ProviderProfileDraft).ThenInclude(p => p.User)
           .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId) ?? new ProviderDraftToken();
        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await context.ProviderProfile.Include(p=>p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
                var existingDraftProvider = await context.ProviderProfileDraft.FirstOrDefaultAsync(p => p.Id == providerDraftId);

                var serviceDrafts = await context.ServiceDraft.Where(p => p.ProviderProfileId == providerProfileId).
               Include(P => P.ServiceRoleMappingDraft).Include(P => P.ServiceQualityLevelMappingDraft).Include(p => p.ServiceIdentityProfileMappingDraft)
               .Include(p => p.ServiceSupSchemeMappingDraft).ToListAsync();

                if (existingProvider != null && existingProvider.ProviderStatus == ProviderStatusEnum.UpdatesRequested && existingDraftProvider!=null 
                    && existingDraftProvider.ProviderProfileId == existingProvider.Id)
                {
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.ProviderStatus = ProviderStatusEnum.Published;                    
                    existingProvider.EditProviderTokenStatus = TokenStatusEnum.RequestCompleted;

                    existingProvider.RegisteredName = existingDraftProvider.RegisteredName ?? existingProvider.RegisteredName;
                    existingProvider.TradingName = existingDraftProvider.TradingName != null
                    ? (existingDraftProvider.TradingName == "-" ? string.Empty : existingDraftProvider.TradingName)
                    : existingProvider.TradingName;

                    existingProvider.PrimaryContactFullName = existingDraftProvider.PrimaryContactFullName ?? existingProvider.PrimaryContactFullName;
                    existingProvider.PrimaryContactJobTitle = existingDraftProvider.PrimaryContactJobTitle ?? existingProvider.PrimaryContactJobTitle;
                    existingProvider.PrimaryContactEmail = existingDraftProvider.PrimaryContactEmail ?? existingProvider.PrimaryContactEmail;
                    existingProvider.PrimaryContactTelephoneNumber = existingDraftProvider.PrimaryContactTelephoneNumber ?? existingProvider.PrimaryContactTelephoneNumber;


                    existingProvider.SecondaryContactFullName = existingDraftProvider.SecondaryContactFullName ?? existingProvider.SecondaryContactFullName;
                    existingProvider.SecondaryContactJobTitle = existingDraftProvider.SecondaryContactJobTitle ?? existingProvider.SecondaryContactJobTitle;
                    existingProvider.SecondaryContactEmail = existingDraftProvider.SecondaryContactEmail ?? existingProvider.SecondaryContactEmail;
                    existingProvider.SecondaryContactTelephoneNumber = existingDraftProvider.SecondaryContactTelephoneNumber ?? existingProvider.SecondaryContactTelephoneNumber;

                    existingProvider.ProviderWebsiteAddress = existingDraftProvider.ProviderWebsiteAddress ?? existingProvider.ProviderWebsiteAddress;
                    

                    existingProvider.PublicContactEmail = existingDraftProvider.PublicContactEmail != null
                  ? (existingDraftProvider.PublicContactEmail == "-" ? string.Empty : existingDraftProvider.PublicContactEmail)
                  : existingProvider.PublicContactEmail;

                    existingProvider.ProviderTelephoneNumber = existingDraftProvider.ProviderTelephoneNumber != null
                    ? (existingDraftProvider.ProviderTelephoneNumber == "-" ? string.Empty : existingDraftProvider.ProviderTelephoneNumber)
                    : existingProvider.ProviderTelephoneNumber;

                    // Update the status of each service
                    foreach (var serviceDraft in serviceDrafts)
                    {
                        var service = await context.Service.Where(s => s.Id == serviceDraft.ServiceId).FirstOrDefaultAsync();
                        service.ServiceStatus = ServiceStatusEnum.Published;                  
                        service.ModifiedTime = DateTime.UtcNow;
                        context.Remove(serviceDraft);
                    }
                    context.Remove(existingDraftProvider);
                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
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



        public async Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = new();
            var existingProvider = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
            var existingDraftProvider = await context.ProviderProfileDraft.FirstOrDefaultAsync(p => p.Id == providerDraftId);
            var serviceDrafts = await context.ServiceDraft.Where(p => p.ProviderProfileId == providerProfileId). 
            Include(P=>P.ServiceRoleMappingDraft).Include(P=>P.ServiceQualityLevelMappingDraft).Include(p=>p.ServiceIdentityProfileMappingDraft)
            .Include(p=>p.ServiceSupSchemeMappingDraft). ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (existingProvider != null && existingDraftProvider!=null )// assign back the previous status
                {

                    existingProvider.ProviderStatus = existingDraftProvider.PreviousProviderStatus;
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.EditProviderTokenStatus = TokenStatusEnum.UserCancelled;
                    foreach (var serviceDraft in serviceDrafts)
                    {
                        var service = await context.Service.Where(s => s.Id == serviceDraft.ServiceId). FirstOrDefaultAsync();
                        service.ServiceStatus = serviceDraft.PreviousServiceStatus;
                        service.ModifiedTime = DateTime.UtcNow;
                        context.Remove(serviceDraft);
                       
                    }
                    context.Remove(existingDraftProvider);
                }
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
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



        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            var providerDraftToken = await context.ProviderDraftToken.FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);
            if (providerDraftToken != null)
            {
                context.ProviderDraftToken.Remove(providerDraftToken);
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
                logger.LogInformation("Remove Provider Draft Token : Token Removed for Provider Id {0}", providerDraftToken.ProviderProfileDraft.ProviderProfileId);
                return true;
            }
            return false;
        }

        public async Task<ProviderProfile> GetProvider(int providerProfileId)
        {
            return await context.ProviderProfile.AsNoTracking().FirstOrDefaultAsync(e => e.Id == providerProfileId) ?? new ProviderProfile();
        }

        #endregion



    }
}
