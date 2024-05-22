using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Guard")]
    public class ViolationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ViolationController(ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment) 
        {
            _context = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult ViolationVerification()
        {

            return View();
        }
       
        public IActionResult Verification(DriverVerificationViewModel driver)
        {
            if (driver != null)
            {
                var obj = _context.Users.Where(x => x.orgNum == driver.OrgNumber).FirstOrDefault();
                if (obj != null)
                {
                    var Permits = _context.Permits.Where(x => x.regPlateNum == driver.RegistrationPlate && x.isGet == true).FirstOrDefault();
                    if (Permits != null)
                    {
                        var BlackListCheck = _context.BlackListDrivers.Where(x => x.RegPlate == driver.RegistrationPlate).FirstOrDefault();
                        if(BlackListCheck ==null)
                        {
                            var data = (from pr in _context.Permits
                                        join v in _context.Vehicles
                                        on pr.regPlateNum equals v.regPlateNum
                                        join D in _context.Users
                                        on pr.OrgNum equals D.orgNum
                                        where pr.regPlateNum == driver.RegistrationPlate && pr.OrgNum == driver.OrgNumber
                                        select new UserViewModel
                                        {
                                            orgNum = pr.OrgNum,
                                            FirstName = D.FirstName,
                                            LastName = D.LastName,
                                            Color = v.vehicleColor,
                                            Make = v.vehicleMake,
                                            Model = v.vehicleModel,
                                            idNumber = D.idNumber,
                                            UserRole = "Driver",
                                            PermitId = pr.QRCodeNumber,
                                            RegisterPlate = pr.regPlateNum,
                                        }).FirstOrDefault();

                            return PartialView("_ViolcationSucess", data);
                        }
                        else
                        {
                            return PartialView("_ViolationFailed");
                        }
                       
                    }
                    else
                    {
                        return PartialView("_ViolationFailed");
                    }

                }

            }
            return NotFound();
        }

        public IActionResult CreateViolation(long OrgNumber, string RegNumber)
        {

            ViewBag.OrgNumber = OrgNumber;
            ViewBag.RegNumbers = RegNumber;
            return View();
        }
        [HttpPost]
        public IActionResult CreateViolation(ViolationDTO violationDTO)
        {
            if(violationDTO !=null)
            {
                if (violationDTO.OffenseSelect == "Parking in other users reserved space")
                {
                    violationDTO.ServerityFee = 200;
                }
                else if (violationDTO.OffenseSelect == "Leaking oil on parking lot")
                {
                    violationDTO.ServerityFee = 200;
                }
                else if (violationDTO.OffenseSelect == "Speeding Class 1")
                {
                    violationDTO.ServerityFee = 400;
                }
                else if (violationDTO.OffenseSelect == "Parking in area not designated for parking")
                {
                    violationDTO.ServerityFee = 400;
                }
                else if (violationDTO.OffenseSelect == "Property damage")
                {
                    violationDTO.ServerityFee = 600;
                }
                else if (violationDTO.OffenseSelect == "Speeding Class 2")
                {
                    violationDTO.ServerityFee = 600;
                }
                else if (violationDTO.OffenseSelect == "Parking in restricted area")
                {
                    violationDTO.ServerityFee = 0;
                }
                else if (violationDTO.OffenseSelect == "Severe property damage")
                {
                    violationDTO.ServerityFee = 0;
                }
                else if (violationDTO.OffenseSelect == "Endangering of lives")
                {
                    violationDTO.ServerityFee = 0;
                }
                else if (violationDTO.OffenseSelect == "Speeding Class 3")
                {
                    violationDTO.ServerityFee = 0;
                }

                var Violation = new StudentViolation
                {
                    IssueDate = System.DateTime.Now,
                    AdditionInformation = violationDTO.AdditionInformation,
                    OffenseSelect = violationDTO.OffenseSelect,
                    OrgNumber = violationDTO.OrgNumber,
                    PartiesInvolved = violationDTO.PartiesInvolved,
                    RegPlate = violationDTO.RegPlate,
                    ServerityFee = violationDTO.ServerityFee,
                    Severity = violationDTO.Severity,
                    AppealSent = false,
                    isPaid = false,
                };
                _context.StudentViolations.Add(Violation);
                _context.SaveChanges();

                if(violationDTO.Listimages !=null)
                {
                    // save multi image 
                    foreach (var item in violationDTO.Listimages)
                    {
                        ViolationImages pro = new ViolationImages();
                        string dateTime2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string webRootPath2 = _hostEnvironment.WebRootPath;
                        string fileName2 = Path.GetFileNameWithoutExtension(item.FileName);
                        string extension2 = Path.GetExtension(item.FileName);
                        string uniqueFileName2 = fileName2 + "_" + dateTime2 + extension2;

                        pro.ViolationImage = uniqueFileName2;
                        string filePath2 = Path.Combine(webRootPath2, "ViolationImage", uniqueFileName2);

                        using (var fileStream = new FileStream(filePath2, FileMode.Create))
                        {
                            item.CopyTo(fileStream);
                        }
                        pro.StudentViolationId = Violation.Id;
                        _context.ViolationImages.Add(pro);
                        _context.SaveChanges();
                        return RedirectToAction("ViolationVerification");
                    }
                    return RedirectToAction("ViolationVerification");
                }
                return RedirectToAction("ViolationVerification");



            }

            return View(violationDTO);
        }
    }
}
