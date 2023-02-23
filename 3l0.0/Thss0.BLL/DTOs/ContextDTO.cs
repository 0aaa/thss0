using Microsoft.EntityFrameworkCore;
using Thss0.DAL.Context;

namespace Thss0.BLL.DTOs
{
    public class ContextDTO : Thss0Context
    {
        public ContextDTO(DbContextOptions<Thss0Context> optns) : base(optns) { }
    }
}