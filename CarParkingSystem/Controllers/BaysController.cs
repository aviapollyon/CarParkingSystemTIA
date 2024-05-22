using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaysController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index()
        {
              return _context.Bays != null ? 
                          View(await _context.Bays.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Bays'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Bay bay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bay);
                await _context.SaveChangesAsync();

                ParkingBay parkingBay = new ParkingBay
                {
                    BayId = bay.SectionLetter + " " + bay.BayNumber,
                    BayNumber = bay.BayNumber,
                    BaySection = bay.SectionLetter,
                    Occupacy = "Unoccupied"
                };
                _context.ParkingBays.Add(parkingBay);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(bay);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bays == null)
            {
                return NotFound();
            }

            var bay = await _context.Bays.FindAsync(id);
            if (bay == null)
            {
                return NotFound();
            }
            return View(bay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bay bay)
        {
            if (id != bay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BayExists(bay.Id))
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
            return View(bay);
        }

        public async Task<IActionResult> Delete(int id)
        {
            
            var bay = await _context.Bays.FindAsync(id);
            if (bay != null)
            {
                _context.Bays.Remove(bay);
                await _context.SaveChangesAsync();
            }
            var parkingbay = _context.ParkingBays.Where(x => x.BaySection == bay.SectionLetter && x.BayNumber == bay.BayNumber).FirstOrDefault();
            _context.ParkingBays.Remove(parkingbay);
          
            int check = _context.SaveChanges();
            if (check == 1)
            {
                return Json(new { success = true, message = "Bay deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error! Bay not deleted" });
            }
           
        }

        private bool BayExists(int id)
        {
          return (_context.Bays?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
