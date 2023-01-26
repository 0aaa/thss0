using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models;

namespace Thss0.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Substance> Substances { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
                => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder mdlBldr)
        {
            var tstVlsArr = new[] { "test" };
            var prcdrsArr = new Procedure[tstVlsArr.Length];
            var sbstncsArr = new Substance[tstVlsArr.Length];
            for (ushort i = 0; i < tstVlsArr.Length; i++)
            {
                prcdrsArr[i] = new Procedure
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tstVlsArr[i],
                    Department = tstVlsArr[i],
                    CreationTime = DateTime.Now,
                    RealizationTime = DateTime.Now,
                    NextProcedureTime = DateTime.Now,
                    Result = tstVlsArr[i]
                };
                sbstncsArr[i] = new Substance
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tstVlsArr[i]
                };
            }
            mdlBldr.Entity<Procedure>().HasData(prcdrsArr);
            mdlBldr.Entity<Substance>().HasData(sbstncsArr);
            base.OnModelCreating(mdlBldr);
        }
    }
}