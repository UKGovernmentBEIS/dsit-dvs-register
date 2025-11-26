namespace DVSRegister.Models.UI
{
    public class PaginationParameters
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentSortAction { get; set; }
    }
}
