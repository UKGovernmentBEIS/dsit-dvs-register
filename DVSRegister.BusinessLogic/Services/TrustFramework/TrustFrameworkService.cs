using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data.CAB;
using DVSRegister.Data.TrustFramework;
using System;

namespace DVSRegister.BusinessLogic.Services
{
    public class TrustFrameworkService(ITrustFrameworkRepository trustFrameworkRepository, IMapper automapper) : ITrustFrameworkService
    {

        private readonly ITrustFrameworkRepository trustFrameworkRepository = trustFrameworkRepository;
        private readonly IMapper automapper = automapper;


        public async Task<List<TrustFrameworkVersionDto>> GetTrustFrameworkVersions()
        {
            var list = await trustFrameworkRepository.GetTrustFrameworkVersions();
            return automapper.Map<List<TrustFrameworkVersionDto>>(list);
        }
        public async Task<List<CabDto>> GetCabs()
        {
            var cabs = await trustFrameworkRepository.GetCabs();
            return automapper.Map<List<CabDto>>(cabs);
        }
    }
}
