using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models;

namespace Thss0.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
                => Database.EnsureCreated();
    }
}