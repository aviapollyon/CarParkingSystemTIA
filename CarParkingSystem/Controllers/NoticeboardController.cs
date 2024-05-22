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
    public class NoticeboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoticeboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Noticeboard
        public async Task<IActionResult> Index()
        {
              return _context.Noticeboard != null ? 
                          View(await _context.Noticeboard.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Noticeboard'  is null.");
        }

        // GET: Noticeboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Noticeboard == null)
            {
                return NotFound();
            }

            var noticeboard = await _context.Noticeboard
                .FirstOrDefaultAsync(m => m.Id == id);
            if (noticeboard == null)
            {
                return NotFound();
            }

            return View(noticeboard);
        }

        // GET: Noticeboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Noticeboard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,DateCreated")] Noticeboard noticeboard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(noticeboard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(noticeboard);
        }

        // GET: Noticeboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Noticeboard == null)
            {
                return NotFound();
            }

            var noticeboard = await _context.Noticeboard.FindAsync(id);
            if (noticeboard == null)
            {
                return NotFound();
            }
            return View(noticeboard);
        }

        // POST: Noticeboard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,DateCreated")] Noticeboard noticeboard)
        {
            if (id != noticeboard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(noticeboard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticeboardExists(noticeboard.Id))
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
            return View(noticeboard);
        }

        // GET: Noticeboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Noticeboard == null)
            {
                return NotFound();
            }

            var noticeboard = await _context.Noticeboard
                .FirstOrDefaultAsync(m => m.Id == id);
            if (noticeboard == null)
            {
                return NotFound();
            }

            return View(noticeboard);
        }

        // POST: Noticeboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Noticeboard == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Noticeboard'  is null.");
            }
            var noticeboard = await _context.Noticeboard.FindAsync(id);
            if (noticeboard != null)
            {
                _context.Noticeboard.Remove(noticeboard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoticeboardExists(int id)
        {
          return (_context.Noticeboard?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
