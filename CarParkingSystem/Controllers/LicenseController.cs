using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LicenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LicenseController(ApplicationDbContext applicationDb) {
            _context = applicationDb;
        }

        public IActionResult Index()
        {
            var obj = _context.Licenses.ToList();
            return View(obj);
        }
        public IActionResult AddLicense()
        {
            var users = (from user in _context.Users
                         join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                         join roles in _context.Roles on userRoles.RoleId equals roles.Id
                         where roles.Name == "Student"
                         select new { IdNumber = user.idNumber, DisplayName = $"{user.idNumber} - {user.FirstName} {user.LastName}" })
            .ToList();
            ViewBag.User = new SelectList(users, "IdNumber", "DisplayName");
          
            return View();
        }

        [HttpPost]
        public IActionResult AddLicense(UserLicense license)
        {
            if(license !=null)
            {
                _context.Licenses.Add(license);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(license);
        }

        public IActionResult EditLicense(int? id)
        {
            if(id !=null)
            {           
                var obj =  _context.Licenses.Find(id);
                if(obj !=null)
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
        public IActionResult EditLicense(UserLicense license)
        {
            if(license !=null)
            {
                _context.Licenses.Update(license);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(license);
            }
        }
        public IActionResult Delete(int? id)
        {
            if(id !=null)
            {
               var obj = _context.Licenses.Find(id);
                if(obj !=null)
                {
                    _context.Licenses.Remove(obj);
                   int check = _context.SaveChanges();
                    if (check == 1)
                    {
                        return Json(new { success = true, message = "User License deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error! License not deleted" });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Error! License not deleted" });
                }          
            }
            return Json(new { success = false, message = "Error! License not deleted" });
        }

        [HttpPost]
        public IActionResult GetName(long idNumber)
        {
            var getName = _context.Users
                .Where(x => x.idNumber == idNumber)
                .Select(x => new { FirstName = x.FirstName, LastName = x.LastName })
                .FirstOrDefault();

            if (getName != null)
            {
                return Json(getName);
            }
            else
            {
                return NotFound(new { error = "User not found." });
            }
        }

    }
}
