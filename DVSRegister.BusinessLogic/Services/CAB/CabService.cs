using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public class CabService : ICabService
    {
        private readonly ICabRepository cabRepository;
        private readonly IMapper automapper;
        private readonly IEmailSender emailSender;

        public CabService(ICabRepository cabRepository, IMapper automapper, IEmailSender emailSender)
        {
            this.cabRepository = cabRepository;
            this.automapper = automapper;
            this.emailSender = emailSender;
        }
     
        public async Task<List<RoleDto>> GetRoles()
        {
            var list = await cabRepository.GetRoles();
            return automapper.Map<List<RoleDto>>(list);
        }

        public async Task<List<SupplementarySchemeDto>> GetSupplementarySchemes()
        {
            var list = await cabRepository.GetSupplementarySchemes();
            return automapper.Map<List<SupplementarySchemeDto>>(list);
        }
        public async Task<List<IdentityProfileDto>> GetIdentityProfiles()
        {
            var list = await cabRepository.GetIdentityProfiles();
            return automapper.Map<List<IdentityProfileDto>>(list);
        }



        public async Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfileDto)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile);
            return genericResponse;
        }

        public async Task<GenericResponse> UpdateProviderProfile(ProviderProfileDto providerProfileDto)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.UpdateProviderProfile(providerProfile);
            return genericResponse;
        }

        public async Task<GenericResponse> SaveService(ServiceDto serviceDto)
        {
            Service service = new Service();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveService(service);
            return genericResponse;
        }

        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName)
        {
            return await cabRepository.CheckProviderRegisteredNameExists(registeredName);
        }

        public async Task<List<ProviderProfileDto>> GetProviders(int cabId, string searchText = "")
        {
            var list = await cabRepository.GetProviders(cabId, searchText);
            List<ProviderProfileDto> providerDtos = automapper.Map<List<ProviderProfileDto>>(list);
            return providerDtos;
        }

        public async Task<ProviderProfileDto> GetProvider(int providerId, int cabId)
        {
            var provider = await cabRepository.GetProvider(providerId, cabId);
            ProviderProfileDto providerDto = automapper.Map<ProviderProfileDto>(provider);
            return providerDto;
        }
        public async Task<ServiceDto> GetServiceDetails(int serviceId, int cabId)
        {
            var service = await cabRepository.GetServiceDetails(serviceId, cabId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }
        public async Task<List<QualityLevelDto>> GetQualitylevels()
        {
            var list = await cabRepository.QualityLevels();
            return automapper.Map<List<QualityLevelDto>>(list);
        }
        public async Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId)
        {
            return await cabRepository.CheckValidCabAndProviderProfile(providerId, cabId);

        }

    }
}
