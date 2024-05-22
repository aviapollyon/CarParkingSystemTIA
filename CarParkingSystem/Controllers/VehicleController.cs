using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VehicleController(ApplicationDbContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var obj = _context.Vehicles.ToList();
            return View(obj);
        }
        public IActionResult AddVehicle()
        {
            var users = (from user in _context.Users
                         join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                         join roles in _context.Roles on userRoles.RoleId equals roles.Id
                         where roles.Name == "Student"
                         select new { IdNumber = user.idNumber, DisplayName = $"{user.idNumber} - {user.FirstName} {user.LastName}" }).ToList();
            ViewBag.User = new SelectList(users, "IdNumber", "DisplayName");
            return View();
        }
        [HttpPost]
        public IActionResult AddVehicle(Vehicle vehicle)
        {
            if(vehicle !=null)
            {
                _context.Vehicles.Add(vehicle);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }    
            return View(vehicle);
        }
        public IActionResult EditVehicle(int? id)
        {
            if (id != null)
            {
                var obj = _context.Vehicles.Find(id);
                if (obj != null)
                {
                    var users = (from user in _context.Users
                                 join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                                 join roles in _context.Roles on userRoles.RoleId equals roles.Id
                                 where roles.Name == "Student"
                                 select new { IdNumber = user.idNumber, DisplayName = $"{user.idNumber} - {user.FirstName} {user.LastName}" }).ToList();
                    ViewBag.User = new SelectList(users, "IdNumber", "DisplayName", obj.idNumber);
                    return View(obj);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult EditVehicle(Vehicle vehicle)
        {
            if(vehicle !=null)
            {
                _context.Vehicles.Update(vehicle);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(vehicle);
            }
        }

        public IActionResult DeleteVehicle(int? id)
        {
            if (id != null)
            {
                var obj = _context.Vehicles.Find(id);
                if (obj != null)
                {
                    _context.Vehicles.Remove(obj);
                    int check = _context.SaveChanges();
                    if (check == 1)
                    {
                        return Json(new { success = true, message = "User Vehicle deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error! Vehicle not deleted" });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Error! Vehicle not deleted" });
                }
            }
            return Json(new { success = false, message = "Error! Vehicle not deleted" });
        }
    }
}
