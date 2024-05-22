using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarParkingSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public LoginController( ApplicationDbContext context, UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = manager;
            _signInManager = signInManager;
        }
        public async Task< IActionResult> Login()
        {
            var LoginUser = await _userManager.GetUserAsync(User);
            if(LoginUser !=null)
            {
                var roles = await _userManager.GetRolesAsync(LoginUser);
                if (LoginUser == null)
                {
                    return View();
                }
                else if (roles.Contains("Admin"))
                {
                    return RedirectToAction("ActiveParkingPermit", "AdminDashboard");
                }
                else if (roles.Contains("Student"))
                {
                    return RedirectToAction("PurchasePermit", "StudentDashboard");
                }
                else if (roles.Contains("Guard"))
                {
                    return RedirectToAction("ParkingHistory", "GuardDashboard");
                }

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (login != null)
            {
                var remember = true;
                var obj = _context.Users.Where(x => x.orgNum == login.OrgNumer).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.IsAccountActive == true)
                    {
                        var result = await _signInManager.PasswordSignInAsync(obj.Email, login.Password, remember, lockoutOnFailure: false);

                        if(result.Succeeded)
                        {
                            var user = await _userManager.FindByEmailAsync(obj.Email);
                            var roles = await _userManager.GetRolesAsync(user);
                            if (roles.Contains("Admin"))
                            {
                                return RedirectToAction("ActiveParkingPermit", "AdminDashboard");
                            }
                            else if (roles.Contains("Student"))
                            {
                                return RedirectToAction("PurchasePermit", "StudentDashboard");
                            }
                            else if(roles.Contains("Guard"))
                            {
                                return RedirectToAction("ParkingHistory", "GuardDashboard");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            return View(login);
                        }
                    }
                    else
                    {
                        return RedirectToAction("ActiveAccount");
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Your record is not available in our system.");
                    return View(login);
                }
            }
            return View(login);
        }

        public IActionResult ActiveAccount()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ActiveAccount(ActiveDTO active)
        {
            if (active != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.orgNum == active.OrgNumber && x.idNumber == active.IdNumber);
                if (user != null)
                {
                    user.IsAccountActive = true; 
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, active.Password);
                    if (resetPasswordResult.Succeeded)
                    {
                        return RedirectToAction("Login"); 
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error: User password did not reset.");
                        return View(active);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(active);
                }

            }
            return View(active);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
