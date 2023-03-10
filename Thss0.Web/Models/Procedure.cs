namespace Thss0.Web.Models
{
    public class Procedure
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime RealizationTime { get; set; } = DateTime.MinValue;
        public DateTime NextProcedureTime { get; set; } = DateTime.MinValue;
        public string Result { get; set; } = "";
        public virtual HashSet<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
        public virtual HashSet<Substance> Substances { get; set; } = new HashSet<Substance>();
    }
}