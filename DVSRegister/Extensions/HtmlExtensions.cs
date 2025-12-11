using DVSRegister.BusinessLogic.Models;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.AspNetCore.Html;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Web;

namespace DVSRegister.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString ToStringWithLineBreaks(string input)
        {
           
            string output = input?.Replace("\r", "<br>")??string.Empty;
            return new HtmlString(output);
        }

        public static HtmlString ToStringWithNewLineBreaks(string input)
        {

            string output = input?.Replace("\n", "<br>") ?? string.Empty;
            return new HtmlString(output);
        }
        private static string GetTagClass<TEnum>(TEnum value) where TEnum : struct, Enum
        {

            switch (value)
            {
                case ServiceStatusEnum.Submitted:
                case ServiceStatusEnum.Received:
                case ServiceStatusEnum.Resubmitted:                
                    return "govuk-tag govuk-tag--blue";

                case ServiceStatusEnum.Published:
                case CertificateReviewEnum.Approved:              
                case ServiceStatusEnum.PublishedUnderReassign:
                case PublicInterestCheckEnum.PublicInterestCheckPassed:
                case CertificateReviewEnum.DeclinedByProvider:
                    return "govuk-tag govuk-tag--green";

                case CertificateReviewEnum.Rejected:
                case CertificateReviewEnum.AmendmentsRequired:
                case ServiceStatusEnum.AmendmentsRequired:               
                case PublicInterestCheckEnum.PublicInterestCheckFailed:
                    return "govuk-tag govuk-tag--red";
                
                case ServiceStatusEnum.Removed:
                case ServiceStatusEnum.RemovedUnderReassign:
                    return "govuk-tag govuk-tag--grey";

                case ServiceStatusEnum.SavedAsDraft:
                case ServiceStatusEnum.AwaitingRemovalConfirmation:
                case ServiceStatusEnum.CabAwaitingRemovalConfirmation:
                case ServiceStatusEnum.UpdatesRequested:
                case ProviderStatusEnum.UpdatesRequested:
                    return "govuk-tag govuk-tag--yellow";

                default:
                    return string.Empty;
            }
        }

        public static HtmlString ToStyledStrongTag<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
        {

            string tagClass = GetTagClass(enumValue);
            string description = GetDescription(enumValue);

            StringBuilder sb = new StringBuilder();
            sb.Append("<strong class=\"");
            sb.Append(HttpUtility.HtmlEncode(tagClass));
            sb.Append("\">");
            sb.Append(HttpUtility.HtmlEncode(description));
            sb.Append("</strong>");
            return new HtmlString(sb.ToString());
        }


        public static HtmlString GetStyledStatusTag(CertificateReviewDto certificateReview,PublicInterestCheckDto publicInterestCheck, ServiceStatusEnum serviceStatus, ServiceStatusEnum? previousServiceStatus, bool isCabRequestedEdit)
        {

            bool isCurrentStatusSubmittedOrReceived = serviceStatus == ServiceStatusEnum.Submitted || serviceStatus == ServiceStatusEnum.Resubmitted ||
            serviceStatus == ServiceStatusEnum.Received;

            bool isPreviousStatusSubmittedOrReceived = previousServiceStatus!=null && (previousServiceStatus == ServiceStatusEnum.Submitted 
             || previousServiceStatus == ServiceStatusEnum.Resubmitted || previousServiceStatus == ServiceStatusEnum.Received);

            bool isCurrentStatusReceived = serviceStatus == ServiceStatusEnum.Received;

            bool isPreviousStatusReceived = previousServiceStatus!=null && previousServiceStatus == ServiceStatusEnum.Received;

           
             if ((isCurrentStatusReceived || isPreviousStatusReceived) 
                && publicInterestCheck != null &&   publicInterestCheck.PublicInterestCheckStatus == PublicInterestCheckEnum.PublicInterestCheckFailed)  // Passed PI check will be displayed as Published
            {
                return HtmlExtensions.ToStyledStrongTag(publicInterestCheck.PublicInterestCheckStatus);
            }
            else if ((isCurrentStatusSubmittedOrReceived || isPreviousStatusSubmittedOrReceived)
                    && certificateReview != null && (certificateReview.CertificateReviewStatus == CertificateReviewEnum.Approved || certificateReview.CertificateReviewStatus == CertificateReviewEnum.Rejected))
            {
                return HtmlExtensions.ToStyledStrongTag(certificateReview.CertificateReviewStatus);
            }
            else if (serviceStatus == ServiceStatusEnum.UpdatesRequested && isCabRequestedEdit == false)
            {
                return HtmlExtensions.ToStyledStrongTag(previousServiceStatus != null && previousServiceStatus > 0 ?
                (ServiceStatusEnum)previousServiceStatus
                : ServiceStatusEnum.Published);
            }
            else
            {
                return HtmlExtensions.ToStyledStrongTag(serviceStatus);
            }
           
        }
        private static string GetDescription<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            try
            {
                FieldInfo field = value.GetType().GetField(value.ToString());
                DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
                return attribute == null ? value.ToString() : attribute.Description;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception ex" + ex.Message);
                return string.Empty;
                
            }
        }

    }
}
