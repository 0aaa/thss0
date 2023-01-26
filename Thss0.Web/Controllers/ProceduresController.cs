using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Models;

namespace Thss0.Web.Controllers
{
    //[Authorize]
    public class ProceduresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProceduresController(ApplicationDbContext context)
            => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Procedures.ToListAsync());

        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Procedures == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        public IActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Department,CreationTime,RealizationTime,NextProcedureTime,Result")] Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                procedure.Id = Guid.NewGuid().ToString();
                _context.Add(procedure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(procedure);
        }

        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Procedures == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }
            return View(procedure);
        }

        //[Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Department,CreationTime,RealizationTime,NextProcedureTime,Result")] Procedure procedure)
        {
            if (id != procedure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procedure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcedureExists(procedure.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(procedure);
        }

        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Procedures == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        //[Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Procedures == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Procedures'  is null.");
            }
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure != null)
            {
                _context.Procedures.Remove(procedure);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureExists(string id)
            => _context.Procedures.Any(e => e.Id == id);
    }
}