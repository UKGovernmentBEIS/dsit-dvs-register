using DVSRegister.Models.CAB;

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
        }
        #endregion




    }
}
