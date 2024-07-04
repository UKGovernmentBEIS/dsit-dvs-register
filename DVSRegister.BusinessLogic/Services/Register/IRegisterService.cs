using DVSRegister.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRegisterService
    {
        public Task<List<ProviderDto>> GetProviders(string providerName = "");
    }
}
