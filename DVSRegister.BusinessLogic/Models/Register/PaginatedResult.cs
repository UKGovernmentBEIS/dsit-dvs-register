namespace DVSRegister.BusinessLogic.Models
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
