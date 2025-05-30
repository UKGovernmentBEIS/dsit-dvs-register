using DVSRegister.BusinessLogic.Models.CAB;
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
                var priorityOrder = new List<ServiceStatusEnum>
                    {
                        ServiceStatusEnum.CabAwaitingRemovalConfirmation,
                        ServiceStatusEnum.ReadyToPublish,
                        ServiceStatusEnum.UpdatesRequested,
                        ServiceStatusEnum.AwaitingRemovalConfirmation,
                        ServiceStatusEnum.PublishedUnderReassign,
                        ServiceStatusEnum.Published,
                        ServiceStatusEnum.RemovedUnderReassign,
                        ServiceStatusEnum.Removed
                    };

                ServiceStatusEnum highestPriorityStatus = services
                  .Where(service => service.ServiceStatus == ServiceStatusEnum.ReadyToPublish ||
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
                    case ServiceStatusEnum.ReadyToPublish:
                        bool hasPublishedServices = services.Any(service => service.ServiceStatus == ServiceStatusEnum.Published);
                        return hasPublishedServices ? ProviderStatusEnum.ReadyToPublishNext : ProviderStatusEnum.ReadyToPublish;
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
        
        public static int GetServicePriority(ServiceDto service)
        {
            if (service.CertificateReview != null)
            {
                if (service.CertificateReview.CertificateReviewStatus == CertificateReviewEnum.AmendmentsRequired)
                    return 0;
                if (service.CertificateReview.CertificateReviewStatus == CertificateReviewEnum.Rejected)
                    return 5;
            }

            return service.ServiceStatus switch
            {
                ServiceStatusEnum.AmendmentsRequired => 0,
                ServiceStatusEnum.UpdatesRequested => 0,
                ServiceStatusEnum.SavedAsDraft => 1,
                ServiceStatusEnum.CabAwaitingRemovalConfirmation => 2,
                ServiceStatusEnum.AwaitingRemovalConfirmation => 2,
                ServiceStatusEnum.Submitted or
                    ServiceStatusEnum.Received or
                    ServiceStatusEnum.Resubmitted or
                    ServiceStatusEnum.ReadyToPublish => 3,
                ServiceStatusEnum.Published => 4,
                ServiceStatusEnum.Removed => 6,
                _ => 7
            };
        }

    }
}
