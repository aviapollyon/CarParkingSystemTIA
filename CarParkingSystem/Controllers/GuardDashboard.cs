using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using CarParkingSystem.ViewModel.GuardSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Asn1.X509;
using QRCoder;
using System.Collections;
using System.Drawing;
using System.Security.Cryptography;

namespace CarParkingSystem.Controllers
{
    public class GuardDashboard : Controller
    {
        private readonly ApplicationDbContext _context;
        private string? error;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        public GuardDashboard(ApplicationDbContext dbContext, IWebHostEnvironment environment, UserManager<ApplicationUser> manager)
        {
            _context = dbContext;
            _environment = environment;
            _userManager = manager;
        }
        [Authorize(Roles = "Guard")]
        public IActionResult ParkingHistory()
        {
            var obj = (from checkin in _context.CheckIns
                       join
                       checkout in _context.checkOuts on
                       checkin.OrgNum equals checkout.OrgNum
                       join
                       u in _context.Users on checkin.OrgNum equals u.orgNum
                       select new HistoryViewModel
                       {
                           BayId = checkin.bayID,
                           CheckIn = checkin.CheckInTime,
                           CheckOut = checkout.CheckInTime,
                           FullName = u.FirstName + " " + u.LastName,
                           OrgNumber = u.orgNum
                       }).ToList();

            return View(obj);
        }
        [Authorize(Roles = "Guard")]
        public IActionResult ActiveReservation()
        {
            var obj = _context.Reservations.ToList();
            return View(obj);
        }
        [Authorize(Roles = "Guard")]
        public IActionResult Availability()
        {
            var obj = _context.Bays.ToList();
            var parkingbay = _context.ParkingBays.ToList();
            ReserveViewModel reserveView = new ReserveViewModel
            {
                ParkingBays = parkingbay,
                Bays = obj,
            };
            return View(reserveView);
        }
        [Authorize(Roles = "Guard")]
        public IActionResult Permitverification()
        {
            return View();
        }
        [Authorize(Roles = "Guard")]
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
                        var blacklist = _context.BlackListDrivers.Where(x => x.RegPlate == driver.RegistrationPlate).FirstOrDefault();
                        if (blacklist == null)
                        {
                            var MediumCount = _context.StudentViolations.Where(x => x.Severity == "Medium" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                            if (MediumCount == 2)
                            {
                                return Json(new { error = "Your Account has been restricted. Pay your Violation." });
                            }
                            var SevereCount = _context.StudentViolations.Where(x => x.Severity == "Severe" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                            if (SevereCount == 1)
                            {
                                return Json(new { error = "Your Account has been restricted. Pay your Violation." });

                            }
                            var HighCount = _context.StudentViolations.Where(x => x.Severity == "High" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                            if (HighCount == 1)
                            {
                                return Json(new { error = "Your Account has been restricted. Pay your Violation." });

                            }
                            var LowCount = _context.StudentViolations.Where(x => x.Severity == "Low" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                            if (LowCount == 4)
                            {
                                return Json(new { error = "Your Account has been restricted. Pay your Violation." });
                            }
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

                            return PartialView("_PermitVerification", data);
                        }
                        else
                        {
                            return Json(new { error = "Driver does not have an active parking permit or is blacklisted!" });
                        }

                    }
                    else
                    {
                        return Json(error = "Driver does not have an active parking permit!");
                    }

                }

            }
            return NotFound();
        }
        [Authorize(Roles = "Guard")]
        public IActionResult ReservationVerified(string id)
        {
            if (id != null)
            {
                var verification = _context.Reservations.Where(x => x.regPlateNum == id).FirstOrDefault();
                if (verification != null)
                {
                    if (verification.ExpDate.Date == System.DateTime.Today.Date)
                    {
                        var parkingbay = _context.ParkingBays.Where(x => x.BayId == verification.BayId).FirstOrDefault();
                        parkingbay.Occupacy = "Unoccupied";
                        _context.ParkingBays.Update(parkingbay);
                        _context.SaveChanges();

                        //Remove user Reservation because it's expired
                        _context.Reservations.Remove(verification);
                        _context.SaveChanges();
                        return RedirectToAction("ReservationExpired");
                    }
                    else
                    {
                        var obj = _context.Bays.ToList();
                        var parkingbay = _context.ParkingBays.ToList();
                        var userReservations = _context.Reservations
                                .Where(x => x.BayId == verification.BayId)
                                .ToList();

                        ReserveViewModel reserveView = new ReserveViewModel
                        {
                            ParkingBays = parkingbay,
                            Bays = obj,
                            regNumber = verification.regPlateNum,
                            UserReservations = userReservations,
                        };
                        return View(reserveView);
                    }
                }
                else
                {
                    return RedirectToAction("ReservationUnverified", new { id = id });
                }

            }
            else
            {
                return NotFound();
            }

        }

