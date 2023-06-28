namespace Thss0.Web.Models.ViewModels.CRUD
{
    public class ProcedureViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string CreationTime { get; set; } = "";
        public string RealizationTime { get; set; } = "";
        public string NextProcedureTime { get; set; } = "";

        public string Department { get; set; } = "";
        public string User { get; set; } = "";
        public string Result { get; set; } = "";
        public string Substance { get; set; } = "";
        public string DepartmentNames { get; set; } = "";
        public string UserNames { get; set; } = "";
        public string ResultNames { get; set; } = "";
        public string SubstanceNames { get; set; } = "";
    }
}