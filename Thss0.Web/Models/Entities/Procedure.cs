using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thss0.Web.Models.Entities
{
    public class Procedure
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Name required")]
        [StringLength(128, ErrorMessage = "Wrong name length", MinimumLength = 2)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Department required")]
        [StringLength(64, ErrorMessage = "Wrong department length", MinimumLength = 2)]
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime RealizationTime { get; set; } = default;
        public DateTime NextProcedureTime { get; set; } = default;

        [ForeignKey("ResultId")]
        public virtual Result? Result { get; set; }
        public virtual Department? Department { get; set; }
        public virtual HashSet<ApplicationUser> User { get; set; } = new();
        public virtual HashSet<Substance> Substance { get; set; } = new();
    }
}