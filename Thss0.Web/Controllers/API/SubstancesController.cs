using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    public class SubstancesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubstancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Substance>>> GetSubstances()
        {
            return await _context.Substances.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Substance>> GetSubstance(string id)
        {
            var substance = await _context.Substances.FindAsync(id);

            if (substance == null)
            {
                return NotFound();
            }

            return substance;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubstance(string id, SubstanceViewModel substance)
        {
            if (id != substance.Id)
            {
                return BadRequest();
            }
            var substanceToUpdate = new Substance
            {
                Id = substance.Id,
                Name = substance.Name
            };
            _context.Entry(substanceToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubstanceExists(id))
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
        public async Task<ActionResult<Substance>> PostSubstance(SubstanceViewModel substance)
        {
            var substanceToAdd = new Substance
            {
                Id = Guid.NewGuid().ToString(),
                Name = substance.Name,
                Procedures = new HashSet<Procedure> { new Procedure() { Id = Guid.NewGuid().ToString(), Name = substance.Procedure } }
            };
            _context.Substances.Add(substanceToAdd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubstanceExists(substanceToAdd.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubstance", new { id = substanceToAdd.Id }, _context.Substances.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubstance(string id)
        {
            var substance = await _context.Substances.FindAsync(id);
            if (substance == null)
            {
                return NotFound();
            }

            _context.Substances.Remove(substance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubstanceExists(string id)
        {
            return _context.Substances.Any(e => e.Id == id);
        }
    }
}