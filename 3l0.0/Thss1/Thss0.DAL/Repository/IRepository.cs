namespace Thss0.DAL.Repositories
{
    public interface IRepository<type>
    {
        Task<IEnumerable<type>> GetAll();
        Task<type> Get(string id);
        void Add(type entity);
        void Delete(type entity);
        void Save();
    }
}