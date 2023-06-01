using System.ComponentModel.DataAnnotations;

namespace Thss0.Web.Models
{
    public class Substance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
    }
}