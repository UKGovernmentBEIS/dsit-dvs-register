using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Edit
{
    public class EditRepository :IEditRepository
    {
        private readonly DVSRegisterDbContext _context;
        private readonly ILogger<EditRepository> _logger;

        public EditRepository(DVSRegisterDbContext context, ILogger<EditRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<GenericResponse> SaveProviderDraft(ProviderProfileDraft draft, string loggedInUserEmail)
        {
            var response = new GenericResponse();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == loggedInUserEmail);
                draft.RequestedUserId = user.Id;
                var existingDraft = await _context.ProviderProfileDraft
                    .FirstOrDefaultAsync(x => x.ProviderProfileId == draft.ProviderProfileId && x.Id == draft.Id);

                var provider = await _context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(x => x.Id == draft.ProviderProfileId);

                if (existingDraft != null)
                {
                    throw new InvalidOperationException("Edit request already exists for the provider");
                }
                else
                {
                    draft.ModifiedTime = DateTime.UtcNow;
                    await _context.ProviderProfileDraft.AddAsync(draft);
                    var servicesList = provider?.Services?.Where(x => x.ServiceStatus == ServiceStatusEnum.Published);

                    provider.ProviderStatus = ProviderStatusEnum.UpdatesRequested;
                    provider.ModifiedTime = DateTime.UtcNow;
                    if (servicesList != null && servicesList.Count() > 0)
                    {
                        foreach (var service in servicesList)
                        {
                            service.ServiceStatus = ServiceStatusEnum.UpdatesRequested;
                            service.ModifiedTime = DateTime.UtcNow;
                        }
                    }
                    await _context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.CabEditProvider, loggedInUserEmail);
                    response.InstanceId = draft.Id;
                }

                transaction.Commit();
                response.Success = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.Success = false;
                _logger.LogError(ex, "Error in SaveProviderDraft");
            }
            return response;
        }

        public async Task<ProviderProfile> GetProviderDetails(int providerId)
        {
            var providerProfile = await _context.ProviderProfile
              .Include(p => p.Services)
              .Include(p => p.ProviderProfileDraft)             
              .FirstOrDefaultAsync(p => p.Id == providerId);
            return providerProfile;
        }

    }
}
