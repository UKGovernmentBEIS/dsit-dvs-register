using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
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
    }
}
