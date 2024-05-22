using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Guard")]
    public class BlackListController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BlackListController( ApplicationDbContext dbContext)
        {
          _context = dbContext;
        }
        public IActionResult Index()
        {
            var obj = _context.BlackListDrivers.ToList();
            return View(obj);
        }
        public IActionResult AddDriver()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddDriver(BlackListDriver blackList)
        {
            if( blackList != null)
            {
                var obj = _context.Vehicles.Where(x => x.regPlateNum == blackList.RegPlate).FirstOrDefault();
                if (obj != null)
                {
                    _context.BlackListDrivers.Add(blackList);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"This Register number{blackList.RegPlate} is not avaible in our system");
                    return View(blackList);
                }
            }
            return View(blackList);
        }
        public IActionResult DriverDetails(int id)
        {
            var obj = _context.BlackListDrivers.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return View();
        }
        public IActionResult BlackListRemove(int id)
        {
            var obj = _context.BlackListDrivers.Find(id);
            if (obj != null)
            {
                return View(obj);
            }
            return View();
        }

        public IActionResult ConfirmRemove(int id)
        {
            var obj = _context.BlackListDrivers.Find(id);
            if(obj !=null)
            {
                _context.BlackListDrivers.Remove(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("BlackListRemove", new { id = id });
        }
    }
}
