﻿using DVSRegister.BusinessLogic.Models;
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
                ActionLogs actionLog = await InitializeActionLogs(actionLogsDto.ActionCategoryEnum, actionLogsDto.ActionDetailsEnum, actionLogsDto.ProviderId);
                CabUser user = await userRepository.GetUser(actionLogsDto.LoggedInUserEmail);
                actionLog.CabUserId = user.Id;
                string displayMessage = string.Empty;
                var actionDetailsEnum = actionLogsDto.ActionDetailsEnum;
                string providerName = actionLogsDto.ProviderName;               

              
                if (actionLogsDto.ActionCategoryEnum == ActionCategoryEnum.ProviderUpdates)
                {
                 
                    actionLog.LoggedTime = DateTime.UtcNow;                    
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
                                stringBuilder.Append(item.Key + " "+ previousData[item.Key].FirstOrDefault() + " to " + updatedData[item.Key].FirstOrDefault());
                                stringBuilder.AppendLine();
                            }
                            displayMessage = stringBuilder.ToString();
                        }
                        else if (actionDetailsEnum == ActionDetailsEnum.ProviderContactUpdate)
                        {
                            if (!previousData.ContainsKey(Constants.PublicContactEmail) && !previousData.ContainsKey(Constants.ProviderWebsiteAddress) && !previousData.ContainsKey(Constants.ProviderTelephoneNumber))
                            {
                                actionLog.ShowInRegisterUpdates = false;
                            }
                            displayMessage = providerName;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Previous data or updated data null");
                    }
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
        private async Task<ActionLogs> InitializeActionLogs(ActionCategoryEnum actionCategoryEnum, ActionDetailsEnum actionDetailsEnum, int providerId)
        {
            ActionCategory actionCategory = await actionLogRepository.GetActionCategory(actionCategoryEnum);
            ActionDetails actionDetails = await actionLogRepository.GetActionDetails(actionDetailsEnum);


            ActionLogs actionLog = new()
            {
                ActionCategoryId = actionCategory.Id,
                ActionDetailsId = actionDetails.Id,                
                ProviderProfileId = providerId,
                LogDate = DateTime.UtcNow.Date,
                LoggedTime = DateTime.UtcNow,                
                OldValues = null,
                NewValues = null,
                ShowInRegisterUpdates = true
            };
            return actionLog;
        }
        #endregion


    }
}
