namespace Thss0.BLL.DTOs
{
    internal class SubstanceDTO : EntityDTO
    {
        public HashSet<string> Procedures { get; set; }
        public SubstanceDTO() => Procedures = new HashSet<string>();
    }
}