        public IActionResult ReservationExpired()
        {
            return View();
        }
        [Authorize(Roles = "Guard")]
        public IActionResult ReservationUnverified(string? id)
        {
            var obj = _context.Bays.ToList();
            var parkingbay = _context.ParkingBays.ToList();
            var reservation = _context.Reservations.ToList();
            ReserveViewModel reserveView = new ReserveViewModel
            {
                ParkingBays = parkingbay,
                Bays = obj,
                regNumber = id,
                Reservations = reservation,
            };
            return View(reserveView);
        }

        public IActionResult AllocateConfirm(string RegNumber)
        {
            var obj = _context.Reservations.Where(x => x.regPlateNum == RegNumber).FirstOrDefault();
            if (obj != null)
            {
                CheckIn checkIn = new CheckIn
                {
                    bayID = obj.BayId,
                    CheckInTime = System.DateTime.Now,
                    OrgNum = obj.OrgNum,
                    regPlateNum = obj.regPlateNum,
                    CheckInBay = "Reservation",
                };
                _context.CheckIns.Add(checkIn);
                _context.SaveChanges();
                return View();
            }
            return NotFound();

        }
        [Authorize(Roles = "Guard")]

        public IActionResult GuardParkingBay(string regNum, string bayId)
        {
            var obj = _context.Vehicles.Where(x => x.regPlateNum == regNum).FirstOrDefault();
            if (obj != null)
            {
                var getOrgNumber = _context.Users.Where(x => x.idNumber == obj.idNumber).FirstOrDefault();
                if (getOrgNumber != null)
                {
                    //update in parking bay
                    var parkingbay = _context.ParkingBays.Where(x => x.BayId == bayId).FirstOrDefault();
                    parkingbay.Occupacy = "Occupied";
                    _context.ParkingBays.Update(parkingbay);
                    _context.SaveChanges();

                    // Add checkin also 
                    CheckIn checkIn = new CheckIn
                    {
                        bayID = bayId,
                        CheckInTime = System.DateTime.Now,
                        OrgNum = getOrgNumber.orgNum,
                        regPlateNum = regNum,
                        CheckInBay = "NotReservation"
                    };
                    _context.CheckIns.Add(checkIn);
                    _context.SaveChanges();
                    return RedirectToAction("GuardAllocateConfirm");
                }
            }
            return View();
        }

