using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;

namespace DVSRegister.Models
{
    public static class ViewModelHelper
    {

        public static DateViewModel GetDayMonthYear(DateTime? dateTime)
        {
            DateViewModel dateViewModel = new();
            DateTime conformityIssueDate = Convert.ToDateTime(dateTime);
            dateViewModel.Day = conformityIssueDate.Day;
            dateViewModel.Month = conformityIssueDate.Month;
            dateViewModel.Year = conformityIssueDate.Year;
            return dateViewModel;
        }

        #region Clear selected data in session if selection changed from yes to no
        public static void ClearGpg44(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.QualityLevelViewModel != null)
            {
                summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators = [];
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = [];
            }

        }
        public static void ClearGpg45(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.IdentityProfileViewModel != null)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = [];          
        }

        public static void ClearSchemes(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.SupplementarySchemeViewModel != null)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = [];
            if (summaryViewModel.SchemeIdentityProfileMapping != null)
                summaryViewModel.SchemeIdentityProfileMapping = [];
            if (summaryViewModel.SchemeQualityLevelMapping != null)
                summaryViewModel.SchemeQualityLevelMapping = [];
        }

        public static void ClearSchemeGpg44(ServiceSummaryViewModel summaryViewModel, int shcemeId)
        {
            if (summaryViewModel.SchemeQualityLevelMapping != null && summaryViewModel.SchemeQualityLevelMapping.Count>0)
            {
                var schemeQualityLevelMapping = summaryViewModel.SchemeQualityLevelMapping.Where(x => x.SchemeId == shcemeId).FirstOrDefault();
                if(schemeQualityLevelMapping!=null && schemeQualityLevelMapping.HasGPG44 == false)
                {
                    schemeQualityLevelMapping.QualityLevel.SelectedQualityofAuthenticators = [];
                    schemeQualityLevelMapping.QualityLevel.SelectedLevelOfProtections = [];
                }
            }

        }
        #endregion




    }
}
