﻿using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IDSITEdit2iRepository
    {
        public Task<ProviderDraftToken> GetProviderDraftToken(string token, string tokenId);
        public Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId);
        public Task<bool> RemoveProviderDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId);
        public Task<ServiceDraftToken> GetServiceDraftToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceStatusAndData(int serviceId, int serviceDraftId);
        public Task<bool> RemoveServiceDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelServiceUpdates(int serviceId, int serviceDraftId);
        public  Task<Service> GetService(int serviceId);
        public Task<ProviderProfile> GetProvider(int providerProfileId);
    }
}
