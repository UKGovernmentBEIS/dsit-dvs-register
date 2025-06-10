using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CAB;
namespace DVSRegister.UnitTests.Helpers
{
    public static class TestDataFactory
    {
        public static ServiceDto CreateServiceDto(int serviceId, int cabId = 123, CertificateReviewDto review = null! )
        {
            return new ServiceDto
            {
                Id = serviceId,
                CertificateReview = review ?? CreateCertificateReviewDto(),
                CabUser = new CabUserDto { CabId = cabId }
            };
        }

        public static CertificateReviewDto CreateCertificateReviewDto(CertificateReviewEnum status = CertificateReviewEnum.InReview,string comments = null!
            ,string commentsForIncorrect = null!)
        {
            return new CertificateReviewDto
            {
                CertificateReviewStatus = status,
                Comments = comments,
                CommentsForIncorrect = commentsForIncorrect
            };
        }

        public static ServiceSummaryViewModel CreateServiceSummaryViewModel(int serviceId, bool isAmendment = true)
        {
            return new ServiceSummaryViewModel
            {
                ServiceId = serviceId,
                IsAmendment = isAmendment
            };
        }
    }
}