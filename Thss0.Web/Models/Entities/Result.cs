using System.ComponentModel.DataAnnotations;

namespace Thss0.Web.Models.Entities
{
    public class Result : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public DateTime ObtainmentTime { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Result required")]
        public string Content { get; set; } = "";
        public virtual Procedure? Procedure { get; set; }
        public virtual HashSet<ApplicationUser> User { get; set; } = [];
    }
}