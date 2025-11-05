using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
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
        public async Task<PaginatedResult<ProviderProfileDto>> GetProviders(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "")
        {        
            var providers = await registerRepository.GetProviders(roles, schemes, tfVersions, pageNum, searchText, sortBy);
            List<ProviderProfileDto> providerProfileDtos = automapper.Map<List<ProviderProfileDto>>(providers.Items);
            return new PaginatedResult<ProviderProfileDto>
            {
                Items = providerProfileDtos,
                TotalCount = providers.TotalCount
            };
        }

        public async Task<PaginatedResult<ServiceDto>> GetServices(List<int> roles, List<int> schemes, List<int> tfVersions, int pageNum, string searchText = "", string sortBy = "")
        {
            var services = await registerRepository.GetServices(roles, schemes, tfVersions, pageNum, searchText, sortBy);
            List<ServiceDto> serviceDtos = automapper.Map<List<ServiceDto>>(services.Items);
            return new PaginatedResult<ServiceDto>
            {
                Items = serviceDtos,
                TotalCount = services.TotalCount
            };
        }

        public async Task<ProviderProfileDto> GetProviderWithServiceDeatils(int providerId)
        {
            var provider = await registerRepository.GetProviderDetails(providerId);
            if (provider == null) return null!;
            ProviderProfileDto providerProfileDto = automapper.Map<ProviderProfileDto>(provider);
            return providerProfileDto;
        }
        public async Task<ServiceDto> GetServiceDetails(int serviceId)
        {
            var service = await registerRepository.GetServiceDetails(serviceId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }
        public async Task<PaginatedResult<GroupedResult<ActionLogsDto>>> GetUpdateLogs(int pageNumber)
        {
            var list = await registerRepository.GetUpdateLogs(pageNumber);
      
            var groupedDtos = list.Items.Select(group => new GroupedResult<ActionLogsDto>
            {
                LogDate = group.LogDate,
                Items = automapper.Map<List<ActionLogsDto>>(group.Items)
            }).ToList();

            return new PaginatedResult<GroupedResult<ActionLogsDto>>
            {
                Items = groupedDtos,
                TotalCount = list.TotalCount,
                LastUpdated = list.LastUpdated
            };
        }


        public async Task<DateTime?> GetLastUpdatedDate()
        {
            return await registerRepository.GetLastUpdatedDate();
        }
        public async Task<List<ServiceDto>> GetPublishedServices()
        {
            var services = await registerRepository.GetPublishedServices();
            List<ServiceDto> serviceDtos = automapper.Map<List<ServiceDto>>(services);
            return serviceDtos;

        }

    }
}
