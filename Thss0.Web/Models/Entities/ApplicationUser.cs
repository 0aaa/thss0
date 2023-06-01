using Microsoft.AspNetCore.Identity;

namespace Thss0.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime DoB { get; set; } = DateTime.Today;
        public string PoB { get; set; } = "";
        public virtual Department? Department { get; set; }
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
        public virtual HashSet<Result> Result { get; set; } = new();
    }
}