using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CabRemovalRequest
{
    public class CabRemovalRequestRepository :ICabRemovalRequestRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CabRemovalRequestRepository> logger;

        public CabRemovalRequestRepository(DVSRegisterDbContext context, ILogger<CabRemovalRequestRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<GenericResponse> UpdateRemovalStatus(int cabId,int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab)
        {
            GenericResponse genericResponse = new();    
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var service = await context.Service.Where(s => s.Id == serviceId && s.ProviderProfileId == providerProfileId && s.CabUser.CabId == cabId).FirstOrDefaultAsync();
                service.ServiceStatus = ServiceStatusEnum.CabAwaitingRemovalConfirmation;
                service.ModifiedTime = DateTime.UtcNow;
                service.RemovalRequestTime = DateTime.UtcNow;
                service.RemovalReasonByCab = removalReasonByCab;
                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveServiceRequestedByCab, loggedInUserEmail);    
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError($"Error in UpdateRemovalStatus method: '{ex.Message}'");
            }
            return genericResponse;
        }
    }
}
