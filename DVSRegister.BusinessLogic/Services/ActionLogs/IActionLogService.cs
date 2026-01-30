using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IActionLogService
    {
        public Task AddEditActionLogs(ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string email, ChangeSet changeSet, ProviderProfileDto providerProfileDto);
        public Task AddActionLog(ServiceDto serviceDto, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail, string? displayMessageAdmin = null);
        public Task AddMultipleActionLogs(List<ServiceDto> serviceDtos, ActionCategoryEnum actionCategory, ActionDetailsEnum actionDetails, string userEmail, string? displayMessageAdmin = null);


    }
}
