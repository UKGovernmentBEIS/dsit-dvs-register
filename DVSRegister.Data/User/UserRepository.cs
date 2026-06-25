using DVSRegister.CommonUtility.Models;
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

        public async Task<GenericResponse> UpdateAccountStatus(string email, AccountStatusEnum accountStatus, string loggedInUser)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            GenericResponse genericResponse = new();
            try
            {
                var existingUser = await context.CabUser.Where(x => x.CabEmail.ToLower() == email.ToLower()).FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    existingUser.ModifiedDate = DateTime.UtcNow;
                    existingUser.AccountStatus = accountStatus;
                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.UpdateUser, loggedInUser);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    genericResponse.Success = false;
                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                Console.Write($"Exception in UpdateAccountStatus - {ex}");
            }
            return genericResponse;
        }

        public async Task<CabUser> UpdateCabUser(string email)
        {
           
            using var transaction = await context.Database.BeginTransactionAsync();
            CabUser cabUser = new();
            try
            {
                var existingEntity = await context.CabUser.FirstOrDefaultAsync<CabUser>(e => e.CabEmail.ToLower() == email.ToLower());
                if(existingEntity != null)
                {
                    existingEntity.LastLoggedIn = DateTime.UtcNow;                                    
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.UpdateUser, email);
                    await transaction.CommitAsync();                    
                    return existingEntity;                    
                }
                else
                {
                    await transaction.RollbackAsync();
                    Console.Write($"User not found");
                    return null!;
                }
               
               
            }
            catch(Exception ex)
            { 
                await transaction.RollbackAsync();
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
            List<string> userEmails = await context.User.Where(u => u.Profile == "DSIT" && u.AccountStatus == AccountStatusEnum.Active)
            .Select(u => u.Email).ToListAsync() ?? new List<string>();
            return userEmails;
        }

        public async Task<List<User>> GetAllOfDIAManagerUsers()
        {
            return await context.User.AsNoTracking().Where(x => x.UserRole == UserRoleEnum.Manager && x.AccountStatus == AccountStatusEnum.Active).ToListAsync();
        }
    }
}

