using DVSRegister.BusinessLogic.Models;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace DVSRegister.BusinessLogic.Services
{
    public class ActionLogService :IActionLogService
    {
        private readonly IActionLogRepository actionLogRepository;
        private readonly IUserRepository userRepository;
    
        private readonly ILogger<ActionLogService> logger;

        public ActionLogService(IActionLogRepository actionLogRepository, IUserRepository userRepository, ILogger<ActionLogService> logger)
        {
         this.actionLogRepository = actionLogRepository;
         this.userRepository = userRepository;         
         this.logger = logger;
        }

        public async Task SaveActionLogs(ActionLogsDto actionLogsDto)
        {
            try
            {
              

                ActionLogs actionLog = await InitializeActionLogs(actionLogsDto);
                string displayMessage = string.Empty;
                var actionDetailsEnum = actionLogsDto.ActionDetailsEnum;
                string providerName = actionLogsDto.ProviderName;               

                if (actionLogsDto.ActionCategoryEnum == ActionCategoryEnum.ProviderUpdates)
                {
                    var previousData = actionLogsDto.PreviousData;
                    var updatedData = actionLogsDto.UpdatedData;
                    if (previousData != null && updatedData != null && previousData.Count>0 && updatedData.Count>0)
                    {
                        actionLog.OldValues = JsonDocument.Parse(JsonSerializer.Serialize(previousData));
                        actionLog.NewValues = JsonDocument.Parse(JsonSerializer.Serialize(updatedData));
                         if (actionDetailsEnum == ActionDetailsEnum.BusinessDetailsUpdate)
                        {
                            StringBuilder stringBuilder = new();
                            foreach (var item in previousData)
                            {
                                stringBuilder.Append(previousData[item.Key].FirstOrDefault() + " to " + updatedData[item.Key].FirstOrDefault() + " (" + item.Key + ")");
                                stringBuilder.AppendLine();
                            }
                            displayMessage = stringBuilder.ToString();
                            actionLog.ShowInRegisterUpdates = actionLogsDto.IsProviderPreviouslyPublished ? true : false;
                        }
                        else if (actionDetailsEnum == ActionDetailsEnum.ProviderContactUpdate)
                        {
                            if (!previousData.ContainsKey(Constants.PublicContactEmail) && !previousData.ContainsKey(Constants.ProviderWebsiteAddress) && !previousData.ContainsKey(Constants.ProviderTelephoneNumber))
                            {
                                actionLog.ShowInRegisterUpdates = false;
                            }
                            else
                            {
                                actionLog.ShowInRegisterUpdates = actionLogsDto.IsProviderPreviouslyPublished ? true : false;
                            }
                            displayMessage = providerName;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Previous data or updated data null");
                    }
                }      
                else if(actionLogsDto.ActionCategoryEnum == ActionCategoryEnum.CR || actionLogsDto.ActionCategoryEnum == ActionCategoryEnum.ActionRequests)
                {                   
                    actionLog.ShowInRegisterUpdates = false;                  
                }
              
                actionLog.DisplayMessage = displayMessage;              
                await actionLogRepository.SaveActionLogs(actionLog);
            
            }
            catch (Exception ex)
            {

                logger.LogError("ActionLogService: SaveActionLogs failed with {exception} ", ex.Message);
            }

        }

      

        

        #region Private methods
        private async Task<ActionLogs> InitializeActionLogs(ActionLogsDto actionLogsDto)
        {
            ActionCategory actionCategory = await actionLogRepository.GetActionCategory(actionLogsDto.ActionCategoryEnum);
            ActionDetails actionDetails = await actionLogRepository.GetActionDetails(actionLogsDto.ActionDetailsEnum);
            if (!string.IsNullOrEmpty(actionLogsDto.LoggedInUserEmail))
            {
                CabUser user = await userRepository.GetUser(actionLogsDto.LoggedInUserEmail);
                actionLogsDto.CabUserId = user.Id;
            }

            ActionLogs actionLog = new()
            {
                ActionCategoryId = actionCategory.Id,
                ActionDetailsId = actionDetails.Id,
                ServiceId = actionLogsDto.ServiceId,
                ProviderProfileId = actionLogsDto.ProviderId,
                LogDate = DateTime.UtcNow.Date,
                LoggedTime = DateTime.UtcNow,                
                OldValues = null,
                NewValues = null,
                ShowInRegisterUpdates = false,
                PublicInterestCheckId = actionLogsDto.PublicInterestCheckId > 0 ? actionLogsDto.PublicInterestCheckId : null,
                CertificateReviewId = actionLogsDto.CertificateReviewId > 0 ? actionLogsDto.CertificateReviewId : null,
                CabTransferRequestId = actionLogsDto.CabTransferRequestId > 0 ? actionLogsDto.CabTransferRequestId : null,
                ServiceRemovalRequestId = actionLogsDto.ServiceRemovalRequestId > 0 ? actionLogsDto.ServiceRemovalRequestId : null,
                CabUserId = actionLogsDto.CabUserId > 0 ? actionLogsDto.CabUserId : null,
                DisplayMessageAdmin = actionLogsDto.DisplayMessageAdmin,
                ServiceStatus = actionLogsDto.ServiceStatus
            };
            return actionLog;
        }
        #endregion


    }
}
