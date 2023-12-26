namespace Thss0.Web.Models.ViewModels
{
    public class ResultViewModel : ViewModel
    {
        public string Content { get; set; } = string.Empty;

        public string User { get; set; } = string.Empty;
        public string Procedure { get; set; } = string.Empty;
        public string UserNames { get; set; } = string.Empty;
        public string ProcedureNames { get; set; } = string.Empty;
    }
}