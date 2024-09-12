using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thss0.Web.Models.Entities
{
    public class Procedure : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Name required")]
        [StringLength(256, ErrorMessage = "Wrong name length", MinimumLength = 2)]
        public string Name { get; set; } = "";

        [StringLength(64, ErrorMessage = "Wrong department length", MinimumLength = 2)]
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime BeginTime { get; set; } = default;
        public DateTime EndTime { get; set; } = default;

        public string NextProcedureId { get; set; } = "";
        [ForeignKey("ResultId")]
        public virtual Result? Result { get; set; }
        [Required(ErrorMessage = "Department required")]
        public virtual Department? Department { get; set; }
        public virtual HashSet<ApplicationUser> User { get; set; } = [];
        public virtual HashSet<Substance> Substance { get; set; } = [];
    }
}