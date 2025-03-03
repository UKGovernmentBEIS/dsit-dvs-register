namespace DVSRegister.BusinessLogic.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Profile { get; set; }
    }
}
