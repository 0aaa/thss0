using Microsoft.EntityFrameworkCore;
using Thss0.BLL.Services;
using Thss0.DAL.Context;

namespace Thss0.BLL.DTOs
{
    public class ContextDTO : Thss0Context
    {
        public ContextDTO() : base(new DbContextOptions<Thss0Context>()) { }
    }
}