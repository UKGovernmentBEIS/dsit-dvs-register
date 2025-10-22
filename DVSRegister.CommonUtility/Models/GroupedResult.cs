namespace DVSRegister.CommonUtility.Models
{
    public class GroupedResult<T>
    { 
        public DateTime LogDate { get; set; }
        public List<T> Items { get; set; }

    }
}
