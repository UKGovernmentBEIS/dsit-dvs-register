using DVSRegister.CommonUtility.Models.Enums;
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

        public async Task<CabUser> AddUser(CabUser user)
        {
           
            using var transaction = context.Database.BeginTransaction();
            CabUser cabUser = new();
            try
            {
                var existingEntity = await context.CabUser.Include(c=>c.Cab).FirstOrDefaultAsync<CabUser>(e => e.CabEmail == user.CabEmail);
                if(existingEntity == null)
                {
                    user.CreatedTime = DateTime.UtcNow;
                    var entity = await context.CabUser.AddAsync(user);
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddService, user.CabEmail);
                    transaction.Commit();                    
                    return user;


                }
                return existingEntity;
               
            }
            catch(Exception ex)
            {                
                
                transaction.Rollback();
                Console.Write($"Exception while adding user to table - {ex}");
                return null;
            }

           
        }


        public async Task<CabUser> GetUser(string email)
        {
            CabUser user = new CabUser();
            user = await context.CabUser.Include(x=>x.Cab).FirstOrDefaultAsync<CabUser>(e => e.CabEmail == email)??new CabUser();
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

