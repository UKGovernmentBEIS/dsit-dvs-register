namespace DVSRegister.Models
{
    public class Filters
    {
        public List<int> SelectedRoleIds { get; set; }
        public List<int> SelectedSupplementarySchemeIds { get; set; }
        public List<int> SelectedTfVersionIds { get; set; }
        public bool FromDetailsPage { get; set; }
        public int RemoveRole { get; set; }
        public int RemoveScheme { get; set; }
        public int RemoveTfVersion { get; set; }
        public string SearchAction  { get; set; }
        public string SearchText { get; set; }
    }
}
