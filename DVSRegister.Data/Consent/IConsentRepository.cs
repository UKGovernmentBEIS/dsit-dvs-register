﻿using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IConsentRepository
    {
        #region opening the loop
        public Task<ProceedApplicationConsentToken> GetProceedApplicationConsentToken(string token, string tokenId);
        public Task<bool> RemoveProceedApplicationConsentToken(string token, string tokenId, string loggedInUserEmail);    
        public Task<GenericResponse> UpdateServiceStatus(int serviceId, ServiceStatusEnum serviceStatus, string providerEmail);
        #endregion


        #region closing the loop
        
        public Task<bool> RemoveProceedPublishConsentToken(string token, string tokenId,string loggedInUserEmail);
        public Task<ProceedPublishConsentToken> GetProceedPublishConsentToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceAndProviderStatus(int serviceId,  string loggedInUserEmail);

        #endregion

        public Task<Service> GetServiceDetails(int serviceId);
        public Task<List<Service>> GetServiceList(int providerId);

        public Task<Service> GetService(int serviceId);
    }
}
