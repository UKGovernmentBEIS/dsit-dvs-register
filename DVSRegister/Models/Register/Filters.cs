namespace DVSRegister.Models
{
    public class Filters
    {
        public List<int> SelectedRoleIds { get; set; }
        public List<int> SelectedSupplementarySchemeIds { get; set; }
        public bool FromDeatilsPage { get; set; }
        public int RemoveRole { get; set; }
        public int RemoveScheme { get; set; }
        public string SearchAction  { get; set; }
        public string SearchProvider { get; set; }
    }
}
