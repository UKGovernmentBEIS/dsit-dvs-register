using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Edit;
using DVSRegister.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DVSRegister.BusinessLogic.Services.Edit
{
    public class EditService : IEditService
    {
        private readonly IEditRepository editRepository;    
        private readonly IMapper mapper;
        private readonly ILogger<EditService> logger;
        private readonly ProviderEditEmailSender emailSender;
        public EditService(IEditRepository editRepository, IMapper mapper, ProviderEditEmailSender emailSender, ILogger<EditService> logger)
        {
          this.editRepository = editRepository;
          this.mapper = mapper;
          this.emailSender = emailSender;
          this.logger = logger;
          
        }

        public async Task<ProviderProfileDto> GetProviderDetails(int providerId)
        {
            var provider = await editRepository.GetProviderDetails(providerId);
            ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
            return providerProfileDto;
        }
        public async Task<GenericResponse> SaveProviderDraft(ProviderProfileDraftDto draftDto, string loggedInUserEmail)
        {

            var draftEntity = mapper.Map<ProviderProfileDraft>(draftDto);
            var response = await editRepository.SaveProviderDraft(draftEntity, loggedInUserEmail);
            if (response.Success)
            {
                await SendProviderEditRequestSubmittedEmail(draftDto, loggedInUserEmail);
            }
            return response;
        }

        private async Task SendProviderEditRequestSubmittedEmail(ProviderProfileDraftDto draftDto, string loggedInUserEmail)
        { 
          
            try
            {
                ProviderProfile providerProfile = await editRepository.GetProviderDetails(draftDto.ProviderProfileId);
                ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(providerProfile);
                var currentDataDictionary = new Dictionary<string, List<string>>();
                var previousDataDictionary = new Dictionary<string, List<string>>();
                GetProviderKeyValueMappings(draftDto, providerProfileDto, currentDataDictionary, previousDataDictionary);
                string newData = Helper.ConcatenateKeyValuePairs(currentDataDictionary);
                string previousData = Helper.ConcatenateKeyValuePairs(previousDataDictionary);
                await emailSender.SendProviderEditRequestSubmittedToCab(loggedInUserEmail, providerProfileDto.RegisteredName,  previousData, newData);
                await emailSender.SendProviderEditRequestSubmittedToOfdia( providerProfileDto.RegisteredName, previousData,newData );

            }
            catch (Exception ex)
            {
                logger.LogError("{Message}", ex.Message);               
            }
          
        }

        public Task<(Dictionary<string, List<string>>, Dictionary<string, List<string>>)> GetProviderKeyValue(ProviderProfileDraftDto changedProvider, ProviderProfileDto currentProvider)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();
            GetProviderKeyValueMappings(changedProvider, currentProvider, currentDataDictionary, previousDataDictionary);
            return Task.FromResult((previousDataDictionary, currentDataDictionary));
        }

        //Current requrement - only Company info require 2i check with ofdia
        private void GetProviderKeyValueMappings(ProviderProfileDraftDto currentData, ProviderProfileDto previousData, Dictionary<string, List<string>> currentDataDictionary, Dictionary<string, List<string>> previousDataDictionary)
        {
            if (currentData.RegisteredName != null)
            {
                previousDataDictionary.Add(Constants.RegisteredName, [previousData.RegisteredName]);
                currentDataDictionary.Add(Constants.RegisteredName, [currentData.RegisteredName]);
            }
            if (currentData.TradingName != null)
            {
                previousDataDictionary.Add(Constants.TradingName, [string.IsNullOrEmpty(previousData.TradingName) ? Constants.NullFieldsDisplay : previousData.TradingName]);
                currentDataDictionary.Add(Constants.TradingName, [currentData.TradingName]);
            }       
            if (currentData.HasRegistrationNumber == true)
            {
                previousDataDictionary.Add(Constants.CompanyRegistrationNumber, [Constants.NullFieldsDisplay]);
                currentDataDictionary.Add(Constants.CompanyRegistrationNumber, [currentData.CompanyRegistrationNumber]);

                previousDataDictionary.Add(Constants.DUNSNumber, [previousData.DUNSNumber]);
                currentDataDictionary.Add(Constants.DUNSNumber, [Constants.NullFieldsDisplay]);
            }
            if (currentData.HasRegistrationNumber == false)
            {
                previousDataDictionary.Add(Constants.CompanyRegistrationNumber, [previousData.CompanyRegistrationNumber]);
                currentDataDictionary.Add(Constants.CompanyRegistrationNumber, [Constants.NullFieldsDisplay]);

                previousDataDictionary.Add(Constants.DUNSNumber, [Constants.NullFieldsDisplay]);
                currentDataDictionary.Add(Constants.DUNSNumber, [currentData.DUNSNumber]);
            }
            if (currentData.HasRegistrationNumber == null && currentData.CompanyRegistrationNumber != null)
            {
                previousDataDictionary.Add(Constants.CompanyRegistrationNumber, [previousData.CompanyRegistrationNumber]);
                currentDataDictionary.Add(Constants.CompanyRegistrationNumber, [currentData.CompanyRegistrationNumber]);
            }
            if (currentData.HasRegistrationNumber == null && currentData.DUNSNumber != null)
            {
                previousDataDictionary.Add(Constants.DUNSNumber, [previousData.DUNSNumber]);
                currentDataDictionary.Add(Constants.DUNSNumber, [currentData.DUNSNumber]);
            }
            if (currentData.HasParentCompany == true)
            {
                previousDataDictionary.Add(Constants.ParentCompanyRegisteredName, [Constants.NullFieldsDisplay]);
                currentDataDictionary.Add(Constants.ParentCompanyRegisteredName, [currentData.ParentCompanyRegisteredName]);

                previousDataDictionary.Add(Constants.ParenyCompanyLocation, [Constants.NullFieldsDisplay]);
                currentDataDictionary.Add(Constants.ParenyCompanyLocation, [currentData.ParentCompanyLocation]);
            }
            if (currentData.HasParentCompany == false)
            {
                previousDataDictionary.Add(Constants.ParentCompanyRegisteredName, [previousData.ParentCompanyRegisteredName]);
                currentDataDictionary.Add(Constants.ParentCompanyRegisteredName, [Constants.NullFieldsDisplay]);

                previousDataDictionary.Add(Constants.ParenyCompanyLocation, [previousData.ParentCompanyLocation]);
                currentDataDictionary.Add(Constants.ParenyCompanyLocation, [Constants.NullFieldsDisplay]);
            }
            if (currentData.HasParentCompany == null && currentData.ParentCompanyRegisteredName != null)
            {
                previousDataDictionary.Add(Constants.ParentCompanyRegisteredName, [previousData.ParentCompanyRegisteredName]);
                currentDataDictionary.Add(Constants.ParentCompanyRegisteredName, [currentData.ParentCompanyRegisteredName]);
            }
            if (currentData.HasParentCompany == null && currentData.ParentCompanyLocation != null)
            {
                previousDataDictionary.Add(Constants.ParenyCompanyLocation, [previousData.ParentCompanyLocation]);
                currentDataDictionary.Add(Constants.ParenyCompanyLocation, [currentData.ParentCompanyLocation]);
            }
            if (currentData.PublicContactEmail != null)
            {
                previousDataDictionary.Add(Constants.PublicContactEmail, [string.IsNullOrEmpty(previousData.PublicContactEmail) ? Constants.NullFieldsDisplay : previousData.PublicContactEmail]);
                currentDataDictionary.Add(Constants.PublicContactEmail, [currentData.PublicContactEmail]);
            }
            if (currentData.ProviderTelephoneNumber != null)
            {
                previousDataDictionary.Add(Constants.ProviderTelephoneNumber, [previousData.ProviderTelephoneNumber]);
                currentDataDictionary.Add(Constants.ProviderTelephoneNumber, [currentData.ProviderTelephoneNumber]);
            }

            if (currentData.ProviderWebsiteAddress != null)
            {
                previousDataDictionary.Add(Constants.ProviderWebsiteAddress, [previousData.ProviderWebsiteAddress]);
                currentDataDictionary.Add(Constants.ProviderWebsiteAddress, [currentData.ProviderWebsiteAddress]);
            }

            

            if (currentData.LinkToContactPage != null)
            {
                previousDataDictionary.Add(Constants.LinkToContactPage, [string.IsNullOrEmpty(previousData.LinkToContactPage) ? Constants.NullFieldsDisplay : previousData.LinkToContactPage]);
                currentDataDictionary.Add(Constants.LinkToContactPage, [currentData.LinkToContactPage]);
            };

        }

    }
}
