using Thss0.DAL.Repositories;
using Thss0.DAL.Context;
using Thss0.BLL.DTOs;
using Microsoft.AspNetCore.Identity;
using Thss0.BLL.Services.Parents;
using Microsoft.EntityFrameworkCore;

namespace Thss0.BLL.Services
{
    public class ProceduresService : IdentityManager
    {
        private readonly GenericRepository<Procedure> _prcdrsRpstry;
        private readonly Thss0Context _cntxt;
        public ProceduresService()
        {
            _prcdrsRpstry = new GenericRepository<Procedure>();
            _cntxt = new Thss0Context(new DbContextOptions<Thss0Context>());
        }
        public async void Add(ProcedureDTO entty)
        {
            _prcdrsRpstry.Add(new Procedure
            {
                Name = entty.Name,
                Department = entty.Department,
                CreationTime = DateTime.Now,
                Result = entty.Result,
                Substances = (HashSet<Substance>)entty.Substances.Select(sbstnceNme => _cntxt.Substances.FirstOrDefault(sbstnce => sbstnce.Name == entty.Name)),
                Client = await SignInManager.UserManager.FindByNameAsync(entty.ClientName),
                Professionals = (HashSet<IdentityUser>)entty.ProfessionalNames.Select(async (prfsnlNme) => await SignInManager.UserManager.FindByNameAsync(prfsnlNme))
            });
        }

        public async void Delete(string id)
        {
            _prcdrsRpstry.Delete(await _prcdrsRpstry.Get(id));
        }

        public async Task<EntityDTO> Get(string id)
        {
            var procedureToGet = await _prcdrsRpstry.Get(id);
            return new ProcedureDTO
            {
                Id = procedureToGet.Id,
                Name = procedureToGet.Name,
                Department = procedureToGet.Department,
                Result = procedureToGet.Result,
                Substances = (HashSet<string>)procedureToGet.Substances.Select(sbstnce => sbstnce.Name),
                ClientName = procedureToGet.Client.UserName,
                ProfessionalNames = (HashSet<string>)procedureToGet.Professionals.Select(prfsnl => prfsnl.UserName)
            };
        }

        public async Task<IEnumerable<EntityDTO>> GetAll()
            => (await _prcdrsRpstry.GetAll()).Select(prcdre => new ProcedureDTO
                {
                    Id = prcdre.Id,
                    Name = prcdre.Name,
                    Department = prcdre.Department,
                    Result = prcdre.Result,
                    Substances = (HashSet<string>)prcdre.Substances.Select(sbstnce => sbstnce.Name),
                    ClientName = prcdre.Client.UserName,
                    ProfessionalNames = (HashSet<string>)prcdre.Professionals.Select(prfsnl => prfsnl.UserName)
            });
        public void Save()
            => _prcdrsRpstry.Save();
    }
}