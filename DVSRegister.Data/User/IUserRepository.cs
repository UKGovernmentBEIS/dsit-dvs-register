using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
	public interface IUserRepository
	{
		public Task<GenericResponse> AddUser(CabUser user);
		public Task<CabUser> GetUser(string email);
		public Task<Cab> GetCab(string cabName);

    }
}

