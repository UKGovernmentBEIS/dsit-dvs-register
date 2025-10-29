namespace DVSRegister.BusinessLogic.Models
{
    public class ActionDetailsDto
    {
    
        public int Id { get; set; }
        public string ActionDetailsKey { get; set; }
        public string ActionDescription { get; set; }     
        public int ActionCategoryId { get; set; }
        public ActionCategoryDto ActionCategory { get; set; }        

    }
}
