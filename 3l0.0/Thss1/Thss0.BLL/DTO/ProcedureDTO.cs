using System.Collections.Generic;

namespace Thss0.BLL.DTO
{
    public class ProcedureDTO : EntityDTO
    {
        public string Department { get; set; }
        public string Substances { get; set; }
        public string Result { get; set; }
        public string ClientName { get; set; }
        public HashSet<string> ProfessionalNames { get; set; }
        public ProcedureDTO() => ProfessionalNames = new HashSet<string>();
    }
}