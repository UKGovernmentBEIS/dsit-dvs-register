﻿using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.Data;

namespace DVSRegister.BusinessLogic.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository registerRepository;
        private readonly ICabService cabService;
        private readonly IMapper automapper;


        public RegisterService(IRegisterRepository registerRepository, IMapper automapper, ICabService cabService)
        {
            this.registerRepository = registerRepository;
            this.automapper = automapper;
            this.cabService = cabService;
         }
        public async Task<List<ProviderDto>> GetProviders(List<int> roles, List<int> schemes,string searchText = "")
        {
            var list = await registerRepository.GetProviders(roles, schemes, searchText);
            var providerList = list.Where(item => item.CertificateInformation != null && item.CertificateInformation.Any()).ToList();
            List<ProviderDto> providerDtos = automapper.Map<List<ProviderDto>>(providerList);

            await PopulateRolesIdentityProfilesSchemes(providerDtos);
            return providerDtos;
        }        

        public async Task<ProviderDto> GetProviderWithServiceDeatils(int providerId)
        {
            var provider = await registerRepository.GetProviderDetails(providerId);
            ProviderDto providerDto = automapper.Map<ProviderDto>(provider);

            // assigning as list to reuse the private method PopulateRolesIdentityProfilesSchemes
            List<ProviderDto> providerDtos = new List<ProviderDto> { providerDto }; 
            await PopulateRolesIdentityProfilesSchemes(providerDtos);
            return providerDtos[0];
        }

        public async Task<List<RegisterPublishLogDto>> GetRegisterPublishLogs()
        {
            var list = await registerRepository.GetRegisterPublishLogs();
            return automapper.Map<List<RegisterPublishLogDto>>(list);
        }

        #region Private methods
        private async Task PopulateRolesIdentityProfilesSchemes(List<ProviderDto> providerDtos)
        {
            var rolesList = await cabService.GetRoles();
            List<RoleDto> roleDtos = automapper.Map<List<RoleDto>>(rolesList);

            var identityProfiles = await cabService.GetIdentityProfiles();
            List<IdentityProfileDto> identityProfileDtos = automapper.Map<List<IdentityProfileDto>>(identityProfiles);

            var schemesList = await cabService.GetSupplementarySchemes();
            List<SupplementarySchemeDto> supplementarySchemeDtos = automapper.Map<List<SupplementarySchemeDto>>(schemesList);
            //to populate roles, identity profiles, schemes
            foreach (var item in providerDtos)
            {
                int serviceNumber = 0;
                foreach (var service in item.CertificateInformation)
                {
                    service.ServiceNumber = ++serviceNumber;
                    var roleIds = service.CertificateInfoRoleMappings.Select(mapping => mapping.RoleId);
                    service.Roles = roleDtos.Where(x => roleIds.Contains(x.Id)).ToList();

                    var identityProfileIds = service.CertificateInfoIdentityProfileMappings.Select(mapping => mapping.IdentityProfileId);
                    service.IdentityProfiles = identityProfiles.Where(x => identityProfileIds.Contains(x.Id)).ToList();

                    var schemeids = service.CertificateInfoSupSchemeMappings?.Select(x => x.SupplementarySchemeId);
                    if (schemeids!=null && schemeids.Count() > 0)
                        service.SupplementarySchemes = supplementarySchemeDtos.Where(x => schemeids.Contains(x.Id)).ToList();
                }
            }
        }
        #endregion
    }
}
