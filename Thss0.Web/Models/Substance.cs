namespace Thss0.Web.Models
{
    public class Substance
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public virtual HashSet<Procedure> Procedures { get; set; } = new HashSet<Procedure>();
    }
}