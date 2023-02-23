using Microsoft.EntityFrameworkCore;
using Thss0.DAL.Context;

namespace Thss0.DAL.Repositories
{
    public class GenericRepository<type> : IRepository<type> where type : class
    {
        private DbContext _cntxt;
        private DbSet<type> _st;
        public GenericRepository()
        {
            //_cntxt = new Thss0Context(new DbContextOptions<Thss0Context>());
            _st = _cntxt.Set<type>();
        }
        public async Task<IEnumerable<type>> GetAll() => await _st.ToListAsync();
        public async Task<type> Get(string id) => await _st.FindAsync(id) ?? throw new NullReferenceException();
        public async void Add(type entity) => await _st.AddAsync(entity);
        public void Delete(type entity) => _st.Remove(entity);
        public async void Save() => await _cntxt.SaveChangesAsync();
    }
}