using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
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
        public async Task<GenericResponse> AddServiceRemovalRequest(int cabId, int serviceId, string loggedInUserEmail, string removalReasonByCab)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var service = await context.Service.Where(s => s.Id == serviceId && s.CabUser.CabId == cabId).FirstOrDefaultAsync();
                if (service == null) { 
                    genericResponse.Success = false; 
                    return genericResponse; 
                }
                ServiceRemovalRequest serviceRemovalRequest = new();
                serviceRemovalRequest.ServiceId = serviceId;
                serviceRemovalRequest.RemovalReasonByCab = removalReasonByCab;
                serviceRemovalRequest.RemovalRequestTime = DateTime.UtcNow;
                serviceRemovalRequest.PreviousServiceStatus = service.ServiceStatus;
                serviceRemovalRequest.RemovalRequestedCabUserId = context.CabUser
                .Where(u => u.CabEmail == loggedInUserEmail).Select(u => u.Id).FirstOrDefault();
                serviceRemovalRequest.IsRequestPending = true;
                await context.ServiceRemovalRequest.AddAsync(serviceRemovalRequest); 
                service.ServiceStatus = ServiceStatusEnum.CabAwaitingRemovalConfirmation;
                service.ModifiedTime = DateTime.UtcNow;
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

        public async Task<GenericResponse> CancelServiceRemovalRequest(int serviceId, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var service = await context.Service.Include(s => s.ServiceRemovalRequest).FirstOrDefaultAsync(s => s.Id == serviceId && s.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation);

                if (service == null ||service.ServiceRemovalRequest == null ) { 
                    genericResponse.Success = false;
                    genericResponse.ErrorType = ErrorTypeEnum.RequestAlreadyProcessed;
                    return genericResponse; 
                }

                service.ServiceStatus = service.ServiceRemovalRequest.PreviousServiceStatus;
                context.ServiceRemovalRequest.Remove(service.ServiceRemovalRequest);

                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.CancelRemovalRequest, loggedInUserEmail);
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError($"Error in CancelRemoval method: '{ex.Message}'");
            }
            return genericResponse;
        }

        public async Task<bool> IsLastService(int serviceId, int providerProfileId)
        {
            var provider = await context.ProviderProfile
                    .Include(p => p.Services)
                    .FirstOrDefaultAsync(p => p.Id == providerProfileId);

            if(provider == null) { return false; }
            return provider.Services!.Where(s => s.Id != serviceId).All(s => s.IsInRegister == false);
        }
    }
}
