using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {

        public Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs();
        public Task<List<ProviderDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "");
    }
}
