using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IUserService
    {
        public Task<GenericResponse> SaveUser(string email, string cabName);
        public Task<CabUserDto> GetUser(string email);
        public Task<CabDto> GetCab(string cabName);
    }
}
