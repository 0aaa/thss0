namespace Thss0.Web.Models.ViewModels
{
    public class ProcedureViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime RealizationTime { get; set; } = DateTime.MinValue;
        public DateTime NextProcedureTime { get; set; } = DateTime.MinValue;
        public string Result { get; set; } = "";
        public string Users { get; set; } = "";
        public string Substances { get; set; } = "";
    }
}