using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic
{
    public  class ServiceHelper
    {
        public static ProviderStatusEnum GetProviderStatus(ICollection<Service> services, ProviderStatusEnum currentStatus)
        {
            ProviderStatusEnum providerStatus = currentStatus;
            if (services != null && services.Count > 0)
            {

                if (services.All(service => service.ServiceStatus == ServiceStatusEnum.Removed))
                {
                    providerStatus = ProviderStatusEnum.RemovedFromRegister;
                }

                else
                {
                    var priorityOrder = new List<ServiceStatusEnum>
                    {
                        ServiceStatusEnum.CabAwaitingRemovalConfirmation,
                        ServiceStatusEnum.ReadyToPublish,
                        ServiceStatusEnum.AwaitingRemovalConfirmation,
                        ServiceStatusEnum.Published,
                        ServiceStatusEnum.Removed
                    };

                    ServiceStatusEnum highestPriorityStatus = services.Where(service => service.ServiceStatus> ServiceStatusEnum.Received)
                        .Select(service => service.ServiceStatus)
                        .OrderBy(status => priorityOrder.IndexOf(status)).FirstOrDefault();


                    switch (highestPriorityStatus)
                    {
                        case ServiceStatusEnum.CabAwaitingRemovalConfirmation:
                            return ProviderStatusEnum.CabAwaitingRemovalConfirmation;
                        case ServiceStatusEnum.ReadyToPublish:
                            bool hasPublishedServices = services.Any(service => service.ServiceStatus == ServiceStatusEnum.Published);
                            return hasPublishedServices ? ProviderStatusEnum.PublishedActionRequired : ProviderStatusEnum.ActionRequired;
                        case ServiceStatusEnum.AwaitingRemovalConfirmation:
                            return ProviderStatusEnum.AwaitingRemovalConfirmation;
                        case ServiceStatusEnum.Published:
                            return ProviderStatusEnum.Published;
                        default:
                            return ProviderStatusEnum.AwaitingRemovalConfirmation;
                    }
                }



            }
            return providerStatus;
        }
    }
}
