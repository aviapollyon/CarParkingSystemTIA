using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using CarParkingSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AddUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AddUserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> signIn)
        {
            _context = dbContext;
            _userManager = signIn;
        }
        public IActionResult UserList()
        {
            var obj = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name !="Admin"
                       select new UserViewModel
                       {
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Email = user.Email,
                           UserRole = roles.Name,
                           userId = user.Id,
                           IsAccountActive = user.IsAccountActive,
                           idNumber =user.idNumber,
                           orgNum = user.orgNum,
                           Phone = user.PhoneNumber,
                       }).ToList();
            return View(obj);
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(AddUserDTO addUser)
        {

            var getIdNumber = _context.Users.Where(x => x.idNumber == addUser.idNumber).FirstOrDefault();
            if(getIdNumber == null)
            {
                var getOrgNumber = _context.Users.Where(x => x.orgNum == addUser.orgNum).FirstOrDefault();
                if(getOrgNumber == null)            
                {
                    var user = new ApplicationUser
                    {
                        Email = addUser.Email,
                        UserName = addUser.Email,
                        FirstName = addUser.FirstName,
                        LastName = addUser.LastName,
                        IsAccountActive = false,
                        idNumber = addUser.idNumber,
                        orgNum = addUser.orgNum,
                        PhoneNumber = addUser.Phone,
                        EmailConfirmed = true,
                    };
                    var result = await _userManager.CreateAsync(user, addUser.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, addUser.UserRole);
                        return RedirectToAction("UserList");

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password must contain at least 6 characters, including uppercase, lowercase, and special characters.");
                        return View(addUser);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"The Org Number {addUser.orgNum} is already available in our system.");
                    return View(addUser);
                }
                
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"The ID number {addUser.idNumber} is already available in our system. Please check your ID number.");
                return View(addUser);
            }                
        }
        public IActionResult DeleteUser(string id)
        {
            var getData = _context.Users.Find(id);
            if (getData != null)
            {
                _context.Users.Remove(getData);
                _context.SaveChanges();
                return Json(new { success = true, message = "User deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error! User did not delete. please try again" });

            }
        }

    }
}
