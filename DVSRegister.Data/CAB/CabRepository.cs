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
        public async Task<GenericResponse> SaveCertificateInformation(Provider provider)
        {

            GenericResponse genericResponse = new GenericResponse();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingProvider = await context.Provider.Include(p => p.CertificateInformation)
               .FirstOrDefaultAsync(p => p.PreRegistrationId == provider.PreRegistrationId);

                if (existingProvider !=null)
                {
                    existingProvider.RegisteredName  = provider.RegisteredName;
                    existingProvider.TradingName = provider.TradingName;
                    existingProvider.PublicContactEmail = provider.PublicContactEmail;
                    existingProvider.TelephoneNumber = provider.TelephoneNumber;
                    existingProvider.WebsiteAddress = provider.WebsiteAddress;
                    existingProvider.Address = provider.Address;                  
                    // at a time only one certificate info is passed from UI
                    existingProvider.CertificateInformation.Add(provider.CertificateInformation.ToList()[0]);
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = existingProvider.Id;
                }
                else
                {
                    provider.CreatedTime = DateTime.UtcNow;
                    var entity = await context.Provider.AddAsync(provider);
                    await context.SaveChangesAsync();
                    genericResponse.InstanceId = entity.Entity.Id;
                }
                transaction.Commit();
                genericResponse.Success = true;
              
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

        public async Task<List<Role>> GetRoles()
        {
            return await context.Role.OrderBy(c => c.RoleName).ToListAsync();
        }

        public async Task<List<IdentityProfile>> GetIdentityProfiles()
        {
            return await context.IdentityProfile.OrderBy(c => c.IdentityProfileName).ToListAsync();
        }

        public async Task<List<SupplementaryScheme>> GetSupplementarySchemes()
        {
            return await context.SupplementaryScheme.OrderBy(c => c.SchemeName).ToListAsync();
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
