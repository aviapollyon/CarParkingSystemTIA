using CarParkingSystem.Areas.Identity.Data;
using CarParkingSystem.DTO;
using CarParkingSystem.Models;
using CarParkingSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboard : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminDashboard(ApplicationDbContext applicationDbContext) 
        {
          _context = applicationDbContext;
        }
        public IActionResult ActiveParkingPermit()
        {
            var obj = _context.Permits.ToList();
            return View(obj);
        }

        public IActionResult ActiveReservation()
        {
            var obj = _context.Reservations.ToList();
            return View(obj);
        }

        public IActionResult UserFeedback()
        {
            var obj = _context.Feedbacks.ToList();
            return View(obj);
        }
        public IActionResult ReplyReview(int id)
        {
            var obj = _context.Feedbacks.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return NotFound();
        }

        public IActionResult UpdateReview(Feedback feedback)
        {
            if(feedback !=null)
            {
                var obj = _context.Feedbacks.Find(feedback.Id);
                if(obj !=null)
                {
                    obj.FeedbackStatus = "Read";
                    obj.AdminReplay = feedback.AdminReplay;
                    _context.Feedbacks.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("UserFeedback");
                }
                return NotFound();
            }
            return NotFound();
        }

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
        public IActionResult UpdateReservation(int? id)
        {
            var getData = _context.Reservations.Find(id);
            if (getData != null)
            {
                getData.isGet = false;
                _context.Reservations.Update(getData);
                _context.SaveChanges();
                return Json(new { success = true, message = "Reservation has been cancel successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error! Reservation did not cancel. please try again" });

            }
        }

        public IActionResult UpdateParkingPermit(int? id)
        {
            var getData = _context.Permits.Find(id);
            if (getData != null)
            {
                getData.isGet = false;
                _context.Permits.Update(getData);
                _context.SaveChanges();
                return Json(new { success = true, message = "Permit has been cancel successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error! Permit did not cancel. please try again" });

            }
        }
        public IActionResult PendingEvent()
        {
            var obj = _context.StudentEvents.ToList();
            ViewBag.Payment = _context.EventPayments.ToList();
            return View(obj);
        }
        public IActionResult EventDetails(int id)
        {
            var obj = _context.StudentEvents.Find(id);
            return View(obj);
        }

        public IActionResult EventHistory(int id)
        {
            var obj = (from E in _context.StudentEvents join
                        u in _context.Users on E.OrgNumber equals u.orgNum
                        where E.Id == id
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
                            EventId = E.Id
                        }).FirstOrDefault();
            return View(obj);
        }

        public IActionResult UpdateEventStatus(int id)
        {
            var obj = _context.StudentEvents.Find(id);
            if(obj !=null)
            {
                obj.EventStatus = "Confirmed";
                _context.StudentEvents.Update(obj);
                int check = _context.SaveChanges();
                if (check == 1)
                {
                    return Json(new { success = true, message = "Event Status Update Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Error! Event Status not update" });
                }
            }
            return Json(new { success = false, message = "Error! System error." });
        }
        public IActionResult EndEvent(int id)
        {
            var obj = _context.StudentEvents.Find(id);
            if (obj != null)
            {
                 obj.isDelete = true;
                _context.StudentEvents.Update(obj);

                var update = _context.ParkingBays.Where(x => x.BaySection == obj.ParkingBayId).FirstOrDefault();
                if(update !=null)
                {
                     update.Occupacy = "Unoccupied";
                    _context.ParkingBays.Update(update);
                    _context.SaveChanges();
                    return RedirectToAction("PendingEvent");
                }

                return RedirectToAction("PendingEvent");
            }
            return RedirectToAction("PendingEvent");
        }

        public IActionResult EventArrivals(int id)
        {
            ViewBag.EventName = _context.StudentEvents.Where(x => x.Id == id).Select(x => x.EventName).FirstOrDefault();
            var obj = _context.EventCheckIns.Where(x => x.EventId == id).ToList();
            return View(obj);
        }

        public IActionResult StudentAppeal()
        {
            var obj = (from sv in _context.StudentViolations
                       join ap in _context.ViolationAppeals on
                       sv.Id equals ap.ViolationId
                       join user in _context.Users
                       on sv.OrgNumber equals user.orgNum
                       where sv.AppealSent == false && ap.Status == "Unconfirmed"
                       select new StudentAppealViewModel
                       {
                           Id = sv.Id,
                           Severity = sv.Severity,
                           FullName = user.FirstName + " " + user.LastName,
                           IssueDate = sv.IssueDate,
                           ispaid = sv.isPaid
                       }).ToList();
            return View(obj);
        }

        public IActionResult AppealsDetails(int id)
        {
            var check = _context.StudentViolations.Where(x => x.Id == id).Select(x => x.Severity).FirstOrDefault();
            if(check == "Severe")
            {
              
                return RedirectToAction("SevereAppeal", new { id = id });
            }
            else
            {
                var obj = (from sv in _context.StudentViolations
                           join ap in _context.ViolationAppeals on
                           sv.Id equals ap.ViolationId
                           where sv.Id == id && ap.ViolationId == id 
                           select new StudentAppealViewModel
                           {
                               Id = sv.Id,
                               OffenseSelect = sv.OffenseSelect,
                               PartiesInvolved = sv.PartiesInvolved,
                               AdditionInformation = sv.AdditionInformation,
                               AppealMessage = ap.AppealMessage,
                               Reason = ap.ReasonAppeal
                           }).FirstOrDefault();

                var appealImage = _context.ViolationImages.Where(x => x.StudentViolationId == id).ToList();

                AppealViewModel appeal = new AppealViewModel
                {
                    StudentAppeal = obj,
                    ViolationImages = appealImage,
                };
                return View(appeal);
            }
            
        }

        public IActionResult SevereAppeal(int id)
        {
            var obj = (from sv in _context.StudentViolations
                       join ap in _context.ViolationAppeals on
                       sv.Id equals ap.ViolationId
                       where sv.Id == id && ap.ViolationId == id
                       select new StudentAppealViewModel
                       {
                           Id = sv.Id,
                           OffenseSelect = sv.OffenseSelect,
                           PartiesInvolved = sv.PartiesInvolved,
                           AdditionInformation = sv.AdditionInformation,
                           AppealMessage = ap.AppealMessage,
                           Reason = ap.ReasonAppeal,
                           ServerityFee = sv.ServerityFee,
                           
                       }).FirstOrDefault();

            var appealImage = _context.ViolationImages.Where(x => x.StudentViolationId == id).ToList();

            AppealViewModel appeal = new AppealViewModel
            {
                StudentAppeal = obj,
                ViolationImages = appealImage,
            };
            return View(appeal);
        }

        public IActionResult ApproveAppeal(int id)
        {
            var update = _context.ViolationAppeals.Where(x => x.ViolationId == id).FirstOrDefault();
            if(update !=null)
            {
                update.Status = "confirmed";
                _context.ViolationAppeals.Update(update);
                _context.SaveChanges();
                var VA = _context.StudentViolations.Where(x => x.Id == id).FirstOrDefault();
                if(VA !=null)
                {
                    VA.AppealSent = true;
                    VA.isPaid = true;
                    _context.StudentViolations.Update(VA);
                    _context.SaveChanges();
                }
            }
            var obj = (from sv in _context.StudentViolations
                       join ap in _context.ViolationAppeals on
                       sv.Id equals ap.ViolationId
                       where sv.Id == id && ap.ViolationId == id
                       select new StudentAppealViewModel
                       {
                           Id = sv.Id,                         
                           AdditionInformation = sv.AdditionInformation,
                           AppealMessage = ap.AppealMessage,
                           Reason = ap.ReasonAppeal,
                           Severity = sv.Severity,
                           IssueDate = sv.IssueDate,
                       }).FirstOrDefault();
            return View(obj);
        }
        public IActionResult DenyAppeal(int id)
        {
            var update = _context.ViolationAppeals.Where(x => x.ViolationId == id).FirstOrDefault();
            if (update != null)
            {
                update.Status = "Reject";
                _context.ViolationAppeals.Update(update);
                _context.SaveChanges();
            }
            var obj = (from sv in _context.StudentViolations
                       join ap in _context.ViolationAppeals on
                       sv.Id equals ap.ViolationId
                       where sv.Id == id && ap.ViolationId == id
                       select new StudentAppealViewModel
                       {
                           Id = sv.Id,
                           AdditionInformation = sv.AdditionInformation,
                           AppealMessage = ap.AppealMessage,
                           Reason = ap.ReasonAppeal,
                           Severity = sv.Severity,
                           IssueDate = sv.IssueDate,
                       }).FirstOrDefault();
            return View(obj);
        }

        public IActionResult SeverePayment(SeverePaymentDTO paymentDTO)
        {
            if (paymentDTO != null)
            {
                if(paymentDTO.Action == "SetPaymentDue")
                {
                    var update = _context.ViolationAppeals.Where(x => x.ViolationId == paymentDTO.ViolationId).FirstOrDefault();
                    if (update != null)
                    {
                        update.Status = "Reject";
                        _context.ViolationAppeals.Update(update);
                        _context.SaveChanges();
                    }

                    var SV = _context.StudentViolations.Where(x => x.Id == paymentDTO.ViolationId).FirstOrDefault();
                    if (SV != null)
                    {
                        SV.ServerityFee = paymentDTO.SevereFee;
                        _context.StudentViolations.Update(SV);
                        _context.SaveChanges();

                        return RedirectToAction("StudentAppeal");
                    }
                }
                else
                {
                    var update = _context.ViolationAppeals.Where(x => x.ViolationId == paymentDTO.ViolationId).FirstOrDefault();
                    if (update != null)
                    {
                        update.Status = "directAppeal";
                        _context.ViolationAppeals.Update(update);
                        _context.SaveChanges();
                    }
                    var SV = _context.StudentViolations.Where(x => x.Id == paymentDTO.ViolationId).FirstOrDefault();
                    if (SV != null)
                    {
                         SV.isPaid = true;
                        _context.StudentViolations.Update(SV);
                        _context.SaveChanges();

                        return RedirectToAction("StudentAppeal");
                    }
                }
                
            }
            return NotFound();
        }
    }
}

