using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic
{
    public  class ServiceHelper
    {       
        
        public static int GetServicePriority(ServiceDto service)
        {
            var certificateReview = service.CertificateReview.Where(s => s.IsLatestReviewVersion).SingleOrDefault();
            if (certificateReview != null)
            {
                if (certificateReview.CertificateReviewStatus == CertificateReviewEnum.AmendmentsRequired)
                    return 0;
                if (certificateReview.CertificateReviewStatus == CertificateReviewEnum.Rejected)
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
                ServiceStatusEnum.Published => 4,
                ServiceStatusEnum.Removed => 6,
                _ => 7
            };
        }

    }
}
