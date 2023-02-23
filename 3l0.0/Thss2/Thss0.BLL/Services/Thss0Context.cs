using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thss0.BLL.Services.Parents;
using Thss0.DAL.Context;

namespace Thss0.BLL.Services
{
    public class Thss0Context : IdentityDbContext<IdentityUser>
    {
        public virtual DbSet<Procedure> Procedures { get; set; }
        public virtual DbSet<Substance> Substances { get; set; }
        public Thss0Context(DbContextOptions<Thss0Context> optns) : base(optns)
            => Database.EnsureCreated();
        /*protected override void OnModelCreating(ModelBuilder mdlBldr)
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
        }*/
    }
}