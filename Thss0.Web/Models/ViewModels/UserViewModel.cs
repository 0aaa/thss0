namespace Thss0.Web.Models.ViewModels
{
    public class UserViewModel : ViewModel
    {
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DoB { get; set; } = string.Empty;
        public string PoB { get; set; } = string.Empty;
        public sbyte[]? Photo { get; set; } = Array.Empty<sbyte>();
        public string Role { get; set; } = string.Empty;

        public string Procedure { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string ProcedureNames { get; set; } = string.Empty;
        public string ResultNames { get; set; } = string.Empty;
    }
}