using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IUserRepository
	{
        public Task<GenericResponse> UpdateAccountStatus(string email, AccountStatusEnum accountStatus, string loggedInUser);

        public Task<CabUser> UpdateCabUser(string email);      

        public Task<CabUser> GetUser(string email);
	
        public Task<List<string>> GetDSITUserEmails();
        public Task<List<User>> GetAllOfDIAManagerUsers();

    }
}

