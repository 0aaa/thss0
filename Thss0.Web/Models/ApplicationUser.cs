using Microsoft.AspNetCore.Identity;

namespace Thss0.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual HashSet<Procedure> Procedures { get; set; } = new HashSet<Procedure>();
    }
}