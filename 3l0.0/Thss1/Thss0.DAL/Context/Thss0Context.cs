using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Thss0.DAL.Context
{
    public class Thss0Context : DbContext
    {
        public virtual DbSet<Procedure> Procedures { get; set; }
        public Thss0Context(DbContextOptions<Thss0Context> optns) : base(optns)
            => Database.EnsureCreated();
        protected override void OnModelCreating(ModelBuilder mdlBldr)
        {
            base.OnModelCreating(mdlBldr);
        }
    }
}