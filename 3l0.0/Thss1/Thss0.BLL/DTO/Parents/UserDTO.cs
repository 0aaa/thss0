namespace Thss0.BLL.DTO
{
    public class UserDTO : EntityDTO
    {
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}