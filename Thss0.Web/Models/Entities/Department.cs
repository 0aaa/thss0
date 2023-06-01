using System.ComponentModel.DataAnnotations;

namespace Thss0.Web.Models
{
    public class Department
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Name required")]
        [StringLength(64, ErrorMessage = "Wrong name length", MinimumLength = 2)]
        public string Name { get; set; } = "";
        public virtual HashSet<ApplicationUser> User { get; set; } = new();
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
    }
}