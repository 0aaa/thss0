namespace Thss0.Web.Models.Entities
{
    public class Substance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
    }
}