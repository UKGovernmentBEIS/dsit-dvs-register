namespace DVSRegister.Models.UI
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Sort { get; set; }
        public string? SortAction { get; set; }
        public string? SortBy { get; set; }
        public string? SearchText { get; set; }
        public List<int>? SelectedRoleIds { get; set; }
        public List<int>? SelectedSupplementarySchemeIds { get; set; }
        public List<int>? SelectedTfVersionIds { get; set; }
        public string Action { get; set; }
    }
}
