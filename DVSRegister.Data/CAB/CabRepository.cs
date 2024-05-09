using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CAB
{
    public class CabRepository : ICabRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<PreRegistrationRepository> logger;

        public CabRepository(DVSRegisterDbContext context, ILogger<PreRegistrationRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }
        public async Task<GenericResponse> SaveCertificateInformation(CertificateInformation certificateInformation)
        {

            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var entity = await context.CertificateInformation.AddAsync(certificateInformation);
                await context.SaveChangesAsync();
                transaction.Commit();
                genericResponse.Success = true;
                genericResponse.InstanceId = entity.Entity.Id;
            }
            catch (Exception ex)
            {               
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }

        public async Task<UniqueReferenceNumber> GetURNDetails(string URN)
        {
            var uniqueReferenceNumber = await context.UniqueReferenceNumber.FirstOrDefaultAsync(e => e.URN == URN);
            return uniqueReferenceNumber;
        }

        public async Task<PreRegistration> GetPreRegistrationDetails(string URN)
        {
            var preRegistration = await context.PreRegistration.FirstOrDefaultAsync(e => e.URN == URN);
            return preRegistration;
        }

        public async Task<GenericResponse> UpdateURNStatus(UniqueReferenceNumber uniqueReferenceNumber)
        {

            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingEntity = await context.UniqueReferenceNumber.FirstOrDefaultAsync(e => e.URN == uniqueReferenceNumber.URN);
                if (existingEntity != null)
                {
                    existingEntity.URNStatus = uniqueReferenceNumber.URNStatus;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    genericResponse.Success = true;
                }

            }
            catch (Exception ex)
            {
                genericResponse.EmailSent = false;
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex.Message);
            }
            return genericResponse;

        }
    }
}
