using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.Data;

namespace DVSRegister.BusinessLogic.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository registerRepository;      
        private readonly IMapper automapper;


        public RegisterService(IRegisterRepository registerRepository, IMapper automapper, ICabService cabService)
        {
            this.registerRepository = registerRepository;
            this.automapper = automapper;
         }
        public async Task<List<ProviderProfileDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "")
        {
        
            var list = await registerRepository.GetProviders(roles, schemes, searchText);
            var providerProfileList = list.Where(item => item.Services != null && item.Services.Any()).ToList();
            List<ProviderProfileDto> providerProfileDtos = automapper.Map<List<ProviderProfileDto>>(providerProfileList);            
            return providerProfileDtos;
           
        }        

        public async Task<ProviderProfileDto> GetProviderWithServiceDeatils(int providerId)
        {
            var provider = await registerRepository.GetProviderDetails(providerId);
            ProviderProfileDto providerProfileDto = automapper.Map<ProviderProfileDto>(provider);
            return providerProfileDto;
        }

        public async Task<ServiceDto> GetServiceDetails(int serviceId)
        {
            var service = await registerRepository.GetServiceDetails(serviceId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }

        public async Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs()
        {
            var list = await registerRepository.GetRegisterPublishLogs();
            return automapper.Map<List<RegisterPublishLogDto>>(list);
        }

       
    }
}
