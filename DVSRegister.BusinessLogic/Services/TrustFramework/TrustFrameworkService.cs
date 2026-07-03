using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data;

namespace DVSRegister.BusinessLogic.Services
{
    public class TrustFrameworkService(ITrustFrameworkRepository trustFrameworkRepository, ICommonRepository commonRepository, IMapper automapper) : ITrustFrameworkService
    {

        private readonly ITrustFrameworkRepository trustFrameworkRepository = trustFrameworkRepository;
        private readonly ICommonRepository commonRepository = commonRepository;
        private readonly IMapper automapper = automapper;


        public async Task<List<TrustFrameworkVersionDto>> GetActiveTrustFrameworkVersions()
        {
            var list = await commonRepository.GetActiveTfVersion();
            return automapper.Map<List<TrustFrameworkVersionDto>>(list);
        }
        public async Task<List<CabDto>> GetCabs()
        {
            var cabs = await trustFrameworkRepository.GetCabs();
            return automapper.Map<List<CabDto>>(cabs);
        }

        public async Task<List<ServiceDto>> GetPublishedUnderpinningServices(string searchText)
        {
            var services = await trustFrameworkRepository.GetPublishedUnderpinningServices(searchText);
            var serviceDtos = automapper.Map<List<ServiceDto>>(services);

            return serviceDtos;
        }

        public async Task<List<ServiceDto>> GetServicesWithManualUnderinningService(string searchText)
        {
            var services = await trustFrameworkRepository.GetServicesWithManualUnderinningService(searchText);
            var serviceDtos = automapper.Map<List<ServiceDto>>(services);

            return serviceDtos;
        }

        public async Task<ServiceDto> GetServiceDetails(int serviceId)
        {
            var service = await trustFrameworkRepository.GetServiceDetails(serviceId);
            return automapper.Map<ServiceDto>(service);
        }
    }
}
