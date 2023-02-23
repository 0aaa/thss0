using Microsoft.EntityFrameworkCore;
using Thss0.DAL.Context;

namespace Thss0.BLL.DTO
{
    public class Thss0ContextDTO : Thss0Context
    {
        public Thss0ContextDTO(DbContextOptions<Thss0Context> optns) : base(optns) { }
    }
}