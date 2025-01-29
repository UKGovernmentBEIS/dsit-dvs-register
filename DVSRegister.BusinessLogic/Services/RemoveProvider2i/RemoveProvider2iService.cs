using AutoMapper;
using DVSAdmin.CommonUtility.Models.Enums;
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
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count>0)
                {
                    var mappedServiceIds = removeProviderToken.RemoveTokenServiceMapping.Where(mapping => mapping.RemoveProviderTokenId == removeProviderToken.Id).Select(mapping => mapping.ServiceId).ToList();
                    ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
                    providerProfileDto.Services = providerProfileDto.Services.Where(service => mappedServiceIds.Contains(service.Id)).ToList();
                    return providerProfileDto;

                }
                else
                {
                    ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);                    
                    return providerProfileDto;
                }
             
            }
            else
            {
                return null;
            }
        }

        public async Task<GenericResponse> UpdateRemovalStatus(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            string serviceNames = string.Empty;
            string removalReason = RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value);

            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            var filteredServiceNames = providerDto.Services.Select(s => s.ServiceName).ToList();
            serviceNames = string.Join("\r", filteredServiceNames);

            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {                   
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    // get updated service list and decide provider status
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    // update provider status
                    ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                    if (genericResponse.Success)
                    {
                        // in the current release only one service expected
                        var serviceRemovalReasonList= providerDto.Services.Where(s => s.ServiceRemovalReason.HasValue) 
                         .Select(s => ServiceRemovalReasonEnumExtensions.GetDescription(s.ServiceRemovalReason.Value)) .ToList();
                        string serviceRemovalReasons = string.Join("\r", serviceRemovalReasonList);

                        await emailSender.RemoveServiceConfirmationToProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, serviceNames, serviceRemovalReasons); //39/Provider/Confirmation of service removal request
                        await emailSender.RemoveServiceConfirmationToProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, serviceNames, serviceRemovalReasons);
                        await emailSender.ServiceRemovalConfirmationToDSIT(providerDto.RegisteredName, serviceNames, serviceRemovalReasons);//40/DSIT/Service removal request received

                       
                        await Task.Delay(500);
                        await emailSender.ServiceRemovedConfirmedToCabOrProvider(providerDto.CabUser.CabEmail, providerDto.CabUser.CabEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                        await emailSender.ServiceRemovedToDSIT(serviceNames, serviceRemovalReasons); //42/DSIT/Service removed
                        await emailSender.ServiceRemovedConfirmedToCabOrProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                        await emailSender.ServiceRemovedConfirmedToCabOrProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                    }
                }
                else
                {                 
                    //remove provider
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, null, loggedInUserEmail);


                    if (genericResponse.Success)
                    {
                        if (team == TeamEnum.Provider) // if provider confirms
                        {
                            await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);//31/Provider/Removal request confirmed
                            await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                            await emailSender.SendProviderRemovalConfirmationToDSIT(providerDto.RegisteredName, serviceNames);//32/DSIT/Provider removal confirmation
                        }

                        else if (team == TeamEnum.DSIT) //if dsit confirms 2i check
                        {                            
                            await emailSender._2iCheckApprovedNotificationToDSIT(providerDto.RegisteredName, serviceNames, removalReason);

                        }

                        await Task.Delay(500);
                        await emailSender.SendRecordRemovedToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                        await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.CabUser.CabEmail, providerDto.CabUser.CabEmail, providerDto.RegisteredName, serviceNames, removalReason);
                        await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, providerDto.RegisteredName, serviceNames, removalReason);
                        await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, providerDto.RegisteredName, serviceNames, removalReason);
                    }
                }
            }
            return genericResponse;
        }


        public async Task<GenericResponse> CancelRemovalRequest(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            string serviceNames = string.Empty;
            string removalReason = string.Empty;

            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            var filteredServiceNames = providerDto.Services.Select(s => s.ServiceName).ToList();
            serviceNames = string.Join("\r", filteredServiceNames);


            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {
                   
                    genericResponse = await removeProvider2iRepository.CancelServiceRemoval(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    // get updated service list and decide provider status
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    // update provider status
                    ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);


                }
                else
                {
                    removalReason = RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value);
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    serviceIds = providerProfile.Services.Select(s => s.Id).ToList(); // for all services
                                                                                    
                    genericResponse = await removeProvider2iRepository.CancelServiceRemoval(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                                     
                }

                if (genericResponse.Success)
                {
                    if (team == TeamEnum.Provider)
                    {
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);//33/Provider/Removal request declined
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                        await emailSender.RemovalRequestDeclinedToDSIT(providerDto.RegisteredName, serviceNames); //34 / DSIT / Removal request declined by provider

                    }
                    else if (team == TeamEnum.DSIT)
                    {
                        await emailSender._2iCheckDeclinedNotificationToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                    }

                }

            }
            return genericResponse;
        }

        public async Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail)
        {
            return await removeProvider2iRepository.RemoveRemovalToken(token, tokenId, loggedInUserEmail);
        }

        #region private methods
       
        #endregion
    }
}
