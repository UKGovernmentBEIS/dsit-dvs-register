using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.Data.Repositories;

namespace DVSRegister.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper automapper;
       

        public UserService(IUserRepository userRepository, IMapper automapper)
        {
            this.userRepository = userRepository;
            this.automapper = automapper;
        }
      
        public async Task<CabUserDto> UpdateCabUser(string email)
        {
           
            var user = await userRepository.UpdateCabUser(email);
            CabUserDto userDto = automapper.Map<CabUserDto>(user);
            return userDto;// return current user details


        }

        public async Task<CabUserDto> GetUser(string email)
        {
            var user = await userRepository.GetUser(email);
            CabUserDto userDto = automapper.Map<CabUserDto>(user);
            return userDto;
        }

   
        public async Task<List<string>> GetDSITUserEmails()
        {
            var userEmails = await userRepository.GetDSITUserEmails();
            return userEmails;
        }
    }
}
