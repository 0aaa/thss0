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
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder mdlBldr)
        {
            var testValuesArr = new[] { "test" };
            var prcdrsArr = new Procedure[testValuesArr.Length];
            var sbstncsArr = new Substance[testValuesArr.Length];
            for (ushort i = 0; i < testValuesArr.Length; i++)
            {
                prcdrsArr[i] = new Procedure
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = testValuesArr[i],
                    Department = testValuesArr[i],
                    CreationTime = DateTime.Now,
                    RealizationTime = DateTime.Now,
                    NextProcedureTime = DateTime.Now,
                    Result = testValuesArr[i]
                };
                sbstncsArr[i] = new Substance
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = testValuesArr[i]
                };
            }
            mdlBldr.Entity<Procedure>().HasData(prcdrsArr);
            mdlBldr.Entity<Substance>().HasData(sbstncsArr);
            base.OnModelCreating(mdlBldr);
        }
        public DbSet<Thss0.Web.Models.User> User { get; set; }
    }
}