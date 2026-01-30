using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Repositories
{
    public class ActionLogRepository : IActionLogRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<ActionLogRepository> logger;

        public ActionLogRepository(DVSRegisterDbContext context, ILogger<ActionLogRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }      

        public async Task<ActionCategory> GetActionCategory(ActionCategoryEnum actionCategory)
        {            
            return await context.ActionCategory.Where(x=>x.ActionKey == actionCategory.ToString()).FirstOrDefaultAsync();
        }

        public async Task<ActionDetails> GetActionDetails(ActionDetailsEnum actionDetails)
        {
            return await context.ActionDetails.Where(x => x.ActionDetailsKey == actionDetails.ToString()).FirstOrDefaultAsync();
        }

        public async Task<GenericResponse> SaveActionLogs(ActionLogs actionLog)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if(actionLog != null)
                {
                    await context.ActionLogs.AddAsync(actionLog);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    genericResponse.Success = true;
                }
                else
                {
                    genericResponse.Success = false;
                    await transaction.RollbackAsync();
                }               
            }
            catch (Exception ex) 
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError("SaveCabTransferRequest failed with {exception} ", ex.Message);
            }
            return genericResponse;
        }


        public async Task<GenericResponse> SaveMultipleActionLogs(List<ActionLogs> actionLogs)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (actionLogs?.Count() > 0)
                {

                    await context.ActionLogs.AddRangeAsync(actionLogs);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    genericResponse.Success = true;

                }
                else
                {
                    genericResponse.Success = false;
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError("SaveMultipleActionLogs failed with {exception} ", ex.Message);
            }
            return genericResponse;
        }


    }
}
