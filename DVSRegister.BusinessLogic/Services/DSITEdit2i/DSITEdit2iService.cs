using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
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
        private readonly Edit2iCheckEmailSender emailSender;        
        private readonly IRemoveProviderService removeProviderService;



        public DSITEdit2iService(IDSITEdit2iRepository dSITEdit2IRepository, IMapper mapper, Edit2iCheckEmailSender emailSender,  IRemoveProviderService removeProviderService)
        {
            this.dSITEdit2IRepository = dSITEdit2IRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;            
            this.removeProviderService = removeProviderService;
        }
        public async Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId)
        {
            ProviderDraftToken providerDraftToken = await dSITEdit2IRepository.GetProviderDraftToken(token, tokenId);
            ProviderDraftTokenDto providerDraftTokenDto = mapper.Map<ProviderDraftTokenDto>(providerDraftToken);
            return providerDraftTokenDto;

        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(ProviderProfileDraftDto providerProfileDraft)
        {
            
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateProviderAndServiceStatusAndData(providerProfileDraft.ProviderProfileId, providerProfileDraft.Id);    
            if(genericResponse.Success) 
            {

                var (previous, current) = GetProviderKeyValue(providerProfileDraft, providerProfileDraft.Provider);
                string currentData = Helper.ConcatenateKeyValuePairs(current);
                string previousData = Helper.ConcatenateKeyValuePairs(previous);

               string userEmail = providerProfileDraft.User.Email;
               await emailSender.EditProviderAccepted(userEmail, userEmail, providerProfileDraft.Provider.RegisteredName, currentData, previousData);
            }
            return genericResponse;
        }

        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveProviderDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelProviderUpdates(ProviderProfileDraftDto providerProfileDraft)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelProviderUpdates(providerProfileDraft.ProviderProfileId, providerProfileDraft.Id);

            if(genericResponse.Success)
            {
                string userEmail = providerProfileDraft.User.Email;
                await emailSender.EditProviderDeclined(userEmail, userEmail, providerProfileDraft.Provider.RegisteredName);
            }
            return genericResponse;
        }


        public async Task<TokenStatusEnum> GetEditProviderTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum tokenStatus = TokenStatusEnum.NA;
            if (tokenDetails.ProviderProfileId>0 )
            {
                var provider = await dSITEdit2IRepository.GetProvider(tokenDetails.ProviderProfileId);
                tokenStatus = provider.EditProviderTokenStatus;
            }

            return tokenStatus;
        }   

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetProviderKeyValue(ProviderProfileDraftDto currentData, ProviderProfileDto previousData)
        {
            var previousDataDictionary = new Dictionary<string, List<string>>();

            var currentDataDictionary = new Dictionary<string, List<string>>();

            if (currentData.RegisteredName != null)
            {
                previousDataDictionary.Add("Registered name", [previousData.RegisteredName]);
                currentDataDictionary.Add("Registered name", [currentData.RegisteredName]);
            }

            if (currentData.TradingName != null)
            {
                previousDataDictionary.Add("Trading name", [previousData.TradingName]);
                currentDataDictionary.Add("Trading name", [currentData.TradingName]);
            }

            if (currentData.PrimaryContactFullName != null)
            {
                previousDataDictionary.Add("Primary contact full name", [previousData.PrimaryContactFullName]);
                currentDataDictionary.Add("Primary contact full name", [currentData.PrimaryContactFullName]);
            }
            if (currentData.PrimaryContactEmail != null)
            {
                previousDataDictionary.Add("Primary contact email", [previousData.PrimaryContactEmail]);
                currentDataDictionary.Add("Primary contact email", [currentData.PrimaryContactEmail]);
            }
            if (currentData.PrimaryContactJobTitle != null)
            {
                previousDataDictionary.Add("Primary contact job title", [previousData.PrimaryContactJobTitle]);
                currentDataDictionary.Add("primary contact job title", [currentData.PrimaryContactJobTitle]);
            }
            if (currentData.PrimaryContactTelephoneNumber != null)
            {
                previousDataDictionary.Add("Primary contact telephone number", [previousData.PrimaryContactTelephoneNumber]);
                currentDataDictionary.Add("Primary contact telephone number", [currentData.PrimaryContactTelephoneNumber]);
            }

            if (currentData.SecondaryContactFullName != null)
            {
                previousDataDictionary.Add("Secondary contact full name", [previousData.SecondaryContactFullName]);
                currentDataDictionary.Add("Secondary contact full name", [currentData.SecondaryContactFullName]);
            }
            if (currentData.SecondaryContactEmail != null)
            {
                previousDataDictionary.Add("Secondary contact email", [previousData.SecondaryContactEmail]);
                currentDataDictionary.Add("Secondary contact email", [currentData.SecondaryContactEmail]);
            }
            if (currentData.SecondaryContactJobTitle != null)
            {
                previousDataDictionary.Add("Secondary contact job title", [previousData.SecondaryContactJobTitle]);
                currentDataDictionary.Add("Secondary contact job title", [currentData.SecondaryContactJobTitle]);
            }
            if (currentData.SecondaryContactTelephoneNumber != null)
            {
                previousDataDictionary.Add("Secondary contact telephone number", [previousData.SecondaryContactTelephoneNumber]);
                currentDataDictionary.Add("Secondary contact telephone number", [currentData.SecondaryContactTelephoneNumber]);
            }

            if (currentData.ProviderWebsiteAddress != null)
            {
                previousDataDictionary.Add("Provider website address", [previousData.ProviderWebsiteAddress]);
                currentDataDictionary.Add("Provider website address", [currentData.ProviderWebsiteAddress]);
            }

            if (currentData.PublicContactEmail != null)
            {
                previousDataDictionary.Add("Public contact email", [previousData.PublicContactEmail]);
                currentDataDictionary.Add("Public contact email", [currentData.PublicContactEmail]);
            }

            if (currentData.ProviderTelephoneNumber != null)
            {
                previousDataDictionary.Add("Provider telephone number", [previousData.ProviderTelephoneNumber]);
                currentDataDictionary.Add("Provider telephone number", [currentData.ProviderTelephoneNumber]);
            }

            return (previousDataDictionary, currentDataDictionary);
        }



    }
}
