using Thss0.BLL.DTOs;
using Thss0.DAL.Context;
using Thss0.DAL.Repositories;

namespace Thss0.BLL.Services
{
    internal class SubstancesService
    {
        private readonly GenericRepository<Substance> _sbstncsRpstry;
        public SubstancesService()
        {
            _sbstncsRpstry = new GenericRepository<Substance>();
        }
        public void Add(SubstanceDTO entty)
        {
            _sbstncsRpstry.Add(new Substance { Name = entty.Name });
        }

        public async void Delete(string id)
        {
            _sbstncsRpstry.Delete(await _sbstncsRpstry.Get(id));
        }

        public async Task<EntityDTO> Get(string id)
        {
            var substanceToGet = await _sbstncsRpstry.Get(id);
            return new SubstanceDTO
            {
                Id = substanceToGet.Id,
                Name = substanceToGet.Name,
                Procedures = (HashSet<string>)substanceToGet.Procedures.Select(prcdre => prcdre.Name)
            };
        }

        public async Task<IEnumerable<EntityDTO>> GetAll()
            => (await _sbstncsRpstry.GetAll()).Select(sbstnce => new SubstanceDTO
            {
                Id = sbstnce.Id,
                Name = sbstnce.Name,
                Procedures = (HashSet<string>)sbstnce.Procedures.Select(prcdre => prcdre.Name)
            });
        public void Save()
            => _sbstncsRpstry.Save();
    }
}