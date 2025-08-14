using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public  class RepositoryHelper
    {
        public static ProviderStatusEnum GetProviderStatus(ICollection<Service> services, ProviderStatusEnum currentStatus)
        {
            ProviderStatusEnum providerStatus = currentStatus;
            if (services != null && services.Count > 0)
            {
                var priorityOrder = new List<ServiceStatusEnum>
                    {
                        ServiceStatusEnum.CabAwaitingRemovalConfirmation,                      
                        ServiceStatusEnum.UpdatesRequested,
                        ServiceStatusEnum.AwaitingRemovalConfirmation,
                        ServiceStatusEnum.PublishedUnderReassign,
                        ServiceStatusEnum.Published,
                        ServiceStatusEnum.RemovedUnderReassign,
                        ServiceStatusEnum.Removed
                    };

                ServiceStatusEnum highestPriorityStatus = services
                  .Where(service => 
                    service.ServiceStatus == ServiceStatusEnum.Published ||
                    service.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign ||
                    service.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign ||
                    service.ServiceStatus == ServiceStatusEnum.Removed ||
                    service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation ||
                    service.ServiceStatus == ServiceStatusEnum.UpdatesRequested ||
                    service.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation)
                    .Select(service => service.ServiceStatus)
                    .OrderBy(status => priorityOrder.IndexOf(status))
                    .FirstOrDefault();



                switch (highestPriorityStatus)
                {
                    case ServiceStatusEnum.CabAwaitingRemovalConfirmation:
                        return ProviderStatusEnum.CabAwaitingRemovalConfirmation;                 
                    case ServiceStatusEnum.UpdatesRequested:
                        return ProviderStatusEnum.UpdatesRequested;
                    case ServiceStatusEnum.AwaitingRemovalConfirmation:
                        return ProviderStatusEnum.AwaitingRemovalConfirmation;
                    case ServiceStatusEnum.Published:
                        return ProviderStatusEnum.Published;
                    case ServiceStatusEnum.PublishedUnderReassign:
                        return ProviderStatusEnum.PublishedUnderReassign;
                    case ServiceStatusEnum.RemovedUnderReassign:
                        return ProviderStatusEnum.RemovedUnderReassign;
                    case ServiceStatusEnum.Removed:
                        return ProviderStatusEnum.RemovedFromRegister;
                    default:
                        return ProviderStatusEnum.AwaitingRemovalConfirmation;
                }

            }
            return providerStatus;
        }
    }
}
