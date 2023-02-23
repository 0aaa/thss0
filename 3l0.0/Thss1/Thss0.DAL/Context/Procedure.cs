using Microsoft.AspNetCore.Identity;

namespace Thss0.DAL.Context
{
    public class Procedure
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Substances { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? RealizationTime { get; set; }
        public DateTime? NextProcedureTime { get; set; }
        public string? Result { get; set; }
        public virtual IdentityUser Client { get; set; }
        public virtual HashSet<IdentityUser> Professionals { get; set; }
        public Procedure() => Professionals = new HashSet<IdentityUser>();
    }
}