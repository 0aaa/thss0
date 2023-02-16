using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            // mdlBldr.Entity<Procedure>().HasData(prcdrsArr);
            // mdlBldr.Entity<Substance>().HasData(sbstncsArr);
            mdlBldr.Entity<Procedure>()
                    .HasMany(e => e.Substances)
                    .WithMany(e => e.Procedures)
                    .UsingEntity<Dictionary<string, object>>(
                        "Tutelage",
                        x => x.HasOne<Substance>().WithMany().OnDelete(DeleteBehavior.Restrict),
                        x => x.HasOne<Procedure>().WithMany().OnDelete(DeleteBehavior.Restrict)
                    )
                    .OnDelete(DeleteBehavior.Restrict);
            mdlBldr.Entity<Substance>()
                    .HasMany(e => e.Procedures)
                    .WithMany(e => e.Substances)
                    .UsingEntity<Dictionary<string, object>>(
                        "Tutelage",
                        x => x.HasOne<Procedure>().WithMany().OnDelete(DeleteBehavior.Restrict),
                        x => x.HasOne<Substance>().WithMany().OnDelete(DeleteBehavior.Restrict)
                    )
                    .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(mdlBldr);
        }
    }
}