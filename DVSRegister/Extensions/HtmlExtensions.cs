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
                case ServiceStatusEnum.ReadyToPublish:
                case ServiceStatusEnum.PublishedUnderReassign:
                case PublicInterestCheckEnum.PublicInterestCheckPassed:
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
                case CertificateReviewEnum.InReview:
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
            var test = sb.ToString();
            return new HtmlString(sb.ToString());
        }


        public static HtmlString GetStyledStatusTag(CertificateReviewDto certificateReview,PublicInterestCheckDto publicInterestCheck, ServiceStatusEnum serviceStatus, ServiceStatusEnum? previousServiceStatus)
        {
            // Check for Certificate Review whilst public interest has not become complete
            if (certificateReview != null && publicInterestCheck == null)
            {
                return HtmlExtensions.ToStyledStrongTag(certificateReview.CertificateReviewStatus);
            }
            // Check for publicInterestCheck whilst service is not ready to publish
            else if (publicInterestCheck != null && (serviceStatus < ServiceStatusEnum.ReadyToPublish || serviceStatus == ServiceStatusEnum.Resubmitted))
            {
                if (publicInterestCheck.PublicInterestCheckStatus == PublicInterestCheckEnum.PublicInterestCheckFailed)
                {
                    return HtmlExtensions.ToStyledStrongTag(publicInterestCheck.PublicInterestCheckStatus);
                }
                return HtmlExtensions.ToStyledStrongTag(PublicInterestCheckEnum.PublicInterestCheckPassed);
            }
            // Check if it is being edited and display the status before updates
            else if (previousServiceStatus > 0 && serviceStatus == ServiceStatusEnum.UpdatesRequested)
            {
                return HtmlExtensions.ToStyledStrongTag((ServiceStatusEnum)previousServiceStatus);               
            }
            // Default to displaying the actual ServiceStatus
            return HtmlExtensions.ToStyledStrongTag(serviceStatus);
        }
        private static string GetDescription<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

    }
}
