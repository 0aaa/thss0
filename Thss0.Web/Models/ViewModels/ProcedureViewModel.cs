namespace Thss0.Web.Models.ViewModels
{
    public class ProcedureViewModel : ViewModel
    {
        public string CreationTime { get; set; } = string.Empty;
        public string BeginTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

        public string NextProcedureId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Substance { get; set; } = string.Empty;
        public string DepartmentNames { get; set; } = string.Empty;
        public string UserNames { get; set; } = string.Empty;
        public string ResultNames { get; set; } = string.Empty;
        public string SubstanceNames { get; set; } = string.Empty;
    }
}