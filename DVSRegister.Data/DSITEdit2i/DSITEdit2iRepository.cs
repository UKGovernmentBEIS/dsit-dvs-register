using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.Repositories
{
    public class DSITEdit2iRepository : IDSITEdit2iRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<DSITEdit2iRepository> logger;

        public DSITEdit2iRepository(DVSRegisterDbContext context, ILogger<DSITEdit2iRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }


        #region Provider update
        public async Task<ProviderDraftToken> GetProviderDraftToken(string token, string tokenId)
        {
            return await context.ProviderDraftToken.Include(p => p.ProviderProfileDraft).ThenInclude(p => p.Provider)
            .Include(p => p.ProviderProfileDraft).ThenInclude(p => p.User)
           .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId) ?? new ProviderDraftToken();
        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingProvider = await context.ProviderProfile.Include(p=>p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
                var existingDraftProvider = await context.ProviderProfileDraft.FirstOrDefaultAsync(p => p.Id == providerDraftId);

                var serviceDrafts = await context.ServiceDraft.Where(p => p.ProviderProfileId == providerProfileId).
               Include(P => P.ServiceRoleMappingDraft).Include(P => P.ServiceQualityLevelMappingDraft).Include(p => p.ServiceIdentityProfileMappingDraft)
               .Include(p => p.ServiceSupSchemeMappingDraft).ToListAsync();

                if (existingProvider != null && existingProvider.ProviderStatus == ProviderStatusEnum.UpdatesRequested && existingDraftProvider!=null 
                    && existingDraftProvider.ProviderProfileId == existingProvider.Id)
                {
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.ProviderStatus = ProviderStatusEnum.ReadyToPublish; 
                    existingProvider.IsInRegister= false;
                    existingProvider.EditProviderTokenStatus = TokenStatusEnum.RequestCompleted;

                    existingProvider.RegisteredName = existingDraftProvider.RegisteredName ?? existingProvider.RegisteredName;
                    existingProvider.TradingName = existingDraftProvider.TradingName != null
                    ? (existingDraftProvider.TradingName == "-" ? string.Empty : existingDraftProvider.TradingName)
                    : existingProvider.TradingName;

                    existingProvider.PrimaryContactFullName = existingDraftProvider.PrimaryContactFullName ?? existingProvider.PrimaryContactFullName;
                    existingProvider.PrimaryContactJobTitle = existingDraftProvider.PrimaryContactJobTitle ?? existingProvider.PrimaryContactJobTitle;
                    existingProvider.PrimaryContactEmail = existingDraftProvider.PrimaryContactEmail ?? existingProvider.PrimaryContactEmail;
                    existingProvider.PrimaryContactTelephoneNumber = existingDraftProvider.PrimaryContactTelephoneNumber ?? existingProvider.PrimaryContactTelephoneNumber;


                    existingProvider.SecondaryContactFullName = existingDraftProvider.SecondaryContactFullName ?? existingProvider.SecondaryContactFullName;
                    existingProvider.SecondaryContactJobTitle = existingDraftProvider.SecondaryContactJobTitle ?? existingProvider.SecondaryContactJobTitle;
                    existingProvider.SecondaryContactEmail = existingDraftProvider.SecondaryContactEmail ?? existingProvider.SecondaryContactEmail;
                    existingProvider.SecondaryContactTelephoneNumber = existingDraftProvider.SecondaryContactTelephoneNumber ?? existingProvider.SecondaryContactTelephoneNumber;

                    existingProvider.ProviderWebsiteAddress = existingDraftProvider.ProviderWebsiteAddress ?? existingProvider.ProviderWebsiteAddress;
                    

                    existingProvider.PublicContactEmail = existingDraftProvider.PublicContactEmail != null
                  ? (existingDraftProvider.PublicContactEmail == "-" ? string.Empty : existingDraftProvider.PublicContactEmail)
                  : existingProvider.PublicContactEmail;

                    existingProvider.ProviderTelephoneNumber = existingDraftProvider.ProviderTelephoneNumber != null
                    ? (existingDraftProvider.ProviderTelephoneNumber == "-" ? string.Empty : existingDraftProvider.ProviderTelephoneNumber)
                    : existingProvider.ProviderTelephoneNumber;

                    // Update the status of each service
                    foreach (var serviceDraft in serviceDrafts)
                    {
                        var service = await context.Service.Where(s => s.Id == serviceDraft.ServiceId).FirstOrDefaultAsync();
                        service.ServiceStatus = ServiceStatusEnum.ReadyToPublish;
                        service.IsInRegister = false;
                        service.ModifiedTime = DateTime.UtcNow;
                        context.Remove(serviceDraft);
                    }
                    context.Remove(existingDraftProvider);
                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
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
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }



        public async Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = new();
            var existingProvider = await context.ProviderProfile.Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == providerProfileId);
            var existingDraftProvider = await context.ProviderProfileDraft.FirstOrDefaultAsync(p => p.Id == providerDraftId);
            var serviceDrafts = await context.ServiceDraft.Where(p => p.ProviderProfileId == providerProfileId). 
            Include(P=>P.ServiceRoleMappingDraft).Include(P=>P.ServiceQualityLevelMappingDraft).Include(p=>p.ServiceIdentityProfileMappingDraft)
            .Include(p=>p.ServiceSupSchemeMappingDraft). ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (existingProvider != null && existingDraftProvider!=null )// assign back the previous status
                {

                    existingProvider.ProviderStatus = existingDraftProvider.PreviousProviderStatus;
                    existingProvider.ModifiedTime = DateTime.UtcNow;
                    existingProvider.EditProviderTokenStatus = TokenStatusEnum.UserCancelled;
                    foreach (var serviceDraft in serviceDrafts)
                    {
                        var service = await context.Service.Where(s => s.Id == serviceDraft.ServiceId). FirstOrDefaultAsync();
                        service.ServiceStatus = serviceDraft.PreviousServiceStatus;
                        service.ModifiedTime = DateTime.UtcNow;
                        context.Remove(serviceDraft);
                       
                    }
                    context.Remove(existingDraftProvider);
                }
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }



        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            var providerDraftToken = await context.ProviderDraftToken.FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);
            if (providerDraftToken != null)
            {
                context.ProviderDraftToken.Remove(providerDraftToken);
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
                logger.LogInformation("Remove Provider Draft Token : Token Removed for Provider Id {0}", providerDraftToken.ProviderProfileDraft.ProviderProfileId);
                return true;
            }
            return false;
        }


        #endregion


        #region Service update

        public async Task<ServiceDraftToken> GetServiceDraftToken(string token, string tokenId)
        {


            return await context.ServiceDraftToken.Include(p => p.ServiceDraft).ThenInclude(p => p.Provider).AsNoTracking()
                .Include(p => p.ServiceDraft).ThenInclude(p => p.User).AsNoTracking()
                .Include(p => p.ServiceDraft).ThenInclude(p => p.ManualUnderPinningServiceDraft).AsNoTracking()
              .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceRoleMappingDraft).ThenInclude(s => s.Role).AsNoTracking()
             .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceQualityLevelMappingDraft).ThenInclude(s => s.QualityLevel).AsNoTracking()
             .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceIdentityProfileMappingDraft).ThenInclude(s => s.IdentityProfile).AsNoTracking()
             .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceSupSchemeMappingDraft).ThenInclude(s => s.SupplementaryScheme).AsNoTracking()
             .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceSupSchemeMappingDraft).ThenInclude(s => s.SchemeGPG44MappingDraft).ThenInclude(s => s.QualityLevel).AsNoTracking()
             .Include(p => p.ServiceDraft).ThenInclude(s => s.ServiceSupSchemeMappingDraft).ThenInclude(s => s.SchemeGPG45MappingDraft).ThenInclude(s => s.IdentityProfile).AsNoTracking()

           .FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId) ?? new ServiceDraftToken();
        }

        public async Task<Service> GetPreviousServiceDetails(int serviceId)
        {
            return await context.Service.Include(p => p.Provider).AsNoTracking()
            .Include(s => s.TrustFrameworkVersion)
            .Include(s => s.UnderPinningService)
            .Include(s => s.ManualUnderPinningService)
            .Include(s => s.ServiceRoleMapping).ThenInclude(s => s.Role).AsNoTracking()
            .Include(s => s.ServiceQualityLevelMapping).ThenInclude(s => s.QualityLevel).AsNoTracking()
            .Include(s => s.ServiceIdentityProfileMapping).ThenInclude(s => s.IdentityProfile).AsNoTracking()
            .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SupplementaryScheme).AsNoTracking()
            .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG44Mapping).ThenInclude(s => s.QualityLevel).AsNoTracking()
            .Include(s => s.ServiceSupSchemeMapping).ThenInclude(s => s.SchemeGPG45Mapping).ThenInclude(s => s.IdentityProfile).AsNoTracking()

            .FirstOrDefaultAsync(e => e.Id == serviceId) ?? new Service();
        }

        public async Task<Service> GetService(int serviceId)
        {
            return await context.Service. AsNoTracking().FirstOrDefaultAsync(e => e.Id ==serviceId) ?? new Service();
        }


        public async Task<Service> GetUnderpinningServiceDetails(int serviceId)
        {
            return await context.Service
            .Include(s => s.CabUser).ThenInclude(s => s.Cab).AsNoTracking()
            .Include(s => s.Provider).AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task<ManualUnderPinningService> GetManualUnderPinningServiceDetails(int serviceId)
        {
            return await context.ManualUnderPinningService.Include(m => m.Cab).FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task<ProviderProfile> GetProvider(int providerProfileId)
        {
            return await context.ProviderProfile.AsNoTracking().FirstOrDefaultAsync(e => e.Id == providerProfileId) ?? new ProviderProfile();
        }


        public async Task<GenericResponse> UpdateServiceStatusAndData(int serviceId, int serviceDraftId)
        {
            GenericResponse genericResponse = new();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingService = await context.Service.Include(p=>p.Provider)
                .Include(p => p.TrustFrameworkVersion)
                 .Include(s => s.ServiceRoleMapping)
                 .Include(s => s.ServiceQualityLevelMapping)
                 .Include(s => s.ServiceIdentityProfileMapping)
                 .Include(s=>s.UnderPinningService)
                 .Include(s => s.ManualUnderPinningService)
                 .Include(s => s.ServiceSupSchemeMapping)
                  .Include(s => s.ServiceSupSchemeMapping).ThenInclude(x=>x.SchemeGPG44Mapping)
                  .Include(s => s.ServiceSupSchemeMapping).ThenInclude(x => x.SchemeGPG45Mapping)
                 .FirstOrDefaultAsync(p => p.Id == serviceId);



                var existingDraftService= await context.ServiceDraft.Include(p => p.Provider)
                 .Include(s => s.ServiceRoleMappingDraft)
                 .Include(s => s.ServiceQualityLevelMappingDraft)
                 .Include(s => s.ServiceIdentityProfileMappingDraft)
                 .Include(s => s.ServiceSupSchemeMappingDraft)
                 .Include(s => s.ManualUnderPinningServiceDraft)
                 .Include(s => s.ServiceSupSchemeMappingDraft).ThenInclude(x=>x.SchemeGPG44MappingDraft)
                 .Include(s => s.ServiceSupSchemeMappingDraft).ThenInclude(x => x.SchemeGPG45MappingDraft)
                 .Include(s=>s.ManualUnderPinningServiceDraft)
                 .FirstOrDefaultAsync(p => p.Id == serviceDraftId);


                if (existingService != null && existingService.ServiceStatus == ServiceStatusEnum.UpdatesRequested && existingDraftService != null
                    && existingDraftService.ServiceId == existingService.Id)
                {

                    ICollection<ServiceRoleMapping> newServiceRoleMapping = [];
                    ICollection<ServiceQualityLevelMapping> newServiceQualityLevelMapping = [];
                    ICollection<ServiceIdentityProfileMapping> newServiceIdentityProfileMapping = [];
                    ICollection<ServiceSupSchemeMapping> newServiceSupSchemeMapping = [];
                  

                    existingService.ModifiedTime = DateTime.UtcNow;
                    existingService.ServiceStatus = ServiceStatusEnum.ReadyToPublish;
                    if (existingService.Provider.ProviderStatus == ProviderStatusEnum.Published)
                        existingService.Provider.ProviderStatus = ProviderStatusEnum.ReadyToPublish;
                    existingService.Provider.ModifiedTime = DateTime.UtcNow;
                    existingService.EditServiceTokenStatus = TokenStatusEnum.RequestCompleted;
                    existingService.IsInRegister = false;

                    existingService.ServiceName = existingDraftService.ServiceName ?? existingService.ServiceName;
                    existingService.CompanyAddress = existingDraftService.CompanyAddress ?? existingService.CompanyAddress;
                    existingService.HasGPG44 = existingDraftService.HasGPG44 ?? existingService.HasGPG44;
                    existingService.HasGPG45 = existingDraftService.HasGPG45 ?? existingService.HasGPG45;
                    existingService.HasSupplementarySchemes = existingDraftService.HasSupplementarySchemes ?? existingService.HasSupplementarySchemes;
                    existingService.ConformityIssueDate = existingDraftService.ConformityIssueDate ?? existingService.ConformityIssueDate;
                    existingService.ConformityExpiryDate = existingDraftService.ConformityExpiryDate ?? existingService.ConformityExpiryDate;


                    //--------------Role------------------------------//
                    if (existingDraftService.ServiceRoleMappingDraft?.Count > 0)
                    {
                        foreach(var item in existingDraftService.ServiceRoleMappingDraft)
                        {
                            newServiceRoleMapping.Add(new ServiceRoleMapping { RoleId = item.RoleId , ServiceId = existingService.Id});
                        }

                        context.ServiceRoleMapping.RemoveRange(existingService.ServiceRoleMapping);
                        existingService.ServiceRoleMapping = newServiceRoleMapping;

                    }
                    //------------------------Quality------------------------------------------//
                    if (existingDraftService.ServiceQualityLevelMappingDraft?.Count > 0)
                    {
                        foreach (var item in existingDraftService.ServiceQualityLevelMappingDraft)
                        {
                            newServiceQualityLevelMapping.Add(new ServiceQualityLevelMapping { QualityLevelId = item.QualityLevelId, ServiceId = existingService.Id });
                        }
                        context.ServiceQualityLevelMapping.RemoveRange(existingService.ServiceQualityLevelMapping);
                        existingService.ServiceQualityLevelMapping = newServiceQualityLevelMapping;
                        existingService.HasGPG44 = true;
                    }
                    else if(existingDraftService.HasGPG44 != null && existingDraftService.HasGPG44 == false) 
                    {                     
                        context.ServiceQualityLevelMapping.RemoveRange(existingService.ServiceQualityLevelMapping);
                        existingService.HasGPG44 = false;
                    }

                    //--------------Identity profiles---------------------------------


                    if (existingDraftService.ServiceIdentityProfileMappingDraft?.Count > 0)
                    {
                        foreach (var item in existingDraftService.ServiceIdentityProfileMappingDraft)
                        {
                            newServiceIdentityProfileMapping.Add(new ServiceIdentityProfileMapping { IdentityProfileId = item.IdentityProfileId, ServiceId = existingService.Id });
                        }

                        context.ServiceIdentityProfileMapping.RemoveRange(existingService.ServiceIdentityProfileMapping);
                        existingService.ServiceIdentityProfileMapping = newServiceIdentityProfileMapping;
                        existingService.HasGPG45 = true;

                    }
                    else if (existingDraftService.HasGPG45 != null && existingDraftService.HasGPG45 == false)
                    {
                        context.ServiceIdentityProfileMapping.RemoveRange(existingService.ServiceIdentityProfileMapping);
                        existingService.HasGPG45 = false;
                    }
                    //--------------------------Scheme-------------------------------------------//


                        if (existingDraftService.ServiceSupSchemeMappingDraft?.Count > 0)
                        {                         

                            foreach (var item in existingDraftService.ServiceSupSchemeMappingDraft)
                            {
                                var existingSchemeMapping = existingService?.ServiceSupSchemeMapping?.Where(x => x.SupplementarySchemeId == item.SupplementarySchemeId).FirstOrDefault();

                                newServiceSupSchemeMapping.Add(new ServiceSupSchemeMapping { SupplementarySchemeId = item.SupplementarySchemeId, ServiceId = existingService.Id });

                            if (existingService.TrustFrameworkVersion.Version == Constants.TFVersion0_4)
                            {
                                MapTFVersion0_4Methods(existingService, newServiceSupSchemeMapping, item);
                            }
                                

                            }
                            context.ServiceSupSchemeMapping.RemoveRange(existingService.ServiceSupSchemeMapping);
                            existingService.ServiceSupSchemeMapping = newServiceSupSchemeMapping;
                            existingService.HasSupplementarySchemes = true;

                        }

                        else if (existingDraftService.HasSupplementarySchemes != null && existingDraftService.HasSupplementarySchemes == false)
                        {
                            context.ServiceSupSchemeMapping.RemoveRange(existingService.ServiceSupSchemeMapping);
                            existingService.HasSupplementarySchemes = false;
                        }
                  
                                         




                    //-----------underpinning service-----------------------------------------------------------------------//

                    if (existingService.TrustFrameworkVersion.Version == Constants.TFVersion0_4 &&  existingDraftService.UnderPinninngServiceEditType != null)
                    {
                        if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.PublishedToPublished)
                        {
                            existingService.UnderPinningServiceId = existingDraftService.UnderPinningServiceId;
                        }
                        else if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.PublishedToSelectOrChangeManual)
                        {
                            existingService.UnderPinningServiceId = null;
                            existingService.IsUnderPinningServicePublished = false;
                            existingService.ManualUnderPinningServiceId = existingDraftService.ManualUnderPinningServiceId;


                            if (existingDraftService.ManualUnderPinningServiceDraft != null)
                            {
                                await UpdateManualUnderpinningService(existingDraftService);
                            }
                        }
                        else if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.PublishedToEnterManual)
                        {
                            existingService.UnderPinningServiceId = null;
                            existingService.IsUnderPinningServicePublished = false;
                            CreateAndAssignManualUnderPinningServiceInstance(existingService, existingDraftService);
                        }
                        else if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.ManualToPublished)
                        {
                            existingService.ManualUnderPinningServiceId = null;
                            existingService.IsUnderPinningServicePublished = true;
                            existingService.UnderPinningServiceId = existingDraftService.UnderPinningServiceId;
                        }
                        else if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.ManualToSelectOrChangeManual)
                        {
                            existingService.ManualUnderPinningServiceId = existingDraftService.ManualUnderPinningServiceId;
                            existingService.IsUnderPinningServicePublished = false;

                            await UpdateManualUnderpinningService(existingDraftService);

                        }
                        else if (existingDraftService.UnderPinninngServiceEditType == UnderPinninngServiceEditEnum.ManualToEnterManual)
                        {
                            CreateAndAssignManualUnderPinningServiceInstance(existingService, existingDraftService);
                        }
                    }

                    //----------------------------------------------------------------------------------//
                    context.Remove(existingDraftService);

                    await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ServiceEdit2i, "DSIT");
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
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }

        private static void MapTFVersion0_4Methods(Service? existingService, ICollection<ServiceSupSchemeMapping> newServiceSupSchemeMapping, ServiceSupSchemeMappingDraft item)
        {
            var newMappingInstance = newServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == item.SupplementarySchemeId).FirstOrDefault();

            newMappingInstance.SchemeGPG45Mapping = [];
            newMappingInstance.SchemeGPG44Mapping = [];
            if (item.SchemeGPG45MappingDraft != null && item.SchemeGPG45MappingDraft.Count > 0)
            {
                foreach (var identityProfileMapping in item.SchemeGPG45MappingDraft)
                {
                    newMappingInstance.SchemeGPG45Mapping.Add(new SchemeGPG45Mapping { IdentityProfileId = identityProfileMapping.IdentityProfileId });
                }

            }
            else
            {
                var existingSchemeIdentityProfileMapping = existingService.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == item.SupplementarySchemeId)
                .SelectMany(x => x.SchemeGPG45Mapping).ToList();
                foreach (var schemeProfile in existingSchemeIdentityProfileMapping)
                {
                    newMappingInstance.SchemeGPG45Mapping.Add(new SchemeGPG45Mapping { IdentityProfileId = schemeProfile.IdentityProfileId });
                }


            }

            if (item.SchemeGPG44MappingDraft != null && item.SchemeGPG44MappingDraft.Count > 0 && item.HasGpg44Mapping ==true)
            {
                newMappingInstance.HasGpg44Mapping = true;
                foreach (var qualityLevelMapping in item.SchemeGPG44MappingDraft)
                {
                    newMappingInstance.SchemeGPG44Mapping.Add(new SchemeGPG44Mapping { QualityLevelId = qualityLevelMapping.QualityLevelId });
                }

            }
            else if (item.HasGpg44Mapping != null && item.HasGpg44Mapping == false)
            {
                newMappingInstance.HasGpg44Mapping = false;
                newMappingInstance.SchemeGPG44Mapping = [];
            }
            else
            {
                var existingSchemMapping = existingService.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == item.SupplementarySchemeId).FirstOrDefault();
                var existingQualityLevelMapping = existingService.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == item.SupplementarySchemeId)
                .SelectMany(x => x.SchemeGPG44Mapping).ToList();

                newMappingInstance.HasGpg44Mapping = existingSchemMapping.HasGpg44Mapping;
                foreach (var schemeQuality in existingQualityLevelMapping)
                {
                    newMappingInstance.SchemeGPG44Mapping.Add(new SchemeGPG44Mapping { QualityLevelId = schemeQuality.QualityLevelId });

                }


            }
        }

        private async Task UpdateManualUnderpinningService(ServiceDraft? existingDraftService)
        {
            if (existingDraftService.ManualUnderPinningServiceDraft != null)
            {
                var manualUnderPinningService = await context.ManualUnderPinningService.FirstOrDefaultAsync(x => x.Id == existingDraftService.ManualUnderPinningServiceId);

                if (manualUnderPinningService.ServiceName != existingDraftService.ManualUnderPinningServiceDraft.ServiceName)
                    manualUnderPinningService.ServiceName = existingDraftService.ManualUnderPinningServiceDraft.ServiceName;

                if (manualUnderPinningService.ProviderName != existingDraftService.ManualUnderPinningServiceDraft.ProviderName)
                    manualUnderPinningService.ProviderName = existingDraftService.ManualUnderPinningServiceDraft.ProviderName;

                if (manualUnderPinningService.CabId != existingDraftService.ManualUnderPinningServiceDraft.CabId)
                    manualUnderPinningService.CabId = existingDraftService.ManualUnderPinningServiceDraft.CabId;

                if (manualUnderPinningService.CertificateExpiryDate != existingDraftService.ManualUnderPinningServiceDraft.CertificateExpiryDate)
                    manualUnderPinningService.CertificateExpiryDate = existingDraftService.ManualUnderPinningServiceDraft.CertificateExpiryDate;

                context.Remove(existingDraftService.ManualUnderPinningServiceDraft);
            }
        }

        private void CreateAndAssignManualUnderPinningServiceInstance(Service? existingService, ServiceDraft? existingDraftService)
        {
            existingService.ManualUnderPinningService = new();
            existingService.ManualUnderPinningService.ServiceName = existingDraftService.ManualUnderPinningServiceDraft.ServiceName;
            existingService.ManualUnderPinningService.ProviderName = existingDraftService.ManualUnderPinningServiceDraft.ProviderName;
            existingService.ManualUnderPinningService.CabId = existingDraftService.ManualUnderPinningServiceDraft.CabId;
            existingService.ManualUnderPinningService.CertificateExpiryDate = existingDraftService.ManualUnderPinningServiceDraft.CertificateExpiryDate;
            context.Remove(existingDraftService.ManualUnderPinningServiceDraft);
        }

        public async Task<bool> RemoveServiceDraftToken(string token, string tokenId)
        {
            var serviceDraftToken = await context.ServiceDraftToken.FirstOrDefaultAsync(e => e.Token == token && e.TokenId == tokenId);
            if (serviceDraftToken != null)
            {
                context.ServiceDraftToken.Remove(serviceDraftToken);
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ServiceEdit2i, "DSIT");
                logger.LogInformation("Remove Service Draft Token : Token Removed for Service Id {0}", serviceDraftToken.ServiceDraft.ServiceId);
                return true;
            }
            return false;
        }


        public async Task<GenericResponse> CancelServiceUpdates(int serviceId, int serviceDraftId)
        {
            GenericResponse genericResponse = new();
            var existingService = await context.Service.FirstOrDefaultAsync(p => p.Id == serviceId && p.IsCurrent == true);
            var existingDraftService = await context.ServiceDraft.Include(P => P.ServiceRoleMappingDraft).Include(P => P.ServiceQualityLevelMappingDraft).Include(p => p.ServiceIdentityProfileMappingDraft)
            .Include(p => p.ServiceSupSchemeMappingDraft).FirstOrDefaultAsync(p => p.Id == serviceDraftId);


            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (existingService != null && existingDraftService != null)// assign back the previous status
                {
                    existingService.EditServiceTokenStatus = TokenStatusEnum.UserCancelled;
                    existingService.ServiceStatus = existingDraftService.PreviousServiceStatus;
                    existingService.ModifiedTime = DateTime.UtcNow;                    
                    context.Remove(existingDraftService);
                }
                await context.SaveChangesAsync(TeamEnum.DSIT, EventTypeEnum.ProviderEdit2i, "DSIT");
                await transaction.CommitAsync();
                genericResponse.Success = true;
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                await transaction.RollbackAsync();
                logger.LogError(ex.Message);
            }
            return genericResponse;
        }

        #endregion
    }
}
