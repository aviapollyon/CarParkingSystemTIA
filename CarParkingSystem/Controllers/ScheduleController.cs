using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using CarParkingSystem.ViewModel.GuardSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ScheduleController(ApplicationDbContext context, UserManager<ApplicationUser> manager ) 
        {
            _context = context;
            _userManager = manager;
        }
        public IActionResult Index()
        {
            var obj = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name == "Guard"
                       select new GuardScheduleViewModel
                       {
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           OrgNumber = user.orgNum,
                       }).ToList();
            return View(obj);
        }

        public IActionResult SetSchedule(long OrgNumber)
        {
            var obj = _context.GuardSchedules.Where(x => x.OrgNumber == OrgNumber).FirstOrDefault();

            var getUerData = _context.Users.Where(x => x.orgNum == OrgNumber).FirstOrDefault();
            ViewBag.OrgNumber = OrgNumber;
            ViewBag.FirstName = getUerData?.FirstName;
            ViewBag.LastName = getUerData?.LastName;
            if (obj != null)
            {
                ViewBag.Status = obj.Status;
                ViewBag.Id = obj.Id;
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult SetSchedule(GuardSchedule guardSchedule)
        {
            if(guardSchedule.Id > 0)
            {
                var obj = _context.GuardSchedules.Find(guardSchedule.Id);
                if(obj !=null)
                {
                    _context.GuardSchedules.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                guardSchedule.Status = "Available";
                _context.GuardSchedules.Add(guardSchedule);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guardSchedule);
        }
        public IActionResult ViewSchedule()
        {
            var obj = _context.GuardSchedules.ToList();
            return View(obj);

        }
    }
}
