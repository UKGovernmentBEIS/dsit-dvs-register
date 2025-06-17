using AutoMapper;
using DVSRegister.Data.TrustFramework;

namespace DVSRegister.BusinessLogic.Services
{
    public class TrustFrameworkService(ITrustFrameworkRepository trustFrameworkRepository, IMapper automapper) : ITrustFrameworkService
    {

        private readonly ITrustFrameworkRepository trustFrameworkRepository = trustFrameworkRepository;
        private readonly IMapper automapper = automapper;
    }
}
