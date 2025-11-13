using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
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
        public EditService(IEditRepository editRepository, IMapper mapper, ILogger<EditService> logger)
        {
          this.editRepository = editRepository;
          this.mapper = mapper;
          this.logger = logger;
          
        }

        public async Task<ProviderProfileDto> GetProviderDetails(int providerId)
        {
            var provider = await editRepository.GetProviderDetails(providerId);
            ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
            return providerProfileDto;
        }
        public async Task<GenericResponse> SaveProviderDraft(ProviderProfileDraftDto draftDto, string loggedInUserEmail, List<string> dsitUserEmails)
        {

            var draftEntity = mapper.Map<ProviderProfileDraft>(draftDto);
            var response = await editRepository.SaveProviderDraft(draftEntity, loggedInUserEmail);
            if (response.Success)
            {
                //response = await SendProviderUpdateEmail(draftDto, loggedInUserEmail, dsitUserEmails);
            }
            return response;
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
   
        }

    }
}
