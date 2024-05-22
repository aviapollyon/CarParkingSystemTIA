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
    public class FaultReportedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaultReportedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FaultReporteds
        public async Task<IActionResult> Index()
        {
              return _context.FaultReported != null ? 
                          View(await _context.FaultReported.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.FaultReported'  is null.");
        }

        // GET: FaultReporteds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FaultReported == null)
            {
                return NotFound();
            }

            var faultReported = await _context.FaultReported
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faultReported == null)
            {
                return NotFound();
            }

            return View(faultReported);
        }

        // GET: FaultReporteds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FaultReporteds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,DateReported,Status,StudentId")] FaultReported faultReported)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faultReported);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faultReported);
        }

        // GET: FaultReporteds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FaultReported == null)
            {
                return NotFound();
            }

            var faultReported = await _context.FaultReported.FindAsync(id);
            if (faultReported == null)
            {
                return NotFound();
            }
            return View(faultReported);
        }

        // POST: FaultReporteds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,DateReported,Status,StudentId")] FaultReported faultReported)
        {
            if (id != faultReported.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faultReported);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaultReportedExists(faultReported.Id))
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
            return View(faultReported);
        }

        // GET: FaultReporteds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FaultReported == null)
            {
                return NotFound();
            }

            var faultReported = await _context.FaultReported
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faultReported == null)
            {
                return NotFound();
            }

            return View(faultReported);
        }

        // POST: FaultReporteds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FaultReported == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FaultReported'  is null.");
            }
            var faultReported = await _context.FaultReported.FindAsync(id);
            if (faultReported != null)
            {
                _context.FaultReported.Remove(faultReported);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaultReportedExists(int id)
        {
          return (_context.FaultReported?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
