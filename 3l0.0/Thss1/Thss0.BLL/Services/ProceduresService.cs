using Thss0.DAL.Repositories;
using Thss0.DAL.Context;
using Thss0.BLL.DTO;
using Microsoft.AspNetCore.Identity;
using Thss0.BLL.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Thss0.BLL.Services
{
    public class ProceduresService : IService
    {
        private readonly GenericRepository<Procedure> _prcdrsRpstry;
        private readonly UserManager<IdentityUser> _usrMngr;
        public ProceduresService(UserManager<IdentityUser> usrMngr, DbContextOptions<Thss0Context> optns)
        {
            _prcdrsRpstry = new GenericRepository<Procedure>(optns);
            _usrMngr = usrMngr;
        }
        public async void Add(ProcedureDTO entity)
        {
            _prcdrsRpstry.Add(new Procedure
            {
                Name = entity.Name,
                Department = entity.Department,
                Substances = entity.Substances,
                CreationTime = DateTime.Now,
                Result = entity.Result,
                Client = await _usrMngr.FindByNameAsync(entity.ClientName),
                Professionals = (HashSet<IdentityUser>)entity.ProfessionalNames.Select(async (prfsnlNme) => await _usrMngr.FindByNameAsync(prfsnlNme))
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
                Substances = procedureToGet.Substances,
                Result = procedureToGet.Result,
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
                    Substances = prcdre.Substances,
                    Result = prcdre.Result,
                    ClientName = prcdre.Client.UserName,
                    ProfessionalNames = (HashSet<string>)prcdre.Professionals.Select(prfsnl => prfsnl.UserName)
            });
        public void Save()
            => _prcdrsRpstry.Save();
    }
}