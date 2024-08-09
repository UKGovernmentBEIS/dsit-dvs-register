using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Repositories
{
    public class UserRepository : IUserRepository
	{
        private readonly DVSRegisterDbContext context;

        public UserRepository(DVSRegisterDbContext context)
        {
            this.context = context;
        }

        public async Task<GenericResponse> AddUser(CabUser user)
        {
            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingEntity = await context.CabUser.FirstOrDefaultAsync<CabUser>(e => e.CabEmail == user.CabEmail);
                if(existingEntity == null)
                {
                    user.CreatedTime = DateTime.UtcNow;
                    await context.CabUser.AddAsync(user);
                    context.SaveChanges();
                    transaction.Commit();
                    genericResponse.Success = true;
                }  
            }
            catch(Exception ex)
            {                
                genericResponse.Success = false;
                transaction.Rollback();
                Console.Write($"Exception while adding user to table - {ex}");
            }

            return genericResponse;
        }


        public async Task<CabUser> GetUser(string email)
        {
            CabUser user = new CabUser();
            user = await context.CabUser.FirstOrDefaultAsync<CabUser>(e => e.CabEmail == email);
            return user;
        }

        public async Task<Cab> GetCab(string cabName)
        {
            Cab cab = new Cab();
            cab = await context.Cab.FirstOrDefaultAsync<Cab>(e => e.CabName == cabName);
            return cab;
        }
    }
}

