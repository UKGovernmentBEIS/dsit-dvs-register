using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVSRegister.Data.CAB
{
    public class CabRepository : ICabRepository
    {
        private readonly DVSRegisterDbContext context;
        private readonly ILogger<CabRepository> logger;

        public CabRepository(DVSRegisterDbContext context, ILogger<CabRepository> logger)
        {
            this.context = context;
            this.logger=logger;
        }
    

        public async Task<List<Role>> GetRoles(decimal tfVersion)
        {
            return await context.Role.Include(x=>x.TrustFrameworkVersion).Where(x=>x.TrustFrameworkVersion.Version <= tfVersion).OrderBy(c => c.Order).ToListAsync();
        }

        public async Task<List<IdentityProfile>> GetIdentityProfiles()
        {
            return await context.IdentityProfile.OrderBy(c => c.IdentityProfileName).ToListAsync();
        }

        public async Task<List<SupplementaryScheme>> GetSupplementarySchemes()
        {
            return await context.SupplementaryScheme.OrderBy(c => c.Order).ToListAsync();
        }
        public async Task<List<TrustFrameworkVersion>> GetTfVersion()
        {
            return await context.TrustFrameworkVersion.OrderBy(c => c.Order).ToListAsync();
        }
        public async Task<List<QualityLevel>> QualityLevels()
        {
            return await context.QualityLevel.ToListAsync();
        }  
       


        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName)
        {
            var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.RegisteredName.ToLower() == registeredName.ToLower());

            if (existingProvider !=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId)
        {
            var existingProvider = await context.ProviderProfile.FirstOrDefaultAsync(p => p.RegisteredName.ToLower() == registeredName.ToLower() && p.Id != providerId);

            if (existingProvider !=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<ProviderProfile>> GetProviders(int cabId, string searchText = "")
        {
            //Filter based on cab type as well, fetch records for users with same cab type
            IQueryable<ProviderProfile> providerQuery = context.ProviderProfile.Include(p => p.Services!.Where(s=>s.CabUser.CabId == cabId)).ThenInclude(s=>s.CabUser)
             .Include(p => p.ProviderProfileCabMapping).ThenInclude(p => p.Cab)
            .Where(p => p.ProviderProfileCabMapping.Any(m => m.CabId == cabId))
            .OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime);
            if (!string.IsNullOrEmpty(searchText))
            {
                var lowerSearchText = searchText.Trim().ToLower();
                providerQuery = providerQuery.Where(p =>
                    // Search in provider names with partial matching
                    p.RegisteredName.ToLower().Contains(lowerSearchText) ||
                    p.TradingName!.ToLower().Contains(lowerSearchText) ||
                    // Search in service names
                    p.Services!.Any(s => s.CabUser.CabId == cabId && s.ServiceName!.ToLower().Contains(lowerSearchText))
                );
            }
            var searchResults = await providerQuery.ToListAsync();
            return searchResults;
        }        

        public async Task<ProviderProfile> GetProvider(int providerId,int cabId)
        {
                ProviderProfile provider = new();
                provider = await context.ProviderProfile.Include(p=>p.Services)!.ThenInclude(p=>p.CertificateReview)
                .Include(p => p.Services)!.ThenInclude(p => p.PublicInterestCheck)
                .Include(p => p.Services)!.ThenInclude(p => p.ServiceDraft)
                .Include(p => p.Services!.Where(s=>s.CabUser.CabId == cabId)).ThenInclude(p => p.CabUser)
                .Include(p => p.Services!.Where(s => s.CabUser.CabId == cabId)).ThenInclude(p => p.CabTransferRequest)!.ThenInclude(p => p.RequestManagement)
                .Include(p => p.ProviderProfileCabMapping).ThenInclude(cu => cu.Cab)
                .Where(p => p.Id == providerId && p.ProviderProfileCabMapping.Any(m => m.CabId == cabId)).OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime).FirstOrDefaultAsync() ?? new ProviderProfile();
                return provider;
        }

            public async Task<Service> GetServiceDetailsWithProvider(int serviceId, int cabId)
            {

            Service service = new();
            service = await context.Service.AsNoTracking()
            .Include(p => p.CabUser).ThenInclude(p => p.Cab)
            .Include(p => p.Provider)
            .Include(p => p.CertificateReview)
            .Include(p => p.PublicInterestCheck)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId).FirstOrDefaultAsync() ?? new Service();
            return service;

            }

        public async Task<Service> GetServiceDetails(int serviceId, int cabId)
        {

            var baseQuery = context.Service.AsNoTracking().Include(p => p.CabUser).ThenInclude(cu => cu.Cab)
            .Where(p => p.Id == serviceId && p.CabUser.CabId == cabId)
            .Include(p => p.Provider).ThenInclude(p => p.ProviderProfileDraft).AsNoTracking()
            .Include(p => p.ServiceDraft).AsNoTracking()
            .Include(p => p.TrustFrameworkVersion).AsNoTracking()
            .Include(p => p.CertificateReview).AsNoTracking()
            .Include(p => p.PublicInterestCheck).AsNoTracking()
            .Include(p => p.CabTransferRequest)!.ThenInclude(x => x.RequestManagement).AsNoTracking()

            .Include(p => p.UnderPinningService).ThenInclude(p=>p.Provider).AsNoTracking()
            .Include(p => p.UnderPinningService).ThenInclude(p => p.CabUser).ThenInclude(cu => cu.Cab).AsNoTracking()
            .Include(p => p.ManualUnderPinningService) .ThenInclude(ms => ms.Cab).AsNoTracking()
            .Include(p => p.ServiceRoleMapping)!.ThenInclude(s => s.Role).AsNoTracking();


            IQueryable<Service> queryWithOptionalIncludes = baseQuery;
            if (await baseQuery.AnyAsync(p => p.ServiceQualityLevelMapping != null && p.ServiceQualityLevelMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceQualityLevelMapping)!
                    .ThenInclude(sq => sq.QualityLevel);
            }

            if (await baseQuery.AnyAsync(p => p.ServiceSupSchemeMapping != null && p.ServiceSupSchemeMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)!
                    .ThenInclude(ssm => ssm.SupplementaryScheme);

                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)!
                    .ThenInclude(ssm => ssm.SchemeGPG44Mapping)!.ThenInclude(ssm=>ssm.QualityLevel);
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceSupSchemeMapping)!
                    .ThenInclude(ssm => ssm.SchemeGPG45Mapping)!.ThenInclude(ssm => ssm.IdentityProfile);
            }

            if (await baseQuery.AnyAsync(p => p.ServiceIdentityProfileMapping != null && p.ServiceIdentityProfileMapping.Any()))
            {
                queryWithOptionalIncludes = queryWithOptionalIncludes.Include(p => p.ServiceIdentityProfileMapping)!
                    .ThenInclude(ssm => ssm.IdentityProfile);
            }
            var service = await queryWithOptionalIncludes.FirstOrDefaultAsync() ?? new Service();


            return service;
        }
        public async Task<List<Service>> GetServiceList(int serviceKey, int cabId)
            {
            return await context.Service
            .Include(s => s.Provider).ThenInclude(p=>p.ProviderProfileDraft).AsNoTracking()
            .Include(s => s.CertificateReview).AsNoTracking()
            .Include(s => s.PublicInterestCheck).AsNoTracking()
            .Include(s => s.CabUser).ThenInclude(s => s.Cab).AsNoTracking()
            .Include(s => s.TrustFrameworkVersion).AsNoTracking()
            .Include(s => s.ServiceDraft).AsNoTracking()
            .Where(s => s.ServiceKey == serviceKey && s.CabUser.Cab.Id == cabId)
            .ToListAsync();
        }
        public async Task<bool> IsManualServiceLinkedToMultipleServices(int manualServiceId)
        {
            return await context.Service.Where(s => s.ManualUnderPinningServiceId == manualServiceId).CountAsync() > 1;
        }

        public async Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId)
        {
            ProviderProfile provider = new();
            provider = await context.ProviderProfile.Include(p=>p.Services). Include(p => p.ProviderProfileCabMapping).ThenInclude(p=>p.Cab).Where(x=>x.Id == providerId).FirstOrDefaultAsync()??new ProviderProfile();
            if(provider.ProviderProfileCabMapping.Any(m => m.CabId == cabId))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<(int,List<CabTransferRequest>)> GetPendingReassignRequestsCount(int cabId)
        {

            var pendingRequests = await context.RequestManagement.Where(r => r.CabId == cabId && r.RequestStatus == RequestStatusEnum.Pending && r.RequestType== RequestTypeEnum.CabTransfer).ToListAsync();

            var pendingUploads= await context.CabTransferRequest.Include(c=>c.Service).ThenInclude(c=>c.Provider)
                .Where(c => c.ToCabId == cabId && c.CertificateUploaded == false && c.RequestManagement != null && c.RequestManagement.RequestStatus == RequestStatusEnum.Approved).ToListAsync();

            return (pendingRequests.Count, pendingUploads);           

        }

        public async Task<List<string>> GetCabEmailListForServices(List<int> serviceIds)
        {
            List<int> cabIds = await context.Service.Include(p => p.CabUser).Where(x => serviceIds.Contains(x.Id)).Select(x => x.CabUser.CabId).Distinct().ToListAsync();
            List<string> activeCabUserEmails = await context.CabUser.Where(c => cabIds.Contains(c.CabId) && c.IsActive).Select(c => c.CabEmail).ToListAsync();
            return activeCabUserEmails;
        }

        public async Task<ProviderProfile> GetProviderWithLatestVersionServices(int providerId, int cabId)
        {
            var latestServiceIds = await context.Service
                .Include(s =>  s.CabTransferRequest)
                .Where(s => s.ProviderProfileId == providerId
                && s.CabUser.CabId == cabId
                && !(s.ServiceStatus == ServiceStatusEnum.SavedAsDraft
                || s.ServiceStatus == ServiceStatusEnum.AmendmentsRequired))
                .GroupBy(s => s.ServiceKey)
                .Select(g => g.OrderByDescending(s => s.ServiceVersion).FirstOrDefault()!.Id)
                .ToListAsync();

            ProviderProfile provider = new();
            provider = await context.ProviderProfile
                .Include(p => p.Services!.Where(s => s.CabUser.CabId == cabId && latestServiceIds.Contains(s.Id))).ThenInclude(s => s.CertificateReview)
                .Include(p => p.Services!.Where(s => s.CabUser.CabId == cabId && latestServiceIds.Contains(s.Id))).ThenInclude(s => s.PublicInterestCheck)                
                .Include(p => p.Services!.Where(s => s.CabUser.CabId == cabId && latestServiceIds.Contains(s.Id))).ThenInclude(p => p.ServiceDraft)
                .Include(p => p.Services!.Where(s => s.CabUser.CabId == cabId && latestServiceIds.Contains(s.Id))).ThenInclude(s => s.CabUser)
                .Include(p => p.ProviderProfileDraft)!
                .Include(p => p.ProviderProfileCabMapping).ThenInclude(cu => cu.Cab)
                .Where(p => p.Id == providerId && p.ProviderProfileCabMapping.Any(m => m.CabId == cabId)).OrderBy(p => p.ModifiedTime != null ? p.ModifiedTime : p.CreatedTime).FirstOrDefaultAsync() ?? new ProviderProfile();

            return provider;
        }

        #region Save/update
        public async Task<GenericResponse> SaveProviderProfile(ProviderProfile providerProfile, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {

                var existingProvider = await context.ProviderProfile.Where(p => p.Id == providerProfile.Id).FirstOrDefaultAsync();
                if (existingProvider != null)
                {
                    UpdateExistingProfileRecord(existingProvider, providerProfile);
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddProvider, loggedInUserEmail);
                    genericResponse.InstanceId = existingProvider.Id;
                }
                else
                {
                    providerProfile.CreatedTime = DateTime.UtcNow;
                    var entity = await context.ProviderProfile.AddAsync(providerProfile);
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddProvider, loggedInUserEmail);
                    genericResponse.InstanceId = entity.Entity.Id;
                }

                transaction.Commit();
                genericResponse.Success = true;

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveProviderProfile");
            }
            return genericResponse;
        }
        public async Task<GenericResponse> SaveService(Service service, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                
                var existingService = await context.Service.Include(x=>x.ServiceRoleMapping).Include(x=>x.ServiceIdentityProfileMapping)
                .Include(x=>x.ServiceQualityLevelMapping).Include(x=>x.ServiceSupSchemeMapping)
                .Include(x => x.ManualUnderPinningService)
                 .Where(x=> x.ServiceKey > 0 && x.ServiceKey == service.ServiceKey && service.IsCurrent == true).FirstOrDefaultAsync();                
                if(existingService != null && existingService.Id>0)
                {
                    // save as draft : update existing records
                    UpdateExistingServiceRecord(service, existingService);
                    genericResponse.InstanceId = existingService.Id;
                    if (service.ServiceStatus == ServiceStatusEnum.SavedAsDraft)
                        await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.SaveAsDraftService, loggedInUserEmail);
                    else
                        await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddService, loggedInUserEmail);
                   
                }
                else
                {
                    //insert as new service
                    service.CreatedTime = DateTime.UtcNow;                 
                    service.ConformityExpiryDate = service.ConformityExpiryDate == DateTime.MinValue ? null : service.ConformityExpiryDate;
                    service.ConformityIssueDate = service.ConformityIssueDate == DateTime.MinValue ? null : service.ConformityIssueDate;
                    AttachListToDbContext(service);
                    var entity = await context.Service.AddAsync(service);

                   
                    if(service.ServiceSupSchemeMapping !=null)
                    {
                        foreach (var mapping in service.ServiceSupSchemeMapping)
                        {
                            if(mapping.SchemeGPG44Mapping!=null)
                            {
                                foreach (var gpg44 in mapping.SchemeGPG44Mapping)
                                {
                                    await context.SchemeGPG44Mapping.AddAsync(gpg44);
                                }
                            }

                            if (mapping.SchemeGPG45Mapping != null)
                            {
                                foreach (var gpg45 in mapping.SchemeGPG45Mapping)
                                {
                                    await context.SchemeGPG45Mapping.AddAsync(gpg45);
                                }
                            }

                        }
                    }

                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.AddService, loggedInUserEmail);
                    genericResponse.InstanceId = entity.Entity.Id;
                    service.ServiceKey = entity.Entity.Id; // for new service addition , assign service key same as that of primary key
                    await context.SaveChangesAsync();
                  
                }
                transaction.Commit();
                genericResponse.Success = true;


            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveService");
            }
            return genericResponse;
        }



        public async Task<GenericResponse> SaveServiceReApplication(Service service, string loggedInUserEmail, bool isReupload, InProgressApplicationParameters? inProgressApplicationParameters)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();


            try
            {
                var existingService = await context.Service.Include(x => x.ServiceRoleMapping).Include(x => x.ServiceIdentityProfileMapping)!
                .Include(x => x.ServiceQualityLevelMapping).Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG44Mapping)
                .Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG45Mapping)
                .Include(x => x.ManualUnderPinningService)
                .Include(s => s.PublicInterestCheck)!
                .Include(s => s.CertificateReview)!
                .Include(s => s.ServiceDraft)!
                .Include(s => s.ServiceRemovalRequest)!
                .Include(s => s.CabTransferRequest)!.ThenInclude(tr => tr.RequestManagement)
                .Include(s => s.ActionLogs)
                 .Include(s => s.ProceedApplicationConsentToken)
                .Where(x => x.ServiceKey == service.ServiceKey && x.IsCurrent == true).FirstOrDefaultAsync();
                if (existingService != null && existingService.Id > 0 && existingService.ServiceKey > 0)
                {

                    if (existingService.ServiceStatus == ServiceStatusEnum.SavedAsDraft) //Resuming save
                    {
                        UpdateExistingServiceRecord(service, existingService);
                        if(inProgressApplicationParameters!=null)
                        await RemoveInprogressApplications(loggedInUserEmail, inProgressApplicationParameters, existingService, service);
                        genericResponse.InstanceId = existingService.Id;
                        await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.SaveAsDraftService, loggedInUserEmail);
                    }
                    else // First time save
                    {

                        if (!isReupload && existingService.ServiceStatus != ServiceStatusEnum.Published && existingService.ServiceStatus != ServiceStatusEnum.Removed && service.ServiceStatus != ServiceStatusEnum.SavedAsDraft

                            && inProgressApplicationParameters != null)
                        {
                            await RemoveInprogressApplications(loggedInUserEmail, inProgressApplicationParameters, existingService);

                        }
                        else if (isReupload)
                        {
                            var cabTransferRequest = context.CabTransferRequest.Include(c => c.RequestManagement).Where(c => c.ServiceId == existingService.Id).OrderByDescending(c => c.Id).FirstOrDefault();

                            //if it is a transfered service, update certificate upload flag, update provider status
                            if (cabTransferRequest != null && cabTransferRequest.RequestManagement != null
                              && cabTransferRequest.RequestManagement.RequestType == RequestTypeEnum.CabTransfer
                              && cabTransferRequest.RequestManagement.RequestStatus == RequestStatusEnum.Approved
                              && cabTransferRequest.CertificateUploaded == false)
                            {
                                cabTransferRequest.CertificateUploaded = true;
                                if (existingService.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign || existingService.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign)
                                    existingService.ServiceStatus = cabTransferRequest.PreviousServiceStatus;

                                var existingProvider = await context.ProviderProfile
                                    .Include(service => service.Services)
                                    .FirstOrDefaultAsync(p => p.Id == service.ProviderProfileId);

                                if (existingProvider != null)
                                {
                                    existingProvider.ModifiedTime = DateTime.UtcNow;
                                }
                            }
                        }


                        //reapplication - insert new version of service


                        var serviceList = await context.Service.Where(x => x.ServiceKey == service.ServiceKey).ToListAsync();
                        int maxServiceVersion = serviceList.Any() ? serviceList.Max(s => s.ServiceVersion) : 0;
                        service.Id = 0; // to insert as new record
                        service.ServiceVersion = maxServiceVersion + 1;
                        service.CreatedTime = DateTime.UtcNow;
                        service.ModifiedTime = DateTime.UtcNow;
                        service.ConformityExpiryDate = service.ConformityExpiryDate == DateTime.MinValue ? null : service.ConformityExpiryDate;
                        service.ConformityIssueDate = service.ConformityIssueDate == DateTime.MinValue ? null : service.ConformityIssueDate;
                        var entity = await context.Service.AddAsync(service);


                        if (inProgressApplicationParameters?.HasInProgressApplication != true || service.ServiceStatus == ServiceStatusEnum.SavedAsDraft )
                        {
                            existingService.IsCurrent = false;
                            existingService.ModifiedTime = DateTime.UtcNow;
                           
                        }
                    
                        await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.ReapplyService, loggedInUserEmail);
                        genericResponse.InstanceId = entity.Entity.Id;
                    }
                    transaction.Commit();
                    genericResponse.Success = true;
                }
                else
                {
                    transaction.Rollback();
                    genericResponse.Success = false;
                    logger.LogError("Service id {ServiceKey} doesnt exist ", service.ServiceKey);
                }
            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveServiceReApplication");
            }
            return genericResponse;
        }

     

        public async Task<GenericResponse> SaveServiceAmendments(Service service, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var existingService = await context.Service.Include(x => x.CertificateReview).Include(x => x.ServiceRoleMapping).Include(x => x.ServiceIdentityProfileMapping)
               .Include(x => x.ServiceQualityLevelMapping).Include(x => x.ServiceSupSchemeMapping)
               .Include(x => x.ManualUnderPinningService)
               .Where(x => x.Id == service.Id && x.IsCurrent == true).FirstOrDefaultAsync();
                if (existingService != null && existingService.Id > 0 && existingService.ServiceKey > 0)
                {
                    if (existingService.ServiceStatus == ServiceStatusEnum.AmendmentsRequired)
                    {                       
                        existingService.ResubmissionTime = DateTime.UtcNow;
                        UpdateExistingServiceRecord(service, existingService);
                       // context.CertificateReview.Remove(existingService.CertificateReview);
                        genericResponse.InstanceId = existingService.ServiceKey;
                        await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.ServiceAmendments, loggedInUserEmail);
                        genericResponse.InstanceId = existingService.Id;
                        transaction.Commit();
                        genericResponse.Success = true;
                    }
                    else
                    {
                        transaction.Rollback();
                        genericResponse.Success = false;
                        logger.LogError( "Service id {ServiceID} doesnt exist ", service.Id);
                    }
                }            

            }
            catch (Exception ex)
            {
                genericResponse.Success = false;
                transaction.Rollback();
                logger.LogError(ex, "Error in SaveServiceReApplication");
            }
            return genericResponse;
        }






        #endregion


        #region private methods

        private async Task RemoveInprogressApplications(string loggedInUserEmail, InProgressApplicationParameters? inProgressApplicationParameters, Service? existingService, Service? newService = null)
        {
            if (inProgressApplicationParameters == null) return;

            if ( (inProgressApplicationParameters.HasInProgressApplication && inProgressApplicationParameters.InProgressApplicationId == existingService?.Id)
                || (inProgressApplicationParameters.InProgressAndUpdateRequested && inProgressApplicationParameters.InProgressAndUpdateRequestedId == existingService?.Id))
            {
                context.Remove(existingService);
                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveInProgressApplication, loggedInUserEmail);
            }
           

            else if( inProgressApplicationParameters.HasInProgressApplication && inProgressApplicationParameters.InProgressApplicationId != existingService?.Id               
                && newService?.ServiceStatus != ServiceStatusEnum.SavedAsDraft && newService != null)
            {
                var inprogressServiceBeforeDraft = await context.Service.Include(x => x.ServiceRoleMapping).Include(x => x.ServiceIdentityProfileMapping)!
                .Include(x => x.ServiceQualityLevelMapping).Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG44Mapping)
                .Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG45Mapping)
                .Include(x => x.ManualUnderPinningService)
                .Include(s => s.PublicInterestCheck)!
                .Include(s => s.CertificateReview)!
                .Include(s => s.ServiceDraft)!
                .Include(s => s.ServiceRemovalRequest)!
                .Include(s => s.CabTransferRequest)!.ThenInclude(tr => tr.RequestManagement)
                .Include(s => s.ActionLogs)
                 .Include(s => s.ProceedApplicationConsentToken)
                .Where(x => x.Id == inProgressApplicationParameters.InProgressApplicationId).FirstOrDefaultAsync();

                if(inprogressServiceBeforeDraft!=null)
                {
                    context.Remove(inprogressServiceBeforeDraft);
                    existingService.ServiceVersion = existingService.ServiceVersion - 1; // as previous service is removed, decrease version by 1
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveInProgressApplication, loggedInUserEmail);
                }
              
            }

            else if (inProgressApplicationParameters.InProgressAndUpdateRequested && inProgressApplicationParameters.InProgressAndUpdateRequestedId != existingService?.Id
               && newService?.ServiceStatus != ServiceStatusEnum.SavedAsDraft && newService != null)
            {
                var inprogressServiceBeforeDraft = await context.Service.Include(x => x.ServiceRoleMapping).Include(x => x.ServiceIdentityProfileMapping)!
                .Include(x => x.ServiceQualityLevelMapping).Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG44Mapping)
                .Include(x => x.ServiceSupSchemeMapping)!.ThenInclude(x => x.SchemeGPG45Mapping)
                .Include(x => x.ManualUnderPinningService)
                .Include(s => s.PublicInterestCheck)!
                .Include(s => s.CertificateReview)!
                .Include(s => s.ServiceDraft)!
                .Include(s => s.ServiceRemovalRequest)!
                .Include(s => s.CabTransferRequest)!.ThenInclude(tr => tr.RequestManagement)
                .Include(s => s.ActionLogs)
                 .Include(s => s.ProceedApplicationConsentToken)
                .Where(x => x.Id == inProgressApplicationParameters.InProgressAndUpdateRequestedId).FirstOrDefaultAsync();

                if (inprogressServiceBeforeDraft != null)
                {
                    context.Remove(inprogressServiceBeforeDraft);
                    existingService.ServiceVersion = existingService.ServiceVersion - 1; // as previous service is removed, decrease version by 1
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveInProgressApplication, loggedInUserEmail);
                }

            }



            if (inProgressApplicationParameters.HasActiveRemovalRequest && inProgressApplicationParameters.InProgressRemovalRequestServiceId > 0)
            {
                var removalService = await context.Service
               .Include(s => s.ServiceRemovalRequest)!.Where(x => x.Id == inProgressApplicationParameters.InProgressRemovalRequestServiceId).FirstOrDefaultAsync();

                var serviceRemovalRequest = removalService?.ServiceRemovalRequest?.Where(x => x.IsRequestPending == true).FirstOrDefault();
            

                removalService.ServiceStatus = serviceRemovalRequest.PreviousServiceStatus;
              
                serviceRemovalRequest.IsRequestPending = false;
                serviceRemovalRequest.Token = null;
                serviceRemovalRequest.TokenId = null;

                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveInProgressApplication, loggedInUserEmail);
            }

            if (inProgressApplicationParameters.HasActiveReassignmentRequest && inProgressApplicationParameters.InProgressReassignmentRequestServiceId > 0)
            {
                var reassignmentService = await context.Service
                .Include(s => s.CabTransferRequest)!.ThenInclude(tr => tr.RequestManagement)
                .Where(x => x.Id == inProgressApplicationParameters.InProgressReassignmentRequestServiceId).FirstOrDefaultAsync();

                if (reassignmentService != null)
                {
                    var transferRequest = reassignmentService.CabTransferRequest!.OrderByDescending(c => c.Id).FirstOrDefault();
                    var request = transferRequest.RequestManagement;
                    reassignmentService.ServiceStatus = transferRequest.PreviousServiceStatus;
                    context.Remove(transferRequest);
                    context.Remove(request);
                    await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.RemoveInProgressApplication, loggedInUserEmail);


                }

            }

            if (inProgressApplicationParameters.HasActiveUpdateRequest && inProgressApplicationParameters.InProgressUpdateRequestServiceIds != null && inProgressApplicationParameters.InProgressUpdateRequestServiceIds.Count > 0)
            {
                foreach (var serviceId in inProgressApplicationParameters.InProgressUpdateRequestServiceIds)
                {
                    if (serviceId != inProgressApplicationParameters.InProgressApplicationId)// if not already removed in first condition
                    {
                        var updateRequestService = await context.Service
                         .Include(s => s.ServiceDraft)!
                         .Where(x => x.Id == serviceId).FirstOrDefaultAsync();

                        if (updateRequestService != null && updateRequestService.ServiceDraft != null)
                        {

                            updateRequestService.ServiceStatus = updateRequestService.ServiceDraft.PreviousServiceStatus;
                            context.Remove(updateRequestService.ServiceDraft);

                        }
                    }
                }

                await context.SaveChangesAsync(TeamEnum.CAB, EventTypeEnum.ReapplyService, loggedInUserEmail);
            }
        }
        private void UpdateExistingServiceRecord(Service service, Service? existingService)
        {
            existingService.TrustFrameworkVersionId = service.TrustFrameworkVersionId;
            existingService.ServiceName = service.ServiceName;
            existingService.WebSiteAddress = service.WebSiteAddress;
            existingService.CompanyAddress = service.CompanyAddress;

            if (existingService.ServiceRoleMapping != null & existingService.ServiceRoleMapping?.Count > 0)
                context.ServiceRoleMapping.RemoveRange(existingService.ServiceRoleMapping);
            existingService.ServiceRoleMapping = service.ServiceRoleMapping;

            if (existingService.ServiceIdentityProfileMapping != null & existingService.ServiceIdentityProfileMapping?.Count > 0)
                context.ServiceIdentityProfileMapping.RemoveRange(existingService.ServiceIdentityProfileMapping);
            existingService.ServiceIdentityProfileMapping = service.ServiceIdentityProfileMapping;

            if (existingService.ServiceQualityLevelMapping != null & existingService.ServiceQualityLevelMapping?.Count > 0)
                context.ServiceQualityLevelMapping.RemoveRange(existingService.ServiceQualityLevelMapping);
            existingService.ServiceQualityLevelMapping = service.ServiceQualityLevelMapping;

            existingService.HasSupplementarySchemes = service.HasSupplementarySchemes;


            existingService.ServiceType = service.ServiceType;
            if (service.ServiceType == ServiceTypeEnum.WhiteLabelled)
            {
                existingService.IsUnderPinningServicePublished = service.IsUnderPinningServicePublished;

                if (service.IsUnderPinningServicePublished == true) // publised underpinning service selected
                {
                    existingService.UnderPinningServiceId = service.UnderPinningServiceId;

                    if (existingService.ManualUnderPinningServiceId != null)
                    {
                        if (context.Service.Where(s => s.ManualUnderPinningServiceId == existingService.ManualUnderPinningServiceId).Count() > 1)
                        {
                            existingService.ManualUnderPinningServiceId = null;
                        }
                        else
                        {
                            var manualServiceToRemove = context.ManualUnderPinningService
                                .FirstOrDefault(s => s.Id == existingService.ManualUnderPinningServiceId);
                            context.ManualUnderPinningService.Remove(manualServiceToRemove);
                        }
                    }
                }

                if (service.IsUnderPinningServicePublished == false )
                {
                    existingService.UnderPinningServiceId = null;
                    if ((service.ManualUnderPinningServiceId == null || service.ManualUnderPinningServiceId == 0) &&
                         service.ManualUnderPinningService != null)
                    {
                        existingService.ManualUnderPinningService = service.ManualUnderPinningService; // insert as new manaul under pinning service
                    }

                    else if (service.ManualUnderPinningServiceId != null || service.ManualUnderPinningServiceId != 0)// a manual under pinning service updated
                    {
                        if (service.ManualUnderPinningServiceId != existingService.ManualUnderPinningServiceId)
                        {
                            if (context.Service.Where(s => s.ManualUnderPinningServiceId == existingService.ManualUnderPinningServiceId).Count() <= 1)
                            {
                                var manualServiceToRemove = context.ManualUnderPinningService
                                    .FirstOrDefault(s => s.Id == existingService.ManualUnderPinningServiceId);
                                context.ManualUnderPinningService.Remove(manualServiceToRemove);
                            }
                        }
                        existingService.ManualUnderPinningServiceId = service.ManualUnderPinningServiceId;
                        if (existingService.ManualUnderPinningService != null && service.ManualUnderPinningService != null)
                        // if there is an already existing manual service mapping update it
                        {
                            existingService.ManualUnderPinningService.ServiceName = service.ManualUnderPinningService.ServiceName;
                            existingService.ManualUnderPinningService.ProviderName = service.ManualUnderPinningService.ProviderName;
                            existingService.ManualUnderPinningService.CabId = service.ManualUnderPinningService.CabId;
                            existingService.ManualUnderPinningService.CertificateExpiryDate = service.ManualUnderPinningService.CertificateExpiryDate;
                        }

                    }
                }
            }
            else
            {
                existingService.UnderPinningServiceId = null;
                existingService.IsUnderPinningServicePublished = null;

                if (existingService.ManualUnderPinningServiceId != null)
                {
                    if (context.Service.Where(s => s.ManualUnderPinningServiceId == existingService.ManualUnderPinningServiceId).Count() > 1)
                    {
                        existingService.ManualUnderPinningServiceId = null;
                    }
                    else
                    {
                        var manualServiceToRemove = context.ManualUnderPinningService
                            .FirstOrDefault(s => s.Id == existingService.ManualUnderPinningServiceId);
                        context.ManualUnderPinningService.Remove(manualServiceToRemove);
                    }
                }
            }
            existingService.HasGPG44 = service.HasGPG44;
            existingService.HasGPG45 = service.HasGPG45;
            if (existingService.ServiceSupSchemeMapping != null & existingService.ServiceSupSchemeMapping?.Count > 0)
            {
                context.ServiceSupSchemeMapping.RemoveRange(existingService.ServiceSupSchemeMapping);

            }

            existingService.ServiceSupSchemeMapping = service.ServiceSupSchemeMapping;
            existingService.FileLink = service.FileLink;
            existingService.FileName = service.FileName;
            existingService.FileSizeInKb = service.FileSizeInKb;
            existingService.ConformityIssueDate = service.ConformityIssueDate;
            existingService.ConformityExpiryDate = service.ConformityExpiryDate;
            existingService.ServiceStatus = service.ServiceStatus;
            existingService.ModifiedTime = DateTime.UtcNow;
        }



        // For event logs, need to attach each item to context
        private void AttachListToDbContext(Service service)
        {
            if(service.ServiceRoleMapping != null)
            {
                foreach (var mapping in service.ServiceRoleMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                }
            }
          
            if(service.ServiceIdentityProfileMapping!=null)
            {
                foreach (var mapping in service.ServiceIdentityProfileMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                }
            }


            if (service.ServiceQualityLevelMapping != null)
            {
                foreach (var mapping in service.ServiceQualityLevelMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                } 
            }
            if (service.ServiceSupSchemeMapping !=null )
            {
                foreach (var mapping in service.ServiceSupSchemeMapping)
                {
                    context.Entry(mapping).State = EntityState.Added;
                } 
            }
        }

        private void UpdateExistingProfileRecord(ProviderProfile existingProfile, ProviderProfile newProfile)
        {

            existingProfile.ProviderStatus = newProfile.ProviderStatus;
            existingProfile.RegisteredName = newProfile.RegisteredName ?? existingProfile.RegisteredName;
            existingProfile.TradingName = newProfile.TradingName ?? existingProfile.TradingName;
            existingProfile.HasRegistrationNumber = newProfile.HasRegistrationNumber ?? existingProfile.HasRegistrationNumber;
            if (newProfile.HasRegistrationNumber == true)
            {
                existingProfile.CompanyRegistrationNumber = newProfile.CompanyRegistrationNumber ?? existingProfile.CompanyRegistrationNumber;
                existingProfile.DUNSNumber = null;
            }
            else if (newProfile.HasRegistrationNumber == false)
            {
                existingProfile.DUNSNumber = newProfile.DUNSNumber ?? existingProfile.DUNSNumber;
                existingProfile.CompanyRegistrationNumber = null;
            }


            existingProfile.HasParentCompany = newProfile.HasParentCompany ?? existingProfile.HasParentCompany;
            existingProfile.ParentCompanyRegisteredName = newProfile.ParentCompanyRegisteredName ?? existingProfile.ParentCompanyRegisteredName;
            existingProfile.ParentCompanyLocation = newProfile.ParentCompanyLocation ?? existingProfile.ParentCompanyLocation;
            existingProfile.PrimaryContactFullName = newProfile.PrimaryContactFullName ?? existingProfile.PrimaryContactFullName;
            existingProfile.PrimaryContactJobTitle = newProfile.PrimaryContactJobTitle ?? existingProfile.PrimaryContactJobTitle;
            existingProfile.PrimaryContactEmail = newProfile.PrimaryContactEmail ?? existingProfile.PrimaryContactEmail;
            existingProfile.PrimaryContactTelephoneNumber = newProfile.PrimaryContactTelephoneNumber ?? existingProfile.PrimaryContactTelephoneNumber;
            existingProfile.SecondaryContactFullName = newProfile.SecondaryContactFullName ?? existingProfile.SecondaryContactFullName;
            existingProfile.SecondaryContactJobTitle = newProfile.SecondaryContactJobTitle ?? existingProfile.SecondaryContactJobTitle;
            existingProfile.SecondaryContactEmail = newProfile.SecondaryContactEmail ?? existingProfile.SecondaryContactEmail;
            existingProfile.SecondaryContactTelephoneNumber = newProfile.SecondaryContactTelephoneNumber ?? existingProfile.SecondaryContactTelephoneNumber;
            existingProfile.PublicContactEmail = newProfile.PublicContactEmail ?? existingProfile.PublicContactEmail;
            existingProfile.ProviderTelephoneNumber = newProfile.ProviderTelephoneNumber ?? existingProfile.ProviderTelephoneNumber;
            existingProfile.ProviderWebsiteAddress = newProfile.ProviderWebsiteAddress ?? existingProfile.ProviderWebsiteAddress;
            existingProfile.LinkToContactPage = newProfile.LinkToContactPage ?? existingProfile.LinkToContactPage;
        }
        #endregion
    }
}
