using Microsoft.AspNetCore.Identity;

namespace Thss0.Web.Models.Entities
{
    public class ApplicationUser : IdentityUser, IEntity
    {
        public string Name { get => base.UserName; set => base.UserName = value; }
        public DateTime DoB { get; set; } = DateTime.Today;
        public string PoB { get; set; } = string.Empty;
        public byte[] Photo { get; set; } = Array.Empty<byte>();
        public virtual Department? Department { get; set; }
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
        public virtual HashSet<Result> Result { get; set; } = new();
    }
}