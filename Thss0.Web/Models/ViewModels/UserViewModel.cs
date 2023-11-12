namespace Thss0.Web.Models.ViewModels
{
    public class UserViewModel : ViewModel
    {
        public string Password { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string DoB { get; set; } = "";
        public string PoB { get; set; } = "";
        public byte[] Photo { get; set; } = Array.Empty<byte>();
        public string Role { get; set; } = "";

        public string Procedure { get; set; } = "";
        public string Result { get; set; } = "";
        public string ProcedureNames { get; set; } = "";
        public string ResultNames { get; set; } = "";
    }
}