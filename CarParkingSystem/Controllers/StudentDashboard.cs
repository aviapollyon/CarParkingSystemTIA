using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;
using QRCoder;
using System.Collections;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboard : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        public StudentDashboard(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }
        public IActionResult PurchasePermit()
        {
            var getuserID = _userManager.GetUserId(User);
            if (getuserID != null)
            {
                var obj = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
                if (obj != null)
                {
                    var permits = _context.Permits.Where(x => x.OrgNum == obj.orgNum).ToList();
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
            }
            return NotFound();    
        }

        public IActionResult ObtainPermit()
        {
            var getuserID = _userManager.GetUserId(User);
            if(getuserID != null)
            {
                var obj = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
                
                UserInformationViewModel userInformationView = new UserInformationViewModel();
               
                if(obj != null)
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

                    var License = _context.Licenses.Where(x => x.idNumber == obj.idNumber).FirstOrDefault();                
                    var Vehicle = (from V in _context.Vehicles.Where(x => x.idNumber == obj.idNumber)
                                   select new UserVehicleViewModel
                                   {
                                       PlateNumber = V.regPlateNum,
                                       ExpiryDate = V.vehicleRegistrationDate,
                                   }).ToList();
                    var Permit = (from Per in _context.Permits.Where(x => x.OrgNum == obj.orgNum)
                                  select new PermitViewModel
                                  {
                                      IsGet = Per.isGet,
                                      regPlateNum = Per.regPlateNum,
                                  }).ToList();
                    
                    if(License !=null)
                    {
                         userInformationView = new UserInformationViewModel
                        {
                            FirstName = obj.FirstName,
                            LastName = obj.LastName,
                            IdNumber = obj.idNumber,
                            LicenseNumber = License.licenseNumber,
                            LicenceValidity =License.isValid,
                        };
                    }
                 
                  
                    UserParkingPermitViewModel viewModel = new UserParkingPermitViewModel
                    {
                        UserInformation = userInformationView,
                        UserVehicle = Vehicle,
                        PermitView = Permit,
                    };
                    return View(viewModel);
                }
            }
            return NotFound();
        }
        public IActionResult PermitPayment(string[] selectedIds)
        {
            int totalCount = selectedIds.Length * 100;
            ViewBag.Total = totalCount;
            ViewBag.Details = selectedIds;
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmationPermit(PermitPayment payment)
        {
            var getuserID = _userManager.GetUserId(User);
            int QRCodeNumber = 0;
            if (getuserID != null)
            {
                var obj = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
                if (obj != null)
                {
                    var vehicle = _context.Vehicles.Where(x => x.idNumber == obj.idNumber).FirstOrDefault();
                    Permit permit = null;
                    PermitPayment permitPayment = null;

                    if (vehicle != null)
                    {
                        for (int i = 0; i < payment.selectedIds.Length; i++)
                        {
                           QRCodeNumber = GenerateRandomNumber();
                            //Create Permit object
                           permit = new Permit
                           {
                               OrgNum = obj.orgNum,
                               regPlateNum = payment.selectedIds[i],
                               ExpDate = System.DateTime.Now.AddYears(1),
                               isGet = true,
                               QRCodeNumber = QRCodeNumber,
                           };
                            _context.Permits.Add(permit);
                            _context.SaveChanges();

                            // Create PermitPayment object (if not created already)
                            if (permitPayment == null)
                            {
                                permitPayment = new PermitPayment
                                {
                                    CardCVV = payment.CardCVV,
                                    CardNumber = payment.CardNumber,
                                    PaymentMode = "Permit purchase",
                                    DateTime = System.DateTime.Today.ToString(),
                                    IdNumber = obj.idNumber,
                                    TotalAmount = payment.selectedIds.Length * 100,
                                    RegNumber = payment.selectedIds[i]
                                };
                                _context.PermitPayment.Add(permitPayment);
                                _context.SaveChanges();
                            }

                            // Create OrderDetails object
                            OrderDetails order = new OrderDetails
                            {
                                IdNumber = obj.idNumber.ToString(),
                                PermitID = permit.Id,
                                PermitPaymentID = permitPayment.Id,
                            };
                            _context.OrderDetails.Add(order);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            int totalCount = payment.selectedIds.Length * 100;
            ViewBag.Total = totalCount;
            ViewBag.Details = payment.selectedIds;
            ViewBag.InvoiceNumber = QRCodeNumber;
            return View();
        }

        public IActionResult DownloadQRCode (string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return NotFound();
            }

            string fileName = Path.GetFileName(imageUrl);
            string filePath = Path.Combine(_environment.WebRootPath, "GeneratedQRCode", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/png", fileName);
        }

       

        public IActionResult Reservation()
        {
            var getuserID = _userManager.GetUserId(User);
            if (getuserID != null)
            {
                var obj = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
                if (obj != null)
                {
                    var permit = _context.Permits.Where(x => x.OrgNum == obj.orgNum).ToList();
                    var Data = (from rs in _context.Reservations
                                join pak in _context.ParkingBays
                                on rs.BayId equals pak.BayId
                                where rs.OrgNum == obj.orgNum
                                select new UserReservationViewModel
                                {
                                    BayId = pak.BayId,
                                    ExpiryDate = rs.ExpDate,
                                    Bay=pak.BayNumber,
                                    Section = pak.BaySection
                                }).ToList();
                    ReservationPermitViewModel permitViewModel = new ReservationPermitViewModel
                    {
                        Permits = permit,
                        Reservations = Data,
                    };
                    return View(permitViewModel);
                }
            }
            return NotFound();
        }

        public IActionResult ActiveCarPermit()
        {
            var getuserID = _userManager.GetUserId(User);
            if (getuserID != null)
            {

                var obj = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
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
                    var permits = _context.Permits.Where(x => x.OrgNum == obj.orgNum).ToList();
                    var reservations = _context.Reservations.Where(x => x.OrgNum == obj.orgNum).ToList();

                    ReserveViewModel reserveView = new ReserveViewModel
                    {
                        Permits = permits,
                        Reservations = reservations,
                    };
                    return View(reserveView);

                }
            }
            return NotFound();
        }
        public IActionResult ReservationPermit(string id)
        {
            var obj = _context.Bays.ToList();
            var parkingbay = _context.ParkingBays.ToList();
            ReserveViewModel reserveView = new ReserveViewModel
            {
                ParkingBays = parkingbay,
                Bays = obj,
            };
            ViewBag.RegNumber = id;
            return View(reserveView);
        }

        public IActionResult Availabilty()
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
        public IActionResult ParkingPayment(string BayID, string RegNumber)
        {
            ViewBag.BayID = BayID;
            ViewBag.RegNumber = RegNumber;
            return View();
        }
        [HttpPost]
        public IActionResult ConfirmPayment(ParkingDTO parking)
        {
            string FromDate = parking.ReservationDateFrom.ToString();
            string ToDate = parking.ReservationDateTo.ToString();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            TimeSpan difference = toDate - fromDate;
            int differenceInDays = (int)difference.TotalDays;         
            int TotalAmount = differenceInDays * 5;
            ViewBag.Total = TotalAmount;
            parking.TotalAmount = TotalAmount;
            ViewBag.ReservationExpDate = parking.ReservationDateTo.ToString();
            return View(parking);
        }

        public IActionResult ParkingPaymentConfirmation(ParkingDTO parking)
        {
            if(parking !=null)
            {
                var Vehicle = _context.Vehicles.Where(x => x.regPlateNum == parking.RegNumber).FirstOrDefault();
                if (Vehicle != null)
                {
                    var user = _context.Users.Where(x => x.idNumber == Vehicle.idNumber).FirstOrDefault();
                    if (user != null)
                    {
                        Reservations reservations = new Reservations
                        {
                            BayId = parking.BayID,
                            regPlateNum = parking.RegNumber,
                            isGet = true,
                            ExpDate = parking.ReservationDateTo,
                            OrgNum = user.orgNum,
                        };
                        _context.Reservations.Add(reservations);
                        _context.SaveChanges();

                        // update the status
                        var obj = _context.ParkingBays.Where(x => x.BayId == parking.BayID).FirstOrDefault();
                        if(obj !=null)
                        {
                            obj.Occupacy = "Reserved";
                            _context.ParkingBays.Update(obj);
                            _context.SaveChanges();
                        }

                        ParkingPayment Payment = new ParkingPayment
                        {

                            CardCVV = parking.CardCVV,
                            DateTime = parking.DateTime,
                            CardNumber = parking.CardNumber,
                            IdNumber = user.idNumber,
                            PaymentMode = "Parking purchase",
                            TotalAmount = parking.TotalAmount,     
                            RegNumber = parking.RegNumber,
                        };
                        _context.parkingPayments.Add(Payment);
                        _context.SaveChanges();
                        //Add in Details 

                        ParkingOrderDetails parkingOrder = new ParkingOrderDetails
                        {
                            IdNumber = user.idNumber,
                            PaymentId = Payment.Id,
                            RevervationID = reservations.Id,                   
                        };
                        _context.ParkingOrderDetails.Add(parkingOrder);
                        _context.SaveChanges();

                        var GetQRCodePermit = _context.Permits.Where(x => x.regPlateNum == parking.RegNumber).Select(x => x.QRCodeNumber).FirstOrDefault();
                        ViewBag.Total = parking.TotalAmount;
                        ViewBag.BayID = parking.BayID;
                        ViewBag.InvoiceNumber = GetQRCodePermit;
                        return View();

                    }
                }
            }
            return NotFound();
        }

        public IActionResult StudentReview()
        {
            var feedbacks = _context.Feedbacks.ToList();

            double averageRating = feedbacks.Any() ? feedbacks.Average(f => f.RatingNumber) : 0;

            int[] starCounts = new int[5];
            foreach (var feedback in feedbacks)
            {
                starCounts[feedback.RatingNumber - 1]++;
            }

            ViewBag.AverageRating = averageRating;
            ViewBag.StarCounts = starCounts;

            return View(feedbacks);

        }

        public IActionResult AddReview()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddReview(Feedback feedback)
        {
            var getuserID = _userManager.GetUserId(User);
            if(getuserID !=null)
            {
                var getData = _context.Users.Where(x => x.Id == getuserID).FirstOrDefault();
                feedback.OrgNumber = getData.orgNum.ToString();
                feedback.RatingDate = System.DateTime.Now;
                feedback.FeedbackStatus = "UnRead";
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                return RedirectToAction("StudentReview");
            }
            else
            {
                return View(feedback);
            }          
           
        }
        public IActionResult ReviewDetails(int id)
        {
            var obj = _context.Feedbacks.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return NotFound();
        }

        public IActionResult ParkingHistory()
        {
            var getuserID = _userManager.GetUserId(User);
            if (getuserID != null)
            {
                var ogrNumber = _context.Users.Where(x => x.Id == getuserID).Select(x => x.orgNum).FirstOrDefault();
                var obj = (from checkin in _context.CheckIns join
                           checkout in _context.checkOuts on
                           checkin.OrgNum equals checkout.OrgNum join
                           u in _context.Users on checkin.OrgNum equals u.orgNum
                           where checkin.OrgNum == ogrNumber
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
            return NotFound();
        }
        public IActionResult Invoice(long? QRCodeNumber)
        {
            if (QRCodeNumber !=null)
            {
                var check = _context.Permits.Where(x => x.QRCodeNumber == QRCodeNumber).Select(x => x.regPlateNum).FirstOrDefault();
                var otherCheck = _context.parkingPayments.Where(x => x.RegNumber == check).FirstOrDefault();
                if(otherCheck != null)
                {
                    return RedirectToAction("ReservationInvoice", new { qrCodeNumber = QRCodeNumber });
                }
                else
                {
                    var obj = (from qr in _context.Permits
                               join
                               pr in _context.PermitPayment on
                               qr.regPlateNum equals pr.RegNumber
                               join u in _context.Users on
                               pr.IdNumber equals u.idNumber                              
                               where qr.QRCodeNumber == QRCodeNumber
                               select new InvoiceViewModel
                               {
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,                              
                                   Phone = u.PhoneNumber,
                                   QrCode = qr.QRCodeNumber,
                                   RegNumber = qr.regPlateNum,                     
                                   PermitAmount = pr.TotalAmount,
                                   TotalAmunt = pr.TotalAmount,
                                  
                               }).FirstOrDefault();
                    return View(obj);
                }                            
            }
            return NotFound();
        }

        public IActionResult ReservationInvoice(long? QRCodeNumber)
        {
            var obj = (from qr in _context.Permits
                       join
                       pr in _context.PermitPayment on
                       qr.regPlateNum equals pr.RegNumber
                       join rs in _context.Reservations
                       on qr.regPlateNum equals rs.regPlateNum
                       join u in _context.Users on
                       pr.IdNumber equals u.idNumber
                       join
                       parkpy in _context.parkingPayments on
                       u.idNumber equals parkpy.IdNumber
                       where parkpy.RegNumber == qr.regPlateNum && qr.QRCodeNumber == QRCodeNumber
                       select new InvoiceViewModel
                       {
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Email = u.Email,
                           BayId = rs.BayId,
                           Phone = u.PhoneNumber,
                           QrCode = qr.QRCodeNumber,
                           RegNumber = qr.regPlateNum,
                           ParkingAmount = parkpy.TotalAmount,
                           PermitAmount = pr.TotalAmount,
                           TotalAmunt = parkpy.TotalAmount + pr.TotalAmount,
                           day = (parkpy.TotalAmount / 5).ToString("0"),
                       }).FirstOrDefault();
            return View(obj);
        }

        public IActionResult StudentViolation()
        {
            var getuserID = _userManager.GetUserId(User);
            if (getuserID != null)
            {
                var ogrNumber = _context.Users.Where(x => x.Id == getuserID).Select(x => x.orgNum).FirstOrDefault();
                if(ogrNumber > 0)
                {
                    var obj = _context.StudentViolations.Where(x => x.OrgNumber == ogrNumber).ToList();
                    var payment = _context.ViolationPayments.ToList();
                    ViewBag.Check = payment;
                    return View(obj);
                }

            }
            return NotFound();
        }

        public IActionResult ViolationDetails(int id)
        {
            var obj = _context.StudentViolations.Find(id);
            var ImagesData = _context.ViolationImages.Where(x => x.StudentViolationId == id).ToList();
            var appeal = _context.ViolationAppeals.Where(x => x.ViolationId == id).FirstOrDefault();
            StudentViolationViewModel studentViolation = new StudentViolationViewModel
            {
                StudentViolation = obj,
                ViolationImages = ImagesData,
                violationAppeals = appeal,
            };
            return View(studentViolation);
        }

        public IActionResult ViolationPay(int id)
        {
            var obj = _context.StudentViolations.Find(id);
            return View(obj);
        }

        [HttpPost]
        public IActionResult ViolationPay(ViolationPayment violationPayment)
        {
            ViolationPayment violation = new ViolationPayment
            {
                CardCVV = violationPayment.CardCVV,
                CardNumber = violationPayment.CardNumber,
                DateTime = violationPayment.DateTime,
                TotalAmount = violationPayment.TotalAmount,
                ViolationId = violationPayment.ViolationId,
            };
            _context.ViolationPayments.Add(violation);
            _context.SaveChanges();

            var obj = _context.StudentViolations.Where(x => x.Id == violationPayment.ViolationId).FirstOrDefault();
            if(obj !=null)
            {
                obj.isPaid = true;
                _context.StudentViolations.Update(obj);
                _context.SaveChanges();
                return RedirectToAction("ConfirmedPay", new { id = violationPayment.ViolationId });
            }
            return RedirectToAction("ConfirmedPay", new { id = violationPayment.ViolationId });     
        }

        public  IActionResult ConfirmedPay(int id)
        {
            var obj = _context.StudentViolations.Find(id);
            return View(obj);
        }

        public IActionResult DownloadViolationInvoice(long OrgNumber)
        {
            var obj = (from u in _context.Users join
                       sr in _context.StudentViolations on
                       u.orgNum equals sr.OrgNumber
                       join pay in _context.ViolationPayments on
                       sr.Id equals pay.ViolationId
                       where sr.OrgNumber == OrgNumber
                       select new InvoiceViewModel
                       {
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Email = u.Email,
                           Phone = u.PhoneNumber,
                           TotalAmunt = pay.TotalAmount,
                           QrCode = pay.Id,
                           RegNumber = sr.RegPlate,
                       }).FirstOrDefault();
            return View(obj);
        }
        public IActionResult ViolationAppeal(int id)
        {
            var obj = _context.StudentViolations.Find(id);
            return View(obj);
        }
        [HttpPost]
        public IActionResult ViolationAppeal(ViolationAppeal violationAppeal)
        {
            var check = _context.ViolationAppeals.Where(x => x.Status == "Reject" && x.ViolationId == violationAppeal.ViolationId).FirstOrDefault();
            if(check !=null)
            {
                check.Status =  violationAppeal.Status = "Unconfirmed";
                check.AppealMessage = violationAppeal.AppealMessage;
                check.ViolationId = violationAppeal.ViolationId;
                check.ReasonAppeal = violationAppeal.ReasonAppeal;
                _context.ViolationAppeals.Update(check);
                _context.SaveChanges();
                return RedirectToAction("ConfirmedAppeal", new { id = violationAppeal.ViolationId });
            }
            else
            {

                violationAppeal.Status = "Unconfirmed";
                _context.ViolationAppeals.Add(violationAppeal);
                _context.SaveChanges();
                return RedirectToAction("ConfirmedAppeal", new { id = violationAppeal.ViolationId });

            }

        }

        public IActionResult SubmittedAppeal()
        {
            return View();
        }
        public IActionResult ConfirmedAppeal(int id)
        {
            var obj = _context.StudentViolations.Find(id);
            return View(obj);
        }

        public IActionResult LockedAccount()
        {
            return View();
        }
        private int GenerateRandomNumber()
        {
            Random random = new Random();
            int randomNumber = random.Next(100000000, 999999999);
            return randomNumber;
        }
    }
}
