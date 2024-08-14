using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IUserService
    {
        public Task<CabUserDto> SaveUser(string email, string cabName);
        public Task<CabUserDto> GetUser(string email);
        public Task<CabDto> GetCab(string cabName);
    }
}
