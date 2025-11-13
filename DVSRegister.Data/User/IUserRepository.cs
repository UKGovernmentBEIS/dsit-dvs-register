using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IUserRepository
	{
		public Task<CabUser> AddUser(CabUser user);
		public Task<CabUser> GetUser(string email);
		public Task<Cab> GetCab(string cabName);
        public Task<List<string>> GetDSITUserEmails();

    }
}

