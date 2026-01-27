using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
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

        public ActionLogService(IActionLogRepository actionLogRepository, IUserRepository userRepository,ILogger<ActionLogService> logger)
        {
         this.actionLogRepository = actionLogRepository;
         this.userRepository = userRepository;            
         this.logger = logger;
        }


        /// <summary>
        /// Save all actions that performs edit
        /// In cab only provider edits currently
        /// </summary>
        /// <param name="actionCategory"></param>
        /// <param name="actionDetails"></param>
        /// <param name="email"></param>
        /// <param name="current"></param>
        /// <param name="previous"></param>
        /// <param name="displayMessageAdmin"></param>
        /// <param name="providerProfileDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task AddEditActionLogs( ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails,string email,ChangeSet changeSet, ProviderProfileDto providerProfileDto)
        {
            try
            {

                ActionLogsDto actionLogsDto = InitializeEditActionLogDto(actionCategory, actionDetails, email, changeSet, providerProfileDto);
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
                    actionLog.DisplayMessage = displayMessage;
                    await actionLogRepository.SaveActionLogs(actionLog);
                }
            
            }
            catch (Exception ex)
            {

                logger.LogError("ActionLogService: SaveActionLogs failed with {exception} ", ex.Message);
            }

        }

        /// <summary>
        /// If there is no change in the display message or no display message
        /// </summary>
        /// <param name="serviceDto"></param>
        /// <param name="actionCategory"></param>
        /// <param name="actionDetails"></param>
        /// <param name="userEmail"></param>
        /// <param name="displayMessageAdmin"></param>
        /// <returns></returns>
        public async Task AddActionLog(ServiceDto serviceDto, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail, string? displayMessageAdmin = null)
        {

            ArgumentNullException.ThrowIfNull(serviceDto, nameof(serviceDto));
            ArgumentNullException.ThrowIfNull(serviceDto.Provider, $"{nameof(serviceDto)}.{nameof(serviceDto.Provider)}");

            ActionLogsDto actionLogsDto = InitializeActionLogDto(serviceDto, actionCategory, actionDetails, userEmail, displayMessageAdmin);
            ActionLogs actionLog = await InitializeActionLogs(actionLogsDto);

            await actionLogRepository.SaveActionLogs(actionLog);

        }

        /// <summary>
        /// For saving bulk logs with no change in the display message or no display message
        /// </summary>
        /// <param name="serviceDtos"></param>
        /// <param name="actionCategory"></param>
        /// <param name="actionDetails"></param>
        /// <param name="userEmail"></param>
        /// <param name="displayMessageAdmin"></param>
        /// <returns></returns>

        public async Task AddMultipleActionLogs(List<ServiceDto> serviceDtos, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail, string? displayMessageAdmin = null)
        {

            ArgumentNullException.ThrowIfNull(serviceDtos, nameof(serviceDtos));
            List<ActionLogs> actionLogs = [];
            foreach (var service in serviceDtos)
            {
                ActionLogsDto actionLogsDto = InitializeActionLogDto(service, actionCategory, actionDetails, userEmail, displayMessageAdmin);
                ActionLogs actionLog = await InitializeActionLogs(actionLogsDto);
                actionLogs.Add(actionLog);
            }


            await actionLogRepository.SaveMultipleActionLogs(actionLogs);

        }




        #region Private methods

        private static ActionLogsDto InitializeActionLogDto(ServiceDto serviceDto, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail, string? displayMessageAdmin)
        {
            return new ActionLogsDto
            {
                LoggedInUserEmail = userEmail,
                ActionCategoryEnum = actionCategory,
                ActionDetailsEnum = actionDetails,
                ServiceId = serviceDto.Id,
                ServiceName = serviceDto.ServiceName,
                ServiceStatus = serviceDto.ServiceStatus,
                ProviderId = serviceDto.Provider.Id,
                ProviderName = serviceDto.Provider.RegisteredName ?? string.Empty,
                PublicInterestCheckId = serviceDto.PublicInterestCheck?
                                                       .FirstOrDefault(x => x.IsLatestReviewVersion)?.Id,
                CertificateReviewId = serviceDto.CertificateReview?
                                                       .FirstOrDefault(x => x.IsLatestReviewVersion)?.Id,
                CabTransferRequestId = serviceDto.CabTransferRequestId,
                ServiceRemovalRequestId = serviceDto.ServiceRemovalRequestId,
                ProviderRemovalRequestId = serviceDto?.ProviderRemovalRequestServiceMapping?.ProviderRemovalRequestId,
                DisplayMessageAdmin = displayMessageAdmin
            };
        }

        private static ActionLogsDto InitializeEditActionLogDto(
        ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail,
        ChangeSet changeSet, ProviderProfileDto providerProfileDto)
        {
            return new ActionLogsDto
            {
                ActionCategoryEnum = actionCategory,
                ActionDetailsEnum = actionDetails,
                LoggedInUserEmail = userEmail,
                ProviderId = providerProfileDto.Id,
                ProviderName = providerProfileDto.RegisteredName,
                PreviousData = changeSet.Previous,
                UpdatedData = changeSet.Current,                
                IsProviderPreviouslyPublished = providerProfileDto.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false
        };
        }
        private async Task<ActionLogs> InitializeActionLogs(ActionLogsDto actionLogsDto)
        {
            ActionCategory actionCategory = await actionLogRepository.GetActionCategory(actionLogsDto.ActionCategoryEnum);
            ActionDetails actionDetails = await actionLogRepository.GetActionDetails(actionLogsDto.ActionDetailsEnum);
            CabUser cabUser = new();
            if (!string.IsNullOrEmpty(actionLogsDto.LoggedInUserEmail))
            {
                cabUser = await userRepository.GetUser(actionLogsDto.LoggedInUserEmail);
              
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
                PublicInterestCheckId = actionLogsDto.PublicInterestCheckId > 0 ? actionLogsDto.PublicInterestCheckId : null,
                CertificateReviewId = actionLogsDto.CertificateReviewId > 0 ? actionLogsDto.CertificateReviewId : null,
                CabTransferRequestId = actionLogsDto.CabTransferRequestId > 0 ? actionLogsDto.CabTransferRequestId : null,
                ServiceRemovalRequestId = actionLogsDto.ServiceRemovalRequestId > 0 ? actionLogsDto.ServiceRemovalRequestId : null,
                ProviderRemovalRequestId = actionLogsDto.ProviderRemovalRequestId > 0 ? actionLogsDto.ProviderRemovalRequestId : null,
                CabUserId = cabUser?.Id>0?cabUser.Id:null,
                UpdateRequestedUserId = actionLogsDto.RequestedUserId > 0 ? actionLogsDto.RequestedUserId : null,
                UpdateRequestedTime = actionLogsDto.UpdateRequestedTime,
                DisplayMessageAdmin = actionLogsDto.DisplayMessageAdmin,
                DisplayMessage = actionLogsDto.DisplayMessage == null ? string.Empty : actionLogsDto.DisplayMessage,
                ServiceStatus = actionLogsDto.ServiceStatus
             

            };
            return actionLog;
        }
        #endregion            


    }
}
