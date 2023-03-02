using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Models;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProceduresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProceduresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procedure>>> GetProcedures()
        {
            return await _context.Procedures.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Procedure>> GetProcedure(string id)
        {
            var procedure = await _context.Procedures.FindAsync(id);

            if (procedure == null)
            {
                return NotFound();
            }

            return procedure;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcedure(string id, ProcedureViewModel procedure)
        {
            if (id != procedure.Id)
            {
                return BadRequest();
            }
            var procedureToUpdate = new Procedure{
                Id = procedure.Id,
                Name = procedure.Name,
                Department = procedure.Department,
                RealizationTime = procedure.RealizationTime,
                NextProcedureTime = procedure.NextProcedureTime,
                Result = procedure.Result
            };
            _context.Entry(procedureToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcedureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Procedure>> PostProcedure(ProcedureViewModel procedure)// To edit.
        {
            var procedureToAdd = new Procedure
            {
                Id = Guid.NewGuid().ToString(),
                Name = procedure.Name,
                Department = procedure.Department,
                CreationTime = DateTime.Now,
                RealizationTime = procedure.RealizationTime,
                NextProcedureTime = procedure.NextProcedureTime,
                Result = procedure.Result,
                Users = new HashSet<ApplicationUser> { new ApplicationUser { UserName = procedure.Users } },
                Substances = new HashSet<Substance> { new Substance { Id = Guid.NewGuid().ToString(), Name = procedure.Substances } }
            };
            _context.Procedures.Add(procedureToAdd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProcedureExists(procedureToAdd.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProcedure", new { id = procedureToAdd.Id }, _context.Procedures.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcedure(string id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }

            _context.Procedures.Remove(procedure);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception excptn)
            {
                Console.WriteLine(excptn.Message);
            }

            return NoContent();
        }

        private bool ProcedureExists(string id)
        {
            return _context.Procedures.Any(e => e.Id == id);
        }
    }
}