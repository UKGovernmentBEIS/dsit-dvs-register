using DVSRegister.BusinessLogic.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IUserService
    {
        public Task<CabUserDto> UpdateCabUser(string email);
        public Task<CabUserDto> GetUser(string email); 
        public Task<List<string>> GetDSITUserEmails();
    }
}
