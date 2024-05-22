using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;

namespace CarParkingSystem.Controllers
{
    public class TechnicianReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TechnicianReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TechnicianReport
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TechnicianReport.Include(t => t.Fault);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TechnicianReport/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TechnicianReport == null)
            {
                return NotFound();
            }

            var technicianReport = await _context.TechnicianReport
                .Include(t => t.Fault)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (technicianReport == null)
            {
                return NotFound();
            }

            return View(technicianReport);
        }

        // GET: TechnicianReport/Create
        public IActionResult Create()
        {
            ViewData["FaultId"] = new SelectList(_context.Fault, "Id", "TechnicianId");
            return View();
        }

        // POST: TechnicianReport/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FaultId,Report,DateCreated")] TechnicianReport technicianReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(technicianReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FaultId"] = new SelectList(_context.Fault, "Id", "TechnicianId", technicianReport.FaultId);
            return View(technicianReport);
        }

        // GET: TechnicianReport/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TechnicianReport == null)
            {
                return NotFound();
            }

            var technicianReport = await _context.TechnicianReport.FindAsync(id);
            if (technicianReport == null)
            {
                return NotFound();
            }
            ViewData["FaultId"] = new SelectList(_context.Fault, "Id", "TechnicianId", technicianReport.FaultId);
            return View(technicianReport);
        }

        // POST: TechnicianReport/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FaultId,Report,DateCreated")] TechnicianReport technicianReport)
        {
            if (id != technicianReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(technicianReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TechnicianReportExists(technicianReport.Id))
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
            ViewData["FaultId"] = new SelectList(_context.Fault, "Id", "TechnicianId", technicianReport.FaultId);
            return View(technicianReport);
        }

        // GET: TechnicianReport/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TechnicianReport == null)
            {
                return NotFound();
            }

            var technicianReport = await _context.TechnicianReport
                .Include(t => t.Fault)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (technicianReport == null)
            {
                return NotFound();
            }

            return View(technicianReport);
        }

        // POST: TechnicianReport/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TechnicianReport == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TechnicianReport'  is null.");
            }
            var technicianReport = await _context.TechnicianReport.FindAsync(id);
            if (technicianReport != null)
            {
                _context.TechnicianReport.Remove(technicianReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TechnicianReportExists(int id)
        {
            return (_context.TechnicianReport?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
