namespace Thss0.DAL.Context
{
    public class Substance
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual HashSet<Procedure> Procedures { get; set; }
        public Substance()
        {
            Id = "";
            Name = "";
        }
    }
}