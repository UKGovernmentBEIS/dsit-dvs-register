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
                case ServiceStatusEnum.Published:
                    return "govuk-tag govuk-tag--blue";

                case CertificateReviewEnum.Approved:
                    return "govuk-tag govuk-tag--green";

                case ServiceStatusEnum.Removed:
                case CertificateReviewEnum.Rejected:
                    return "govuk-tag govuk-tag--red";

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
        private static string GetDescription<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