        public IActionResult GuardAllocateConfirm()
        {
            return View();
        }
        [Authorize(Roles = "Guard")]
        public IActionResult DeallocateBay()
        {
            return View();
        }
        public IActionResult DeallocateBayVerifaction(DriverVerificationViewModel driver)
        {
            if (driver != null)
            {
                var obj = _context.Users.Where(x => x.orgNum == driver.OrgNumber).FirstOrDefault();
                if (obj != null)
                {
                    var Permits = _context.Permits.Where(x => x.regPlateNum == driver.RegistrationPlate && x.isGet == true).FirstOrDefault();
                    if (Permits != null)
                    {
                        var checkin = _context.CheckIns.Where(x => x.regPlateNum == driver.RegistrationPlate && x.OrgNum == driver.OrgNumber).FirstOrDefault();
                        if (checkin != null)
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
                                            BayId = checkin.bayID,
                                            CheckInBay = checkin.CheckInBay,
                                        }).FirstOrDefault();

                            return PartialView("_DeallocateFailureSuccss", data);
                        }
                        else
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

                            return PartialView("_DeallocateFailure", data);
                        }

                    }
                    else
                    {
                        return Json(error = "Driver does not have an active parking permit!");
                    }

                }

            }
            return NotFound();
        }

        public IActionResult DeallocateBayVerifactionQR(DriverVerificationViewModel driver)
        {
            if (driver != null)
            {
                var obj = _context.Users.Where(x => x.orgNum == driver.OrgNumber).FirstOrDefault();
                if (obj != null)
                {
                    var Permits = _context.Permits.Where(x => x.regPlateNum == driver.RegistrationPlate && x.isGet == true).FirstOrDefault();
                    if (Permits != null)
                    {
                        var checkin = _context.CheckIns.Where(x => x.regPlateNum == driver.RegistrationPlate && x.OrgNum == driver.OrgNumber).FirstOrDefault();
                        if (checkin != null)
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
                                            BayId = checkin.bayID,
                                            CheckInBay = checkin.CheckInBay,
                                        }).FirstOrDefault();

                            return View(data);
                        }
                        else
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
                            return RedirectToAction("DeallocateFailureQRCode", new { OrgNumber = driver.OrgNumber, RegistrationPlate = driver.RegistrationPlate });
                        }

                    }
                    else
                    {
                        return RedirectToAction("DeallocateQRPermit");
                    }
                }

            }
            return NotFound();
        }

        public IActionResult DeallocateFailureQRCode(long OrgNumber, string RegistrationPlate)
        {
            if (RegistrationPlate != null)
            {
                var obj = _context.Users.Where(x => x.orgNum == OrgNumber).FirstOrDefault();
                if (obj != null)
                {
                    var Permits = _context.Permits.Where(x => x.regPlateNum == RegistrationPlate && x.isGet == true).FirstOrDefault();
                    if (Permits != null)
                    {
                        var checkin = _context.CheckIns.Where(x => x.regPlateNum == RegistrationPlate && x.OrgNum == OrgNumber).FirstOrDefault();
                        if (checkin != null)
                        {
                            var data = (from pr in _context.Permits
                                        join v in _context.Vehicles
                                        on pr.regPlateNum equals v.regPlateNum
                                        join D in _context.Users
                                        on pr.OrgNum equals D.orgNum
                                        where pr.regPlateNum == RegistrationPlate && pr.OrgNum == OrgNumber
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
                                            BayId = checkin.bayID
                                        }).FirstOrDefault();

                            return View(data);
                        }
                        else
                        {
                            var data = (from pr in _context.Permits
                                        join v in _context.Vehicles
                                        on pr.regPlateNum equals v.regPlateNum
                                        join D in _context.Users
                                        on pr.OrgNum equals D.orgNum
                                        where pr.regPlateNum == RegistrationPlate && pr.OrgNum == OrgNumber
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

                            return View(data);
                        }

                    }
                    else
                    {
                        return RedirectToAction("DeallocateQRPermit");
                    }

                }

            }
            return NotFound();
        }

        public IActionResult DeallocateQRPermit()
        {
            return View();
        }

        public IActionResult DeallocationSuccess(string BayId, long OrgNumber, string RegNumber, string CheckInBay)
        {
            var obj = _context.ParkingBays.Where(x => x.BayId == BayId).FirstOrDefault();
            if (obj != null)
            {
                if (CheckInBay == "Reservation")
                {
                    CheckOut checkOut = new CheckOut
                    {
                        bayID = BayId,
                        CheckInTime = System.DateTime.Now,
                        OrgNum = OrgNumber,
                        regPlateNum = RegNumber,
                    };
                    _context.checkOuts.Add(checkOut);
                    _context.SaveChanges();
                    ViewBag.Text = "Parking Bay has been deallocated successfully";
                    return View();
                }
                else
                {
                    // this user has not reservation so we need only update the parking bay status and information store in the checkout
                    obj.Occupacy = "Unoccupied";
                    _context.ParkingBays.Update(obj);
                    _context.SaveChanges();

                    CheckOut checkOut = new CheckOut
                    {
                        bayID = BayId,
                        CheckInTime = System.DateTime.Now,
                        OrgNum = OrgNumber,
                        regPlateNum = RegNumber,
                    };
                    _context.checkOuts.Add(checkOut);
                    _context.SaveChanges();
                    ViewBag.Text = "Parking Bay has been deallocated successfully and is now unoccupied";
                    return View();
                }


            }
            return NotFound();
        }

        [Authorize(Roles = "Guard")]
        public IActionResult Visitation()
        {
            var obj = _context.Visitations.ToList();
            return View(obj);
        }
        [Authorize(Roles = "Guard")]
        public IActionResult AddVisitation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddVisitation(Visitation visitation)
        {
            if (visitation != null)
            {
                var verification = _context.BlackListDrivers.Where(x => x.RegPlate == visitation.RegistrationPlate).FirstOrDefault();
                if (verification == null)
                {
                    _context.Visitations.Add(visitation);
                    _context.SaveChanges();

                    var obj = _context.ParkingBays.Where(x => x.BayId == visitation.ParkingBay).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.Occupacy = "Occupied";
                        _context.ParkingBays.Update(obj);
                        _context.SaveChanges();
                        return RedirectToAction("Visitation");
                    }
                    return RedirectToAction("Visitation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This driver has been blacklisted");
                    return View(visitation);
                }

            }
            else
            {
                return View(visitation);
            }

        }
        [Authorize(Roles = "Guard")]
        public IActionResult VisitationDetails(int id)
        {
            var obj = _context.Visitations.Find(id);
            if (obj != null)
            {
                return View(obj);
            }
            return View();
        }
        [Authorize(Roles = "Guard")]
        public IActionResult VisitationUpdate(int id)
        {
            var obj = _context.Visitations.Find(id);
            if (obj != null)
            {
                obj.ExitTime = System.DateTime.Now.ToString();
                _context.Visitations.Update(obj);
                _context.SaveChanges();

                //update the status

                var getdata = _context.ParkingBays.Where(x => x.BayId == obj.ParkingBay).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.Occupacy = "Unoccupied";
                    _context.ParkingBays.Update(getdata);
                    _context.SaveChanges();
                    return RedirectToAction("Visitation");
                }
            }
            return RedirectToAction("Visitation");
        }
        [Authorize(Roles = "Guard")]
        public IActionResult StudentPermit()
        {
            var permits = _context.Permits.ToList();
            foreach (var permit in permits)
            {
                //string trackingUrl = $"{Request.Scheme}://{Request.Host}/Guard/PermitTracking/{permit.QRCodeNumber}";
                string trackingUrl = $"{Request.Scheme}://{Request.Host}/Guard/permitVerificationQrCode/OrgNumber/{permit.OrgNum}/RegistrationPlate/{permit.QRCodeNumber}";
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(trackingUrl, QRCodeGenerator.ECCLevel.Q);

                int pixelsPerModule = 20;
                int width = qrCodeData.ModuleMatrix.Count > 0 ? qrCodeData.ModuleMatrix[0].Count * pixelsPerModule : 0;
                int height = qrCodeData.ModuleMatrix.Count * pixelsPerModule;
                Bitmap qrCodeImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                {
                    graphics.Clear(Color.White);
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
                        {
                            BitArray row = qrCodeData.ModuleMatrix[x];
                            for (int y = 0; y < row.Count; y++)
                            {
                                if (row[y])
                                {
                                    graphics.FillRectangle(brush, y * pixelsPerModule, x * pixelsPerModule, pixelsPerModule, pixelsPerModule);
                                }
                            }
                        }
                    }
                }

                int resizedWidth = 150;  // Set the desired width
                int resizedHeight = 150; // Set the desired height
                string image;
                using (Bitmap resizedImage = new Bitmap(qrCodeImage, resizedWidth, resizedHeight))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        string base64Image = Convert.ToBase64String(byteImage);
                        string imgSrc = $"data:image/png;base64,{base64Image}";
                        image = imgSrc;
                    }
                }

                string fileName = $"qrcode_{permit.QRCodeNumber}.png";
                string filePath = Path.Combine(_environment.WebRootPath, "GeneratedQRCode", fileName);
                qrCodeImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                permit.QRCodeImage = image;
                permit.DownloadPath = fileName;
            }

            return View(permits);
        }

        [HttpGet("Guard/permitVerificationQrCode/OrgNumber/{OrgNumber}/RegistrationPlate/{RegistrationPlate}")]
        public IActionResult permitVerificationQrCode(long OrgNumber, long RegistrationPlate)
        {
            var obj = _context.Users.FirstOrDefault(x => x.orgNum == OrgNumber);
            if (obj != null)
            {
                var MediumCount = _context.StudentViolations.Where(x => x.Severity == "Medium" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                if (MediumCount == 2)
                {
                    return RedirectToAction("LockedAccount");
                }
                var SevereCount = _context.StudentViolations.Where(x => x.Severity == "Severe" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                if (SevereCount == 1)
                {
                    return RedirectToAction("LockedAccount");
                }
                var HighCount = _context.StudentViolations.Where(x => x.Severity == "High" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                if (HighCount == 1)
                {
                    return RedirectToAction("LockedAccount");

                }
                var LowCount = _context.StudentViolations.Where(x => x.Severity == "Low" && x.isPaid == false && x.OrgNumber == obj.orgNum).Count();
                if (LowCount == 4)
                {
                    return RedirectToAction("LockedAccount");
                }

                var permit = _context.Permits.FirstOrDefault(x => x.QRCodeNumber == RegistrationPlate);
                if (permit != null)
                {
                    var data = (from pr in _context.Permits
                                join v in _context.Vehicles on pr.regPlateNum equals v.regPlateNum
                                join D in _context.Users on pr.OrgNum equals D.orgNum
                                where pr.QRCodeNumber == RegistrationPlate && pr.OrgNum == OrgNumber
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

                    return View(data);
                }
                else
                {
                    return Json(error = "Driver does not have an active parking permit!");
                }
            }

            return NotFound();
        }


        public IActionResult LockedAccount()
        {
            return View();
        }
        //Phase two working start

        public IActionResult GuardSchedule()
        {
            var userId = _userManager.GetUserId(User);
            var LoginUser = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var obj = _context.GuardSchedules.ToList();

            GuardScheduleCheckInViewModel guard = new GuardScheduleCheckInViewModel
            {
                GuardSchedulesList = obj,
                LoginUserOrgNumber = LoginUser.orgNum
            };
            return View(guard);
        }

        public IActionResult GuardScheduleCheckIn(long OrgNumber)
        {
            var obj = _context.GuardSchedules.FirstOrDefault(x => x.OrgNumber == OrgNumber);

            if (obj != null)
            {
                DateTime currentTime = DateTime.Now;
                string status = obj.Status;

                DateTime onDutyTime = DateTime.Parse(obj.OnDutyTime);
                DateTime offDutyTime = DateTime.Parse(obj.OffDutyTime);

                if (currentTime < onDutyTime)
                {
                    if (status == "Off-Duty")
                    {
                        status = "Available";
                    }
                }
                else if (currentTime >= onDutyTime && currentTime <= offDutyTime)
                {
                    if (status == "Off-Duty")
                    {
                        status = "Late for Shift";
                    }
                    else if (status == "Available")
                    {
                        status = "On-Duty";
                    }
                    else if (status == "Late for Shift")
                    {
                        status = "On-Duty [Late]";
                    }
                }
                else if (currentTime > offDutyTime)
                {
                    status = "Off-Duty";
                }

                obj.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("GuardSchedule");
        }

        public IActionResult GuardScheduleCheckOut(long OrgNumber)
        {
            var obj = _context.GuardSchedules.Where(x => x.OrgNumber == OrgNumber).FirstOrDefault();
            if (obj != null)
            {
                DateTime currentTime = DateTime.Now;
                string currentTime24HourFormat = currentTime.ToString("HH:mm");
                if (currentTime24HourFormat == obj.OffDutyTime)
                {
                    obj.Status = "Off-Duty";
                    _context.GuardSchedules.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("GuardSchedule");
                }
                else
                {
                    obj.Status = "Extra-Duty";
                    _context.GuardSchedules.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("GuardSchedule");
                }

            }
            return RedirectToAction("GuardSchedule");
        }

        public IActionResult GuardScheduleLeave(long OrgNumber)
        {
            ViewBag.OrgNumber = OrgNumber;
            return View();
        }
        [HttpPost]
        public IActionResult GuardScheduleLeave(GuardScheduleLeaveDTO guardSchedule)
        {
            if (guardSchedule != null)
            {
                var obj = _context.GuardSchedules.Where(x => x.OrgNumber == guardSchedule.OrgNumber).FirstOrDefault();
                if (obj != null)
                {
                    obj.Status = "Unavaiable";
                    obj.LeaveTimeTo = guardSchedule.LeaveTo;
                    obj.LeaveTimeFrom = guardSchedule.LeaveFrom;
                    _context.GuardSchedules.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("GuardSchedule");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetSpot()
        {
            var obj = _context.ParkingBays.Where(x => x.Occupacy == "Unoccupied").FirstOrDefault();
            if (obj != null)
            {
                return new JsonResult(obj);
            }
            return BadRequest();
        }

        public IActionResult BookingNotExist()
        {
            var verfication = _context.StudentEvents.Where(x => x.isDelete == false).FirstOrDefault();
            if (verfication != null)
            {
                string CurrentDate = System.DateTime.Today.Date.ToString();
                DateTime currentDate = DateTime.Parse(CurrentDate);
                DateTime startDate = verfication.EventStartDate.Date;
                DateTime endDate = verfication.EventEndDate.Date;

                // Check if CurrentDate is within the range of EventStartDate and EventEndDate
                if (currentDate >= startDate && currentDate <= endDate)
                {
                    if (verfication.EventStatus == "Confirmed")
                    {
                        var check = _context.EventPayments.Where(x => x.EventId == verfication.Id).FirstOrDefault();
                        if (check == null)
                        {
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("StudentEvent");
                        }

                    }
                    else
                    {
                        return View();
                    }

                }
                else
                {
                    return RedirectToAction("LateEvent");
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult LateEvent()
        {
            return View();
        }

        public IActionResult StudentEvent()
        {
            string CurrentDate = System.DateTime.Today.Date.ToString();
            DateTime currentDate = DateTime.Parse(CurrentDate);

            var obj = (from E in _context.StudentEvents
                       join
                       u in _context.Users on E.OrgNumber equals u.orgNum
                       where E.EventStatus == "Confirmed" && E.isDelete == false && E.EventStartDate.Date == currentDate
                       select new EventHistoryViewModel
                       {
                           Email = u.Email,
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Phone = u.PhoneNumber,
                           EventName = E.EventName,
                           FromDate = E.EventStartDate,
                           ToDate = E.EventEndDate,
                           ParkingSection = E.ParkingBayId,
                           EventId = E.Id,
                       }).FirstOrDefault();
            return View(obj);
        }

        public IActionResult EventBookingCheckIn(EventCheckInDTO eventCheckInDTO)
        {
            if (eventCheckInDTO != null)
            {
                DateTime checkInTime = DateTime.Now.Date.Add(DateTime.Now.TimeOfDay);
                EventCheckIn check = new EventCheckIn
                {
                    CheckInTime = checkInTime.ToString(),
                    CheckOutTime = "",
                    RegPlate = eventCheckInDTO.RegPlate,
                    EventId = eventCheckInDTO.EventId
                };
                _context.EventCheckIns.Add(check);
                int Savecheck = _context.SaveChanges();
                if (Savecheck == 1)
                {
                    return Json(new { success = true, message = "Student CheckIn Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Student CheckIn failed" });
                }
            }
            return Json(new { success = false, message = "Error! Bay not deleted" });
        }
    }


}
