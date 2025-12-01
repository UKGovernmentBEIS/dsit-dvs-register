using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Edit
{
    public class EditRepository :IEditRepository
    {
        private readonly DVSRegisterDbContext _context;
        private readonly ILogger<EditRepository> _logger;

        public EditRepository(DVSRegisterDbContext context, ILogger<EditRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<GenericResponse> SaveProviderDraft(ProviderProfileDraft draft, string loggedInUserEmail)
        {
            var response = new GenericResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cabUser = await _context.CabUser.FirstOrDefaultAsync(x => x.CabEmail == loggedInUserEmail);
                draft.RequestedCabUserId = cabUser!.Id;
                draft.RequestedUserId = null;
               draft.IsCabRequested = true;
                var existingDraft = await _context.ProviderProfileDraft
                    .FirstOrDefaultAsync(x => x.ProviderProfileId == draft.ProviderProfileId && x.Id == draft.Id);

                var provider = await _context.ProviderProfile.Include(p => p.Services)!.ThenInclude(x=>x.CabUser).FirstOrDefaultAsync(x => x.Id == draft.ProviderProfileId);

                if (existingDraft != null)
                {
                    throw new InvalidOperationException("Edit request already exists for the provider");
                }
                else
                {
                    draft.ModifiedTime = DateTime.UtcNow;
                    await _context.ProviderProfileDraft.AddAsync(draft);
                    var servicesList = provider?.Services?.Where(x => x.ServiceStatus == ServiceStatusEnum.Published );

                    provider.ProviderStatus = ProviderStatusEnum.UpdatesRequested;
                    provider.ModifiedTime = DateTime.UtcNow;
                    if (servicesList != null && servicesList.Count() > 0)
                    {
                        foreach (var service in servicesList)
                        {
                            service.ServiceStatus = ServiceStatusEnum.UpdatesRequested;
                            service.ModifiedTime = DateTime.UtcNow;
                        }
                    }
                    await _context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.CabEditProvider, loggedInUserEmail);
                    response.InstanceId = draft.Id;
                }

                await transaction.CommitAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                _logger.LogError(ex, "Error in SaveProviderDraft");
            }
            return response;
        }

        public async Task<ProviderProfile> GetProviderDetails(int providerId, int cabId)
        {
        var providerProfile = await _context.ProviderProfile
        .Include(p => p.Services)
        .Include(p => p.ProviderProfileCabMapping)
        .Include(p => p.ProviderProfileDraft)
        .Where(p => p.Id == providerId && p.ProviderProfileCabMapping.Any(m => m.CabId == cabId)).FirstOrDefaultAsync();
        return providerProfile!;
        }
        public async Task<GenericResponse> UpdateCompanyInfoAndPublicProviderInfo(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await _context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);

                if (existingProvider != null)
                {
                    existingProvider.RegisteredName = providerProfile.RegisteredName;
                    existingProvider.TradingName = providerProfile.TradingName;
                    existingProvider.HasParentCompany = providerProfile.HasParentCompany;
                    existingProvider.HasRegistrationNumber = providerProfile.HasRegistrationNumber;
                    if (providerProfile.HasParentCompany == false)
                    {
                        existingProvider.ParentCompanyLocation = null;
                        existingProvider.ParentCompanyRegisteredName = null;

                    }
                    else if(providerProfile.HasParentCompany == true && !string.IsNullOrEmpty(providerProfile.ParentCompanyLocation) && !string.IsNullOrEmpty(providerProfile.ParentCompanyRegisteredName))
                    {
                        existingProvider.ParentCompanyLocation = providerProfile.ParentCompanyLocation;
                        existingProvider.ParentCompanyRegisteredName = providerProfile.ParentCompanyRegisteredName;
                    }

                    if(providerProfile.HasRegistrationNumber == true && !string.IsNullOrEmpty(providerProfile.CompanyRegistrationNumber))
                    {
                        existingProvider.CompanyRegistrationNumber = providerProfile.CompanyRegistrationNumber;
                        existingProvider.DUNSNumber = null;
                    }
                    else if(providerProfile.HasRegistrationNumber == false && !string.IsNullOrEmpty(providerProfile.DUNSNumber))
                    {
                        existingProvider.DUNSNumber = providerProfile.DUNSNumber;
                        existingProvider.CompanyRegistrationNumber = null;
                    }                    


                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    existingProvider.PublicContactEmail = providerProfile.PublicContactEmail;
                    existingProvider.ProviderTelephoneNumber = providerProfile.ProviderTelephoneNumber;
                    existingProvider.ProviderWebsiteAddress = providerProfile.ProviderWebsiteAddress;
                    existingProvider.LinkToContactPage = providerProfile.LinkToContactPage;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.CompanyInfoUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                _logger.LogError(ex, "Error in UpdateCompanyInfo");
            }
            return genericResponse;
        }

 
        public async Task<GenericResponse> UpdatePrimaryContact(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingProvider = await _context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);
                if (existingProvider != null)
                {
                    existingProvider.PrimaryContactFullName = providerProfile.PrimaryContactFullName;
                    existingProvider.PrimaryContactJobTitle = providerProfile.PrimaryContactJobTitle;
                    existingProvider.PrimaryContactEmail = providerProfile.PrimaryContactEmail;
                    existingProvider.PrimaryContactTelephoneNumber = providerProfile.PrimaryContactTelephoneNumber;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.PrimaryContactUpdate, loggedInUserEmail);
                    transaction.Commit();
                    genericResponse.Success = true;

                }
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                _logger.LogError(ex, "Error in UpdatePrimaryContact");
            }
            return genericResponse;
        }
        public async Task<GenericResponse> UpdateSecondaryContact(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await _context.ProviderProfile.FirstOrDefaultAsync(p => p.Id == providerProfile.Id);
                if (existingProvider != null)
                {
                    existingProvider.SecondaryContactFullName = providerProfile.SecondaryContactFullName;
                    existingProvider.SecondaryContactJobTitle = providerProfile.SecondaryContactJobTitle;
                    existingProvider.SecondaryContactEmail = providerProfile.SecondaryContactEmail;
                    existingProvider.SecondaryContactTelephoneNumber = providerProfile.SecondaryContactTelephoneNumber;
                    existingProvider.CabEditedTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.SecondaryContactUpdate, loggedInUserEmail);
                    await transaction.CommitAsync();
                    genericResponse.Success = true;

                }

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error in UpdateSecondaryContact");
            }
            return genericResponse;
        }
      

    }
}
