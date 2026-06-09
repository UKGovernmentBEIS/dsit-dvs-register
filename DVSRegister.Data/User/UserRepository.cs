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

        public async Task<CabUser> UpdateCabUser(string email)
        {
           
            using var transaction = context.Database.BeginTransaction();
            CabUser cabUser = new();
            try
            {
                var existingEntity = await context.CabUser.FirstOrDefaultAsync<CabUser>(e => e.CabEmail.ToLower() == email.ToLower());
                if(existingEntity != null)
                {
                    existingEntity.LastLoggedIn = DateTime.UtcNow;
                    existingEntity.AccountStatus = AccountStatusEnum.Active;                    
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.UpdateUser, email);
                    transaction.Commit();                    
                    return existingEntity;                    
                }
                else
                {
                    transaction.Rollback();
                    Console.Write($"User not found");
                    return null!;
                }
               
               
            }
            catch(Exception ex)
            { 
                transaction.Rollback();
                Console.Write($"Exception while adding user to table - {ex}");
                return null!;
            }

           
        }


        public async Task<CabUser> GetUser(string email)
        {
            CabUser user = new CabUser();
            user = await context.CabUser.Include(x=>x.Cab).FirstOrDefaultAsync<CabUser>(e => e.CabEmail.ToLower() == email.ToLower())??new CabUser();
            return user;
        }

      

        public async Task<List<string>> GetDSITUserEmails()
        {
            List<string> userEmails = await context.User.Where(u => u.Profile == "DSIT")
            .Select(u => u.Email).ToListAsync() ?? new List<string>();
            return userEmails;
        }

        public async Task<List<User>> GetAllOfDIAManagerUsers()
        {
            return await context.User.AsNoTracking().Where(x => x.UserRole == UserRoleEnum.Manager && x.AccountStatus == AccountStatusEnum.Active).ToListAsync();
        }
    }
}

