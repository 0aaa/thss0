namespace Thss0.Web.Models.ViewModels
{
    public class ProcedureViewModel : ViewModel
    {
        public string CreationTime { get; set; } = "";
        public string BeginTime { get; set; } = "";
        public string EndTime { get; set; } = "";

        public string NextProcedureId { get; set; } = "";
        public string Department { get; set; } = "";
        public string Professional { get; set; } = "";
        public string Client { get; set; } = "";
        public string Result { get; set; } = "";
        public string Substance { get; set; } = "";
        public string DepartmentNames { get; set; } = "";
        public string ProfessionalNames { get; set; } = "";
        public string ClientNames { get; set; } = "";
        public string ResultNames { get; set; } = "";
        public string SubstanceNames { get; set; } = "";
    }
}