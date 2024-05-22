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
    public class FaultController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fault
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Fault.Include(f => f.FaultReported);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Fault/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fault == null)
            {
                return NotFound();
            }

            var fault = await _context.Fault
                .Include(f => f.FaultReported)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fault == null)
            {
                return NotFound();
            }

            return View(fault);
        }

        // GET: Fault/Create
        public IActionResult Create()
        {
            ViewData["FaultReportedId"] = new SelectList(_context.FaultReported, "Id", "Description");
            return View();
        }

        // POST: Fault/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FaultReportedId,TechnicianId,DateAssigned,Status")] Fault fault)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fault);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FaultReportedId"] = new SelectList(_context.FaultReported, "Id", "Description", fault.FaultReportedId);
            return View(fault);
        }

        // GET: Fault/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fault == null)
            {
                return NotFound();
            }

            var fault = await _context.Fault.FindAsync(id);
            if (fault == null)
            {
                return NotFound();
            }
            ViewData["FaultReportedId"] = new SelectList(_context.FaultReported, "Id", "Description", fault.FaultReportedId);
            return View(fault);
        }

        // POST: Fault/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FaultReportedId,TechnicianId,DateAssigned,Status")] Fault fault)
        {
            if (id != fault.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fault);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaultExists(fault.Id))
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
            ViewData["FaultReportedId"] = new SelectList(_context.FaultReported, "Id", "Description", fault.FaultReportedId);
            return View(fault);
        }

        // GET: Fault/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fault == null)
            {
                return NotFound();
            }

            var fault = await _context.Fault
                .Include(f => f.FaultReported)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fault == null)
            {
                return NotFound();
            }

            return View(fault);
        }

        // POST: Fault/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fault == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Fault'  is null.");
            }
            var fault = await _context.Fault.FindAsync(id);
            if (fault != null)
            {
                _context.Fault.Remove(fault);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaultExists(int id)
        {
          return (_context.Fault?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
