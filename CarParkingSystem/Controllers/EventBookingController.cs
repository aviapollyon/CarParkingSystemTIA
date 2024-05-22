using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class EventBookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public EventBookingController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> manager)
        {
          _context = applicationDbContext;
          _userManager = manager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            if(userId != null)
            {
                var obj = _context.Users.Where(x=>x.Id ==  userId).FirstOrDefault();    
                if(obj != null)
                {
                    var verfication = _context.StudentEvents.Where(x=>x.OrgNumber == obj.orgNum && x.isDelete == false).FirstOrDefault();
                    if(verfication != null)
                    {
                        string CurrentDate = System.DateTime.Today.Date.ToString();
                        DateTime currentDate = DateTime.Parse(CurrentDate);
                        DateTime startDate = verfication.EventStartDate.Date;
                        DateTime endDate = verfication.EventEndDate.Date;

                        // Check if CurrentDate is within the range of EventStartDate and EventEndDate
                        if (currentDate >= startDate && currentDate <= endDate)
                        {
                            if (verfication.EventStatus == "unConfirm")
                            {
                                var check = _context.EventPayments.Where(x => x.EventId == verfication.Id).FirstOrDefault();
                                if (check == null)
                                {
                                   
                                    return RedirectToAction("BookingExit");
                                }
                                else
                                {
                                    return RedirectToAction("BookingList");
                                }
                            }
                            else
                            {
                                var check = _context.EventPayments.Where(x => x.EventId == verfication.Id).FirstOrDefault();
                                if(check == null)
                                {
                                    return RedirectToAction("BookingList");
                                }
                                else
                                {
                                    return RedirectToAction("BookedEvent");
                                }
                               
                            }

                        }
                        else
                        {
                            return RedirectToAction("BookingExit");
                        }
                    }
                    else
                    {
                        var check = _context.StudentEvents.Where(x =>x.EventStatus == "Confirmed" && x.isDelete == false).OrderByDescending(x=>x.Id).FirstOrDefault();
                        if(check !=null)
                        {
                            var Feecheck = _context.EventPayments.Where(x => x.EventId == check.Id).FirstOrDefault();
                            if(Feecheck == null)
                            {
                                return RedirectToAction("NoBookingExit");
                            }
                            else
                            {
                                return RedirectToAction("BookingExit");
                            }
                        }
                        else
                        {
                            return RedirectToAction("NoBookingExit");
                        }
                       
                    }
                }

            }
            return View();
        }

        public IActionResult BookingExit()
        {
            return View();
        }

        public IActionResult NoBookingExit()
        {
            return View();
        }
        public IActionResult RequestBooking()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RequestBooking(StudentEvent studentEvent)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var obj = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                if(obj !=null)
                {
                    //update in parking bay
                    var parkingbay = _context.ParkingBays.Where(x => x.BayId == studentEvent.ParkingBayId).FirstOrDefault();
                    if (parkingbay != null)
                    {
                        parkingbay.Occupacy = "Unavailable";
                        _context.ParkingBays.Update(parkingbay);
                        _context.SaveChanges();

                        studentEvent.ParkingBayId = parkingbay.BaySection;
                    }
                    int TotalAmount = 0;
                    string FromDate = studentEvent.EventStartDate.ToString();
                    string ToDate = studentEvent.EventEndDate.ToString();

                    string FromTime = studentEvent.StartTime;
                    string EndTime = studentEvent.EndTime;

                    DateTime fromDate = DateTime.Parse(FromTime);
                    DateTime toDate = DateTime.Parse(EndTime);

                    TimeSpan difference = toDate - fromDate;
                    int differenceInMinutes = (int)difference.TotalMinutes;
                    if (differenceInMinutes < 60)
                    {

                        ModelState.AddModelError("", "The event duration should be at least 1 hour.");
                        return View(studentEvent);
                    }
                    else
                    {
                        int hourlyRate = 500; 
                        TotalAmount = (differenceInMinutes / 60) * hourlyRate;
                        int remainingMinutes = differenceInMinutes % 60;
                        if (remainingMinutes > 0)
                        {
                            TotalAmount += (int)Math.Ceiling((double)remainingMinutes / 60) * hourlyRate; 
                        }
                    }
                    TimeSpan FromTimes = TimeSpan.Parse(studentEvent.StartTime);
                    TimeSpan ToTimes = TimeSpan.Parse(studentEvent.EndTime);

                    studentEvent.EventStartDate = studentEvent.EventStartDate + FromTimes;
                    studentEvent.EventEndDate = studentEvent.EventEndDate + ToTimes;

                    studentEvent.isDelete = false;
                    studentEvent.EventStatus = "unConfirm";
                    studentEvent.OrgNumber = obj.orgNum;
                    studentEvent.AmountPaid = TotalAmount;
                    _context.StudentEvents.Add(studentEvent);
                    _context.SaveChanges();


                    return RedirectToAction("Index");
                }

               

            }
            return View();
        }

        public IActionResult RequestBookedPaymet(int id)
        {
            var obj = _context.StudentEvents.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {

                return View(obj);
             }

            return NotFound();
        }
        [HttpPost]
        public IActionResult RequestBookedPaymet(EventPayment eventPayment)
        {
            if(eventPayment !=null)
            {
                _context.EventPayments.Add(eventPayment);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            return RedirectToAction("RequestBookedPaymet", new { id = eventPayment.EventId });
        }
        public IActionResult BookingList()
        {
            var userId = _userManager.GetUserId(User);
            if(userId !=null)
            {
                var LoginUser = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                if(LoginUser !=null)
                {
                    var obj = _context.StudentEvents.Where(x=>x.OrgNumber == LoginUser.orgNum).ToList();
                    return View(obj);
                }
            }
            return View();
        }
        //show login user booked event
        public IActionResult BookedEvent()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var LoginUser = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (LoginUser != null)
                {
                    var obj = (from E in _context.StudentEvents join
                               u in _context.Users on E.OrgNumber equals u.orgNum
                               where E.OrgNumber == LoginUser.orgNum
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
                               }).FirstOrDefault();
                    return View(obj);
                }
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult GetSpot()
        {
            var obj = _context.ParkingBays.Where(x => x.Occupacy == "Unoccupied").ToList();
            if (obj != null)
            {
                return new JsonResult(obj);
            }
            return BadRequest();
        }

        public async Task<IActionResult> Delete(int id)
        {

            var Event = await _context.StudentEvents.FindAsync(id);
            if (Event != null)
            {
                Event.isDelete = true;
                _context.StudentEvents.Update(Event);
                int check = _context.SaveChanges();
                if (check == 1)
                {
                    return Json(new { success = true, message = "Event cencel successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Error! Event is not Update" });
                }
            }
            return Json(new { success = false, message = "Error! System erroor" });
        }
    }
}
