using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Repositories
{
    public class PreRegistrationRepository : IPreRegistrationRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<PreRegistrationRepository> logger;

        public PreRegistrationRepository(DVSRegisterDbContext context, ILogger<PreRegistrationRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }

        public Task<List<Country>> GetCountries()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> SavePreRegistration(PreRegistration preAssessment)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> SaveURN(UniqueReferenceNumber uniqueReferenceNumber)
        {
            throw new NotImplementedException();
        }
        //public async Task<List<Country>> GetCountries()
        //{
        //    return await context.Country.OrderBy(c => c.CountryName).ToListAsync();
        //}

        //public async Task<GenericResponse> SavePreRegistration(PreRegistration preRegistration)
        //{
        //    GenericResponse genericResponse = new GenericResponse();
        //    using var transaction = context.Database.BeginTransaction();
        //    try
        //    {
        //        var entity = await context.PreRegistration.AddAsync(preRegistration);
        //        await context.SaveChangesAsync();
        //        transaction.Commit();
        //        genericResponse.Success = true;
        //        genericResponse.InstanceId = entity.Entity.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        genericResponse.EmailSent = false;
        //        genericResponse.Success = false;              
        //        transaction.Rollback();
        //        logger.LogError(ex.Message);
        //    }
        //    return genericResponse;
        //}

        //public async Task<GenericResponse> SaveURN(UniqueReferenceNumber uniqueReferenceNumber)
        //{
        //    GenericResponse genericResponse = new GenericResponse();
        //    using var transaction = context.Database.BeginTransaction();
        //    try
        //    {
        //        await context.UniqueReferenceNumber.AddAsync(uniqueReferenceNumber);
        //        await context.SaveChangesAsync();
        //        transaction.Commit();
        //        genericResponse.Success = true;
        //    }
        //    catch(Exception ex)
        //    {
        //        genericResponse.EmailSent = false;
        //        genericResponse.Success = false;
        //        transaction.Rollback();
        //        logger.LogError(ex.Message);
        //    }

        //    return genericResponse;
        //}
    }
}
