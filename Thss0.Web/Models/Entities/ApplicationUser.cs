using Microsoft.AspNetCore.Identity;

namespace Thss0.Web.Models.Entities
{
    public class ApplicationUser : IdentityUser, IEntity
    {
        public string Name { get => base.UserName ?? ""; set => base.UserName = value; }
        public string DoB { get; set; } = DateTime.Today.ToShortDateString();
        public string PoB { get; set; } = "";
        public byte[] Photo { get; set; } = [];
        public virtual Department? Department { get; set; }
        public virtual HashSet<Procedure> Procedure { get; set; } = [];
        public virtual HashSet<Result> Result { get; set; } = [];
    }
}