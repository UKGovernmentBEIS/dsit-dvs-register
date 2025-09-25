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
                //update provider status
                var providerEntity = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(e => e.Id == providerProfileId);
                ProviderStatusEnum providerStatus = RepositoryHelper.GetProviderStatus(providerEntity.Services, providerEntity.ProviderStatus);
                providerEntity.ProviderStatus = providerStatus;
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
