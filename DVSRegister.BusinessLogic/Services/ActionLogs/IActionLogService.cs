
using DVSRegister.BusinessLogic.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IActionLogService
    {
        public  Task SaveActionLogs(ActionLogsDto actionLogsDto);        
       
    }
}
