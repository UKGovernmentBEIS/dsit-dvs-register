using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;

namespace DVSRegister.BusinessLogic.Services
{
    public class DSITEdit2iService : IDSITEdit2iService
    {
        private readonly IDSITEdit2iRepository dSITEdit2IRepository;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;


        public DSITEdit2iService(IDSITEdit2iRepository dSITEdit2IRepository, IMapper mapper, IEmailSender emailSender)
        {
            this.dSITEdit2IRepository = dSITEdit2IRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
        }
        public async Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId)
        {
            ProviderDraftToken providerDraftToken = await dSITEdit2IRepository.GetProviderDraftToken(token, tokenId);
            ProviderDraftTokenDto providerDraftTokenDto = mapper.Map<ProviderDraftTokenDto>(providerDraftToken);
            return providerDraftTokenDto;

        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateProviderAndServiceStatusAndData(providerProfileId, providerDraftId);       
            return genericResponse;
        }

        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveProviderDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelProviderUpdates(providerProfileId, providerDraftId);
            return genericResponse;
        }


        public async Task<ServiceDraftTokenDto> GetServiceChangesByToken(string token, string tokenId)
        {
            ServiceDraftToken serviceDraftToken = await dSITEdit2IRepository.GetServiceDraftToken(token, tokenId);
            ServiceDraftTokenDto serviceDraftTokenDto = mapper.Map<ServiceDraftTokenDto>(serviceDraftToken);
            return serviceDraftTokenDto;

        }


        public async Task<GenericResponse> UpdateServiceStatus(int serviceId, int serviceDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateServiceStatus(serviceId, serviceDraftId);
            return genericResponse;
        }

        public async Task<bool> RemoveServiceDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveServiceDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelServiceUpdates(int serviceId, int serviceDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelServiceUpdates(serviceId, serviceDraftId);
            return genericResponse;
        }
    }
}
