using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.Data;

namespace DVSRegister.BusinessLogic.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository registerRepository;
        private readonly IMapper automapper;


        public RegisterService(IRegisterRepository registerRepository, IMapper automapper)
        {
            this.registerRepository = registerRepository;
            this.automapper = automapper;
    }
        public async Task<List<ProviderDto>> GetProviders(string providerName = "")
        {
            var list = await registerRepository.GetProviders(providerName);
            return automapper.Map<List<ProviderDto>>(list);
        }
    }
}
