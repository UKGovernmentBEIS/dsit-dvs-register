using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IActionLogRepository
    {
        public Task<ActionCategory> GetActionCategory(ActionCategoryEnum actionCategory);
        public Task<ActionDetails> GetActionDetails(ActionDetailsEnum actionDetails);
        public Task<GenericResponse> SaveActionLogs(ActionLogs actionLogs);
        public Task<GenericResponse> SaveMultipleActionLogs(List<ActionLogs> actionLogs);
    }
}
