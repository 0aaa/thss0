using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net;
using Thss0.Web.Data;
using Thss0.Web.Models;

namespace Thss0.Web.Controllers
{
    //[Authorize]
    public class SubstancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubstancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //ApiHandling();
            return View(await _context.Substances.ToListAsync());
        }

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

        public IActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Substance substance)
        {
            if (ModelState.IsValid)
            {
                substance.Id = Guid.NewGuid().ToString();
                _context.Add(substance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(substance);
        }

        //[Authorize(Roles = "admin")]
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

        //[Authorize(Roles = "admin")]
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

        //[Authorize(Roles = "admin")]
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

        //[Authorize(Roles = "admin")]
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
            => _context.Substances.Any(e => e.Id == id);

        private async void ApiHandling()
        {
            ushort sbstncsQntty = 5;
            var rqstStr = "http://www.vidal.ru/api/rest/v1/product/list";
            var rsltStr = await new HttpClient().GetStringAsync(rqstStr);
            var rsltJsn = JObject.Parse(rsltStr);
            var sbstncs = new List<Substance>();
            for (ushort i = 0; i < sbstncsQntty; i++)
            {
                sbstncs.Add(new Substance
                {
                    Id = rsltJsn["products"][i]["id"].ToString(),
                    Name = rsltJsn["products"][i]["engName"].ToString(),
                });
            }
        }
    }
}