namespace Thss0.BLL.DTOs
{
    public class ProcedureDTO : EntityDTO
    {
        public string Department { get; set; }
        public string Result { get; set; }
        public string ClientName { get; set; }
        public HashSet<string> Substances { get; set; }
        public HashSet<string> ProfessionalNames { get; set; }
        public ProcedureDTO()
        {
            Substances = new HashSet<string>();
            ProfessionalNames = new HashSet<string>();
        }
    }
}