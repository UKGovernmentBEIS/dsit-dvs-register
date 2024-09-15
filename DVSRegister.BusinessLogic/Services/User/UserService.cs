using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
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
      
        public async Task<CabUserDto> SaveUser(string email , string cabName)
        {
            Cab cab = await userRepository.GetCab(cabName);
            CabUser cabUser = new CabUser();
            cabUser.CabEmail = email;
            cabUser.CabId = cab.Id;
            var user = await userRepository.AddUser(cabUser);
            CabUserDto userDto = automapper.Map<CabUserDto>(user);
            return userDto;// return current user details


        }

        public async Task<CabUserDto> GetUser(string email)
        {
            var user = await userRepository.GetUser(email);
            CabUserDto userDto = automapper.Map<CabUserDto>(user);
            return userDto;
        }

        public async Task<CabDto> GetCab(string cabName)
        {
            var cab = await userRepository.GetCab(cabName);
            CabDto cabDto = automapper.Map<CabDto>(cab);
            return cabDto;
        }
    }
}
