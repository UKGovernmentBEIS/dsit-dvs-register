﻿using DVSRegister.CommonUtility.Models;
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
        public async Task<List<Country>> GetCountries()
        {
            try
            {
                return await context.Country.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<GenericResponse> SavePreRegistration(PreRegistration preRegistration)
        {
            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                await context.PreRegistration.AddAsync(preRegistration);
                context.SaveChanges();
                transaction.Commit();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.EmailSent = false;
                genericResponse.Success = false;
                genericResponse.Message = "Db save failed.";
                transaction.Rollback();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }
        
    
    }
}
