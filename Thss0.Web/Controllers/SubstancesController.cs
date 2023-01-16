using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Models;

namespace Thss0.Web.Controllers
{
    public class SubstancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubstancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Substances
        public async Task<IActionResult> Index()
        {
              return View(await _context.Substances.ToListAsync());
        }

        // GET: Substances/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Substances == null)
            {
                return NotFound();
            }

            var substance = await _context.Substances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (substance == null)
            {
                return NotFound();
            }

            return View(substance);
        }

        // GET: Substances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Substances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Substance substance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(substance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(substance);
        }

        // GET: Substances/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Substances == null)
            {
                return NotFound();
            }

            var substance = await _context.Substances.FindAsync(id);
            if (substance == null)
            {
                return NotFound();
            }
            return View(substance);
        }

        // POST: Substances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] Substance substance)
        {
            if (id != substance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(substance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubstanceExists(substance.Id))
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
            return View(substance);
        }

        // GET: Substances/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Substances == null)
            {
                return NotFound();
            }

            var substance = await _context.Substances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (substance == null)
            {
                return NotFound();
            }

            return View(substance);
        }

        // POST: Substances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Substances == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Substances'  is null.");
            }
            var substance = await _context.Substances.FindAsync(id);
            if (substance != null)
            {
                _context.Substances.Remove(substance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubstanceExists(string id)
        {
          return _context.Substances.Any(e => e.Id == id);
        }
    }
}
