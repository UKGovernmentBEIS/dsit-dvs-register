using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class RemoveProvider2iService :IRemoveProvider2iService 
    {

        private readonly IRemoveProvider2iRepository removeProvider2iRepository;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;


        public RemoveProvider2iService(IRemoveProvider2iRepository removeProvider2iRepository, IMapper mapper, IEmailSender emailSender)
        {
            this.removeProvider2iRepository = removeProvider2iRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
        }
        public async Task<ProviderProfileDto?> GetProviderAndServiceDetailsByRemovalToken(string token, string tokenId)
        {
            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (removeProviderToken.Provider != null)
            {
                var provider = await removeProvider2iRepository.GetProviderDetails(removeProviderToken.ProviderProfileId);
                ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
                return providerProfileDto;
            }
            else
            {
                return null;
            }
        }

        public async Task<GenericResponse> UpdateRemovalStatus(string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            TeamEnum teamEnum = TeamEnum.Provider;// To Do : update after correcting Removal Reason to int
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {
                    List<int> serviceIds = removeProviderToken.RemoveTokenServiceMapping.Select(mapping => mapping.ServiceId).ToList();
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, teamEnum, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                }
                else
                {
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, teamEnum, EventTypeEnum.RemoveServices2i, null, loggedInUserEmail);
                }

                if (genericResponse.Success)
                {
                    // ToDo : send email confirmation                
                }

            }
            return genericResponse;
        }

        public async Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail)
        {
            return await removeProvider2iRepository.RemoveRemovalToken(token, tokenId, loggedInUserEmail);
        }
    }
}
