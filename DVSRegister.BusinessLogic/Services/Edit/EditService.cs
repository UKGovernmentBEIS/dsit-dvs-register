using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Edit;
using DVSRegister.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Text;

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

        public async Task<ProviderProfileDto> GetProviderDetails(int providerId, int cabId)
        {
            var provider = await editRepository.GetProviderDetails(providerId, cabId);
            ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
            return providerProfileDto;
        }
        public async Task<GenericResponse> SaveProviderDraft(ProviderProfileDraftDto draftDto, string loggedInUserEmail, int cabId)
        {

            var draftEntity = mapper.Map<ProviderProfileDraft>(draftDto);
            var response = await editRepository.SaveProviderDraft(draftEntity, loggedInUserEmail);
            if (response.Success)
            {
                await SendProviderEditRequestSubmittedEmail(draftDto, loggedInUserEmail, cabId);
            }
            return response;
        }

        private async Task SendProviderEditRequestSubmittedEmail(ProviderProfileDraftDto draftDto, string loggedInUserEmail, int cabId)
        { 
          
            try
            {
                ProviderProfile providerProfile = await editRepository.GetProviderDetails(draftDto.ProviderProfileId, cabId);
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
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetSecondaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.SecondaryContactFullName != previousData.SecondaryContactFullName)
            {
                previousDataDictionary.Add(Constants.SecondaryContactName, [previousData.SecondaryContactFullName]);
                currentDataDictionary.Add(Constants.SecondaryContactName, [currentData.SecondaryContactFullName]);
            }
            if (currentData.SecondaryContactEmail != previousData.SecondaryContactEmail)
            {
                previousDataDictionary.Add(Constants.SecondaryContactEmail, [previousData.SecondaryContactEmail]);
                currentDataDictionary.Add(Constants.SecondaryContactEmail, [currentData.SecondaryContactEmail]);
            }
            if (currentData.SecondaryContactJobTitle != previousData.SecondaryContactJobTitle)
            {
                previousDataDictionary.Add(Constants.SecondaryContactJobTitle, [previousData.SecondaryContactJobTitle]);
                currentDataDictionary.Add(Constants.SecondaryContactJobTitle, [currentData.SecondaryContactJobTitle]);
            }
            if (currentData.SecondaryContactTelephoneNumber != previousData.SecondaryContactTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.SecondaryContactTelephone, [previousData.SecondaryContactTelephoneNumber]);
                currentDataDictionary.Add(Constants.SecondaryContactTelephone, [currentData.SecondaryContactTelephoneNumber]);
            }

            return (currentDataDictionary, previousDataDictionary);

        }
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPrimaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.PrimaryContactFullName != previousData.PrimaryContactFullName)
            {
                previousDataDictionary.Add(Constants.PrimaryContactName, [previousData.PrimaryContactFullName]);
                currentDataDictionary.Add(Constants.PrimaryContactName, [currentData.PrimaryContactFullName]);
            }
            if (currentData.PrimaryContactEmail != previousData.PrimaryContactEmail)
            {
                previousDataDictionary.Add(Constants.PrimaryContactEmail, [previousData.PrimaryContactEmail]);
                currentDataDictionary.Add(Constants.PrimaryContactEmail, [currentData.PrimaryContactEmail]);
            }
            if (currentData.PrimaryContactJobTitle != previousData.PrimaryContactJobTitle)
            {
                previousDataDictionary.Add(Constants.PrimaryContactJobTitle, [previousData.PrimaryContactJobTitle]);
                currentDataDictionary.Add(Constants.PrimaryContactJobTitle, [currentData.PrimaryContactJobTitle]);
            }
            if (currentData.PrimaryContactTelephoneNumber != previousData.PrimaryContactTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.PrimaryContactTelephone, [previousData.PrimaryContactTelephoneNumber]);
                currentDataDictionary.Add(Constants.PrimaryContactTelephone, [currentData.PrimaryContactTelephoneNumber]);
            }

            return (currentDataDictionary, previousDataDictionary);

        }

        public async Task ConfirmPrimaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName,
            string providerName)
        {
            var labels = new Dictionary<string, string>
            {
                { Constants.PrimaryContactName, "Full name of primary contact" },
                { Constants.PrimaryContactJobTitle, "Job title of primary contact" },
                { Constants.PrimaryContactEmail, "Email address of primary contact" },
                { Constants.PrimaryContactTelephone, "Telephone number of primary contact" }
            };

            await ConfirmContactUpdates(current, previous, emailAddress, recipientName, providerName, labels, "Primary contact’s details");
        }
        public async Task ConfirmSecondaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName,
            string providerName)
        {
            var labels = new Dictionary<string, string>
            {
                { Constants.SecondaryContactName, "Full name of secondary contact" },
                { Constants.SecondaryContactJobTitle, "Job title of secondary contact" },
                { Constants.SecondaryContactEmail, "Email address of secondary contact" },
                { Constants.SecondaryContactTelephone, "Telephone number of secondary contact" }
            };

            await ConfirmContactUpdates(current, previous, emailAddress, recipientName, providerName, labels, "Secondary contact’s details");
        }

        public async Task<GenericResponse> UpdateCompanyInfoAndPublicProviderInfo(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            mapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await editRepository.UpdateCompanyInfoAndPublicProviderInfo(providerProfile, loggedInUserEmail);
            return genericResponse;
        }
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPublicContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.ProviderWebsiteAddress != previousData.ProviderWebsiteAddress)
            {
                previousDataDictionary.Add(Constants.ProviderWebsiteAddress, [previousData.ProviderWebsiteAddress]);
                currentDataDictionary.Add(Constants.ProviderWebsiteAddress, [currentData.ProviderWebsiteAddress]);
            }
            if (currentData.PublicContactEmail != previousData.PublicContactEmail)
            {
                previousDataDictionary.Add(Constants.PublicContactEmail, [string.IsNullOrEmpty(previousData.PublicContactEmail) ? Constants.NullFieldsDisplay : previousData.PublicContactEmail]);
                currentDataDictionary.Add(Constants.PublicContactEmail, [string.IsNullOrEmpty(currentData.PublicContactEmail) ? Constants.NullFieldsDisplay : currentData.PublicContactEmail]);
            }
            if (currentData.ProviderTelephoneNumber != previousData.ProviderTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.ProviderTelephoneNumber, [string.IsNullOrEmpty(previousData.ProviderTelephoneNumber) ? Constants.NullFieldsDisplay : previousData.ProviderTelephoneNumber]);
                currentDataDictionary.Add(Constants.ProviderTelephoneNumber, [string.IsNullOrEmpty(currentData.ProviderTelephoneNumber) ? Constants.NullFieldsDisplay : currentData.ProviderTelephoneNumber]);
            }
            if (currentData.LinkToContactPage != previousData.LinkToContactPage)
            {
                previousDataDictionary.Add(Constants.LinkToContactPage, [string.IsNullOrEmpty(previousData.LinkToContactPage) ? Constants.NullFieldsDisplay : previousData.LinkToContactPage]);
                currentDataDictionary.Add(Constants.LinkToContactPage, [string.IsNullOrEmpty(currentData.LinkToContactPage) ? Constants.NullFieldsDisplay : currentData.LinkToContactPage]);
            }


            return (currentDataDictionary, previousDataDictionary);

        }
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetCompanyValueUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.RegisteredName != previousData.RegisteredName)
            {
                previousDataDictionary.Add(Constants.RegisteredName, [previousData.RegisteredName]);
                currentDataDictionary.Add(Constants.RegisteredName, [currentData.RegisteredName]);
            }
            if (currentData.TradingName != previousData.TradingName)
            {
                previousDataDictionary.Add(Constants.TradingName, [string.IsNullOrEmpty(previousData.TradingName) ? Constants.NullFieldsDisplay : previousData.TradingName]);
                currentDataDictionary.Add(Constants.TradingName, [string.IsNullOrEmpty(currentData.TradingName) ? Constants.NullFieldsDisplay : currentData.TradingName]);
            }

            if (previousData.HasRegistrationNumber == true && currentData.CompanyRegistrationNumber != previousData.CompanyRegistrationNumber)
            {
                currentDataDictionary.Add(Constants.CompanyRegistrationNumber, [currentData.CompanyRegistrationNumber]);
                previousDataDictionary.Add(Constants.CompanyRegistrationNumber, [previousData.CompanyRegistrationNumber]);
            }
            if (previousData.HasRegistrationNumber == false && currentData.DUNSNumber != previousData.DUNSNumber)
            {

                currentDataDictionary.Add(Constants.DUNSNumber, [currentData.DUNSNumber]);
                previousDataDictionary.Add(Constants.DUNSNumber, [previousData.DUNSNumber]);
            }

            if (previousData.HasParentCompany == true)
            {
                if (currentData.ParentCompanyRegisteredName != previousData.ParentCompanyRegisteredName)
                {
                    currentDataDictionary.Add(Constants.ParentCompanyRegisteredName, [currentData.ParentCompanyRegisteredName]);
                    previousDataDictionary.Add(Constants.ParentCompanyRegisteredName, [previousData.ParentCompanyRegisteredName]);
                }
                if (currentData.ParentCompanyLocation != previousData.ParentCompanyLocation)
                {
                    currentDataDictionary.Add(Constants.ParenyCompanyLocation, [currentData.ParentCompanyLocation]);
                    previousDataDictionary.Add(Constants.ParenyCompanyLocation, [previousData.ParentCompanyLocation]);
                }
            }

            return (currentDataDictionary, previousDataDictionary);
        }
       
        public async Task<GenericResponse> UpdatePrimaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            mapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await editRepository.UpdatePrimaryContact(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> UpdateSecondaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            mapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await editRepository.UpdateSecondaryContact(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

       
        #region Private Methods
        private async Task ConfirmContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName,
            string providerName, Dictionary<string, string> labels, string header)
        {
            var previousData = new StringBuilder();
            var newData = new StringBuilder();

            previousData.AppendLine(header);
            previousData.AppendLine();
            newData.AppendLine(header);
            newData.AppendLine();

            foreach (var key in previous.Keys)
            {
                if (labels.TryGetValue(key, out var label))
                {
                    var prevValue = previous[key];
                    var currValue = current[key];

                    previousData.AppendLine($"{label}: {string.Join(", ", prevValue)}");
                    newData.AppendLine($"{label}: {string.Join(", ", currValue)}");
                }
            }

            previousData.AppendLine();
            newData.AppendLine();

            await emailSender.SendEmailContactUpdatesToCab(
                emailAddress, recipientName, providerName, previousData.ToString(), newData.ToString());

            await emailSender.SendEmailContactUpdatesToDSIT(
                providerName, previousData.ToString(), newData.ToString());
        }

        #endregion

    }
}